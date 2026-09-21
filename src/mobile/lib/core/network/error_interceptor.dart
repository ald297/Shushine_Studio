import 'package:dio/dio.dart';
import '../auth/token_storage.dart';
import '../error/failures.dart';

class ErrorInterceptor extends Interceptor {
  final TokenStorage _tokenStorage;
  final void Function()? onUnauthorized;

  ErrorInterceptor({
    required TokenStorage tokenStorage,
    this.onUnauthorized,
  }) : _tokenStorage = tokenStorage;

  @override
  void onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    final token = await _tokenStorage.getToken();
    if (token != null && token.isNotEmpty) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    options.headers['Accept'] = 'application/json';
    options.headers['Content-Type'] = 'application/json';
    return handler.next(options);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    Failure failure;

    switch (err.type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
      case DioExceptionType.connectionError:
        failure = const NetworkFailure(
          message: 'Error de conexión con el salón. Intente de nuevo más tarde.',
        );
        break;

      case DioExceptionType.badResponse:
        final statusCode = err.response?.statusCode;
        final responseData = err.response?.data;

        if (statusCode == 401) {
          await _tokenStorage.clearAll();
          if (onUnauthorized != null) {
            onUnauthorized!();
          }
          failure = const AuthFailure(
            message: 'Su sesión ha expirado o no es válida. Inicie sesión nuevamente.',
            statusCode: 401,
          );
        } else if (statusCode == 403) {
          failure = const AuthFailure(
            message: 'No cuenta con los permisos necesarios para realizar esta acción.',
            statusCode: 403,
          );
        } else if (responseData is Map<String, dynamic>) {
          // Deserialización RFC 7807 Problem Details
          final detail = responseData['detail']?.toString();
          final title = responseData['title']?.toString();
          final errors = responseData['errors'];

          String message = detail ?? title ?? 'Ocurrió un error en la solicitud.';

          if (errors is Map<String, dynamic> && errors.isNotEmpty) {
            final firstErrorKey = errors.keys.first;
            final firstErrorVal = errors[firstErrorKey];
            if (firstErrorVal is List && firstErrorVal.isNotEmpty) {
              message = '$detail (${firstErrorVal.first})';
            }
          }

          failure = ServerFailure(
            message: message,
            statusCode: statusCode,
            detail: detail,
            errors: errors is Map<String, dynamic> ? errors : null,
          );
        } else {
          failure = ServerFailure(
            message: 'Error interno del servidor. Por favor intente más tarde.',
            statusCode: statusCode ?? 500,
          );
        }
        break;

      case DioExceptionType.cancel:
        failure = const FailureDefault(message: 'La solicitud fue cancelada.');
        break;

      default:
        failure = ServerFailure(
          message: err.message ?? 'Error inesperado de comunicación.',
        );
    }

    final customException = DioException(
      requestOptions: err.requestOptions,
      response: err.response,
      type: err.type,
      error: failure,
      message: failure.message,
    );

    return handler.next(customException);
  }
}

class FailureDefault extends Failure {
  const FailureDefault({required super.message});
}

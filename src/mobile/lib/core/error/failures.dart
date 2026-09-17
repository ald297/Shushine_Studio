import 'package:equatable/equatable.dart';

/// Base abstract class for all failure representations in the application.
abstract class Failure extends Equatable {
  final String message;
  final int? statusCode;

  const Failure({required this.message, this.statusCode});

  @override
  List<Object?> get props => [message, statusCode];
}

/// Represents failures originating from the backend API (RFC 7807 Problem Details).
class ServerFailure extends Failure {
  final String? detail;
  final Map<String, dynamic>? errors;

  const ServerFailure({
    required super.message,
    super.statusCode,
    this.detail,
    this.errors,
  });

  @override
  List<Object?> get props => [message, statusCode, detail, errors];
}

/// Represents authentication and authorization failures (HTTP 401 / 403).
class AuthFailure extends Failure {
  const AuthFailure({required super.message, super.statusCode});
}

/// Represents network connectivity errors, DNS failure, or timeouts.
class NetworkFailure extends Failure {
  const NetworkFailure({
    super.message = 'Error de conexión. Verifique su acceso a internet.',
    super.statusCode,
  });
}

/// Represents cache or secure storage access failures.
class CacheFailure extends Failure {
  const CacheFailure({
    super.message = 'Error al acceder al almacenamiento local seguro.',
    super.statusCode,
  });
}

/// Represents client-side or server-side input validation errors.
class ValidationFailure extends Failure {
  final Map<String, List<String>> fieldErrors;

  const ValidationFailure({
    required super.message,
    this.fieldErrors = const {},
    super.statusCode = 400,
  });

  @override
  List<Object?> get props => [message, statusCode, fieldErrors];
}

/// Generic functional Result type to handle operations that can succeed or fail.
sealed class Result<T> {
  const Result();

  bool get isSuccess => this is Success<T>;
  bool get isFailure => this is ErrorResult<T>;

  T? get dataOrNull => this is Success<T> ? (this as Success<T>).data : null;
  Failure? get failureOrNull =>
      this is ErrorResult<T> ? (this as ErrorResult<T>).failure : null;

  R when<R>({
    required R Function(T data) onSuccess,
    required R Function(Failure failure) onError,
  }) {
    if (this is Success<T>) {
      return onSuccess((this as Success<T>).data);
    } else {
      return onError((this as ErrorResult<T>).failure);
    }
  }
}

class Success<T> extends Result<T> {
  final T data;
  const Success(this.data);
}

class ErrorResult<T> extends Result<T> {
  final Failure failure;
  const ErrorResult(this.failure);
}

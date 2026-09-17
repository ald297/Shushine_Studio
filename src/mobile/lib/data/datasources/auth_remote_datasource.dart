import 'package:dio/dio.dart';
import '../../core/error/failures.dart';
import '../../core/network/dio_client.dart';
import '../models/user_dto.dart';

abstract class AuthRemoteDataSource {
  Future<UserDto> login({required String login, required String clave});
  Future<UserDto> register({
    required String nombre,
    String? apellido,
    String? telefono,
    required String login,
    required String clave,
    int? rolId,
  });
  Future<UserDto> getCurrentUser();
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  final DioClient _dioClient;

  AuthRemoteDataSourceImpl({required DioClient dioClient})
      : _dioClient = dioClient;

  @override
  Future<UserDto> login({required String login, required String clave}) async {
    try {
      final response = await _dioClient.post(
        '/auth/login',
        data: {'login': login, 'clave': clave},
      );

      final data = response.data as Map<String, dynamic>;
      final token = data['token']?.toString();
      return UserDto.fromJson(data, token: token);
    } on DioException catch (e) {
      if (e.error is Failure) {
        throw e.error as Failure;
      }
      throw ServerFailure(
        message: e.message ?? 'Error al iniciar sesión en el servidor.',
        statusCode: e.response?.statusCode,
      );
    }
  }

  @override
  Future<UserDto> register({
    required String nombre,
    String? apellido,
    String? telefono,
    required String login,
    required String clave,
    int? rolId,
  }) async {
    try {
      final response = await _dioClient.post(
        '/auth/registro',
        data: {
          'nombre': nombre,
          'apellido': apellido,
          'telefono': telefono,
          'login': login,
          'clave': clave,
          'rolId': rolId,
        },
      );

      final data = response.data as Map<String, dynamic>;
      final token = data['token']?.toString();
      return UserDto.fromJson(data, token: token);
    } on DioException catch (e) {
      if (e.error is Failure) {
        throw e.error as Failure;
      }
      throw ServerFailure(
        message: e.message ?? 'Error al registrar nuevo usuario en el servidor.',
        statusCode: e.response?.statusCode,
      );
    }
  }

  @override
  Future<UserDto> getCurrentUser() async {
    try {
      final response = await _dioClient.get('/auth/me');
      final data = response.data as Map<String, dynamic>;
      return UserDto.fromJson(data);
    } on DioException catch (e) {
      if (e.error is Failure) {
        throw e.error as Failure;
      }
      throw ServerFailure(
        message: e.message ?? 'Error al obtener el perfil de usuario.',
        statusCode: e.response?.statusCode,
      );
    }
  }
}

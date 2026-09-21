import '../../core/error/failures.dart';
import '../../data/models/user_dto.dart';

abstract class AuthRepository {
  Future<Result<UserDto>> login({required String login, required String clave});
  Future<Result<UserDto>> register({
    required String nombre,
    String? apellido,
    String? telefono,
    required String login,
    required String clave,
    int? rolId,
  });
  Future<Result<UserDto>> getCurrentUser();
  Future<Result<void>> logout();
  Future<bool> isAuthenticated();
}

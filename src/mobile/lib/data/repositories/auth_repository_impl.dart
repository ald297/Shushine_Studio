import '../../core/auth/token_storage.dart';
import '../../core/error/failures.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth_remote_datasource.dart';
import '../models/user_dto.dart';

class AuthRepositoryImpl implements AuthRepository {
  final AuthRemoteDataSource _remoteDataSource;
  final TokenStorage _tokenStorage;

  AuthRepositoryImpl({
    required AuthRemoteDataSource remoteDataSource,
    required TokenStorage tokenStorage,
  })  : _remoteDataSource = remoteDataSource,
        _tokenStorage = tokenStorage;

  @override
  Future<Result<UserDto>> login({required String login, required String clave}) async {
    try {
      final user = await _remoteDataSource.login(login: login, clave: clave);
      if (user.token != null && user.token!.isNotEmpty) {
        await _tokenStorage.saveToken(user.token!);
        await _tokenStorage.saveUserData(
          id: user.id,
          login: user.login,
          role: user.rol,
          name: user.nombreCompleto,
        );
      }
      return Success(user);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<UserDto>> register({
    required String nombre,
    String? apellido,
    String? telefono,
    required String login,
    required String clave,
    int? rolId,
  }) async {
    try {
      final user = await _remoteDataSource.register(
        nombre: nombre,
        apellido: apellido,
        telefono: telefono,
        login: login,
        clave: clave,
        rolId: rolId,
      );
      if (user.token != null && user.token!.isNotEmpty) {
        await _tokenStorage.saveToken(user.token!);
        await _tokenStorage.saveUserData(
          id: user.id,
          login: user.login,
          role: user.rol,
          name: user.nombreCompleto,
        );
      }
      return Success(user);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<UserDto>> getCurrentUser() async {
    try {
      final user = await _remoteDataSource.getCurrentUser();
      return Success(user);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> logout() async {
    try {
      await _tokenStorage.clearAll();
      return const Success(null);
    } catch (e) {
      return ErrorResult(CacheFailure(message: e.toString()));
    }
  }

  @override
  Future<bool> isAuthenticated() async {
    final token = await _tokenStorage.getToken();
    return token != null && token.isNotEmpty;
  }
}

import '../../../../core/error/failures.dart';
import '../../../data/models/user_dto.dart';
import '../../repositories/auth_repository.dart';

class LoginUseCase {
  final AuthRepository _repository;

  LoginUseCase(this._repository);

  Future<Result<UserDto>> call({
    required String login,
    required String clave,
  }) async {
    return await _repository.login(login: login, clave: clave);
  }
}

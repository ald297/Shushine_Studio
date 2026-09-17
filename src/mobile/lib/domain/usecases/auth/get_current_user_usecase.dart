import '../../../../core/error/failures.dart';
import '../../../data/models/user_dto.dart';
import '../../repositories/auth_repository.dart';

class GetCurrentUserUseCase {
  final AuthRepository _repository;

  GetCurrentUserUseCase(this._repository);

  Future<Result<UserDto>> call() async {
    return await _repository.getCurrentUser();
  }
}

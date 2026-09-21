import '../../../../core/error/failures.dart';
import '../../../data/models/user_dto.dart';
import '../../repositories/auth_repository.dart';

class RegisterUseCase {
  final AuthRepository _repository;

  RegisterUseCase(this._repository);

  Future<Result<UserDto>> call({
    required String nombre,
    String? apellido,
    String? telefono,
    required String login,
    required String clave,
    int? rolId,
  }) async {
    return await _repository.register(
      nombre: nombre,
      apellido: apellido,
      telefono: telefono,
      login: login,
      clave: clave,
      rolId: rolId,
    );
  }
}

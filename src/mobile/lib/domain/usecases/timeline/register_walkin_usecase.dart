import '../../../../core/error/failures.dart';
import '../../../data/models/appointment_dto.dart';
import '../../../data/models/timeline_item_dto.dart';
import '../../repositories/timeline_repository.dart';

class RegisterWalkinUseCase {
  final TimelineRepository _repository;

  RegisterWalkinUseCase(this._repository);

  Future<Result<AppointmentDto>> call(CreateWalkinRequest request) async {
    return await _repository.registerWalkin(request);
  }
}

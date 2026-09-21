import '../../../../core/error/failures.dart';
import '../../../data/models/appointment_dto.dart';
import '../../repositories/appointment_repository.dart';

class CreateAppointmentUseCase {
  final AppointmentRepository _repository;

  CreateAppointmentUseCase(this._repository);

  Future<Result<AppointmentDto>> call(CreateAppointmentRequest request) async {
    return await _repository.createAppointment(request);
  }
}

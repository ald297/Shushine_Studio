import '../../../../core/error/failures.dart';
import '../../../data/models/appointment_dto.dart';
import '../../repositories/appointment_repository.dart';

class GetMyAppointmentsUseCase {
  final AppointmentRepository _repository;

  GetMyAppointmentsUseCase(this._repository);

  Future<Result<List<AppointmentDto>>> call() async {
    return await _repository.getMyAppointments();
  }
}

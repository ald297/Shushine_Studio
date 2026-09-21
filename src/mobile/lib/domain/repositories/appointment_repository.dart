import '../../core/error/failures.dart';
import '../../data/models/appointment_dto.dart';

abstract class AppointmentRepository {
  Future<Result<AppointmentDto>> createAppointment(CreateAppointmentRequest request);
  Future<Result<List<AppointmentDto>>> getMyAppointments();
  Future<Result<AppointmentDto>> getAppointmentById(int id);
}

import '../../../../core/error/failures.dart';
import '../../../data/models/appointment_dto.dart';
import '../../repositories/timeline_repository.dart';

class UpdateAppointmentStatusUseCase {
  final TimelineRepository _repository;

  UpdateAppointmentStatusUseCase(this._repository);

  Future<Result<AppointmentDto>> call({
    required int citaId,
    required String nuevoEstado,
    String? motivoCancelacion,
  }) async {
    return await _repository.updateAppointmentStatus(
      citaId: citaId,
      nuevoEstado: nuevoEstado,
      motivoCancelacion: motivoCancelacion,
    );
  }
}

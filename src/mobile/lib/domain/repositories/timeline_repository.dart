import '../../core/error/failures.dart';
import '../../data/models/timeline_item_dto.dart';
import '../../data/models/appointment_dto.dart';

abstract class TimelineRepository {
  Future<Result<List<TimelineItemDto>>> getTimeline({required String fecha, int? estilistaId});
  Future<Result<AppointmentDto>> updateAppointmentStatus({required int citaId, required String nuevoEstado, String? motivoCancelacion});
  Future<Result<AppointmentDto>> registerWalkin(CreateWalkinRequest request);
}

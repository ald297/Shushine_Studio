import '../../core/error/failures.dart';
import '../../domain/repositories/timeline_repository.dart';
import '../datasources/timeline_remote_datasource.dart';
import '../models/appointment_dto.dart';
import '../models/timeline_item_dto.dart';

class TimelineRepositoryImpl implements TimelineRepository {
  final TimelineRemoteDataSource _remoteDataSource;

  TimelineRepositoryImpl({required TimelineRemoteDataSource remoteDataSource})
      : _remoteDataSource = remoteDataSource;

  @override
  Future<Result<List<TimelineItemDto>>> getTimeline({required String fecha, int? estilistaId}) async {
    try {
      final items = await _remoteDataSource.getTimeline(fecha: fecha, estilistaId: estilistaId);
      return Success(items);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<AppointmentDto>> updateAppointmentStatus({
    required int citaId,
    required String nuevoEstado,
    String? motivoCancelacion,
  }) async {
    try {
      final updated = await _remoteDataSource.updateAppointmentStatus(
        citaId: citaId,
        nuevoEstado: nuevoEstado,
        motivoCancelacion: motivoCancelacion,
      );
      return Success(updated);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<AppointmentDto>> registerWalkin(CreateWalkinRequest request) async {
    try {
      final created = await _remoteDataSource.registerWalkin(request);
      return Success(created);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }
}

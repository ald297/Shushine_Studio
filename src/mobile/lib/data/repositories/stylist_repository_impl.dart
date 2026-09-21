import '../../core/error/failures.dart';
import '../../domain/repositories/stylist_repository.dart';
import '../datasources/stylist_remote_datasource.dart';
import '../models/stylist_dto.dart';

class StylistRepositoryImpl implements StylistRepository {
  final StylistRemoteDataSource _remoteDataSource;

  StylistRepositoryImpl({required StylistRemoteDataSource remoteDataSource})
      : _remoteDataSource = remoteDataSource;

  @override
  Future<Result<List<StylistDto>>> getActiveStylists() async {
    try {
      final stylists = await _remoteDataSource.getActiveStylists();
      return Success(stylists);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<AvailabilityDto>> getStylistAvailability(
    int stylistId,
    String fecha, {
    int? duracionMinutos,
  }) async {
    try {
      final availability = await _remoteDataSource.getStylistAvailability(
        stylistId,
        fecha,
        duracionMinutos: duracionMinutos,
      );
      return Success(availability);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }
}

import '../../core/error/failures.dart';
import '../../data/models/stylist_dto.dart';

abstract class StylistRepository {
  Future<Result<List<StylistDto>>> getActiveStylists();
  Future<Result<AvailabilityDto>> getStylistAvailability(int stylistId, String fecha, {int? duracionMinutos});
}

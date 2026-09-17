import '../../../../core/error/failures.dart';
import '../../../data/models/stylist_dto.dart';
import '../../repositories/stylist_repository.dart';

class GetStylistAvailabilityUseCase {
  final StylistRepository _repository;

  GetStylistAvailabilityUseCase(this._repository);

  Future<Result<AvailabilityDto>> call({
    required int stylistId,
    required String fecha,
    int? duracionMinutos,
  }) async {
    return await _repository.getStylistAvailability(
      stylistId,
      fecha,
      duracionMinutos: duracionMinutos,
    );
  }
}

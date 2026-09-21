import '../../../../core/error/failures.dart';
import '../../../data/models/stylist_dto.dart';
import '../../repositories/stylist_repository.dart';

class GetActiveStylistsUseCase {
  final StylistRepository _repository;

  GetActiveStylistsUseCase(this._repository);

  Future<Result<List<StylistDto>>> call() async {
    return await _repository.getActiveStylists();
  }
}

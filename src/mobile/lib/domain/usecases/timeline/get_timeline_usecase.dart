import '../../../../core/error/failures.dart';
import '../../../data/models/timeline_item_dto.dart';
import '../../repositories/timeline_repository.dart';

class GetTimelineUseCase {
  final TimelineRepository _repository;

  GetTimelineUseCase(this._repository);

  Future<Result<List<TimelineItemDto>>> call({required String fecha, int? estilistaId}) async {
    return await _repository.getTimeline(fecha: fecha, estilistaId: estilistaId);
  }
}

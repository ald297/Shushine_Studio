import '../../../../core/error/failures.dart';
import '../../../data/models/service_dto.dart';
import '../../repositories/catalog_repository.dart';

class GetServiceDetailUseCase {
  final CatalogRepository _repository;

  GetServiceDetailUseCase(this._repository);

  Future<Result<ServiceDto>> call(int id) async {
    return await _repository.getServiceById(id);
  }
}

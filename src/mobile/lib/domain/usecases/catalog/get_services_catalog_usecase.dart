import '../../../../core/error/failures.dart';
import '../../../data/models/category_dto.dart';
import '../../../data/models/service_dto.dart';
import '../../repositories/catalog_repository.dart';

class GetServicesCatalogUseCase {
  final CatalogRepository _repository;

  GetServicesCatalogUseCase(this._repository);

  Future<Result<List<CategoryDto>>> getCategories() async {
    return await _repository.getCategories();
  }

  Future<Result<List<ServiceDto>>> getServices({int? categoryId, String? search}) async {
    if (search != null && search.trim().isNotEmpty) {
      return await _repository.searchServices(search.trim());
    }
    if (categoryId != null) {
      return await _repository.getServicesByCategory(categoryId);
    }
    return await _repository.getAllServices();
  }
}

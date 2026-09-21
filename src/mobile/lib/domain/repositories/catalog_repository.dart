import '../../core/error/failures.dart';
import '../../data/models/category_dto.dart';
import '../../data/models/service_dto.dart';

abstract class CatalogRepository {
  Future<Result<List<CategoryDto>>> getCategories();
  Future<Result<List<ServiceDto>>> getAllServices();
  Future<Result<List<ServiceDto>>> getServicesByCategory(int categoryId);
  Future<Result<List<ServiceDto>>> searchServices(String query);
  Future<Result<ServiceDto>> getServiceById(int id);
}

import '../../core/error/failures.dart';
import '../../domain/repositories/catalog_repository.dart';
import '../datasources/catalog_remote_datasource.dart';
import '../models/category_dto.dart';
import '../models/service_dto.dart';

class CatalogRepositoryImpl implements CatalogRepository {
  final CatalogRemoteDataSource _remoteDataSource;

  CatalogRepositoryImpl({required CatalogRemoteDataSource remoteDataSource})
      : _remoteDataSource = remoteDataSource;

  @override
  Future<Result<List<CategoryDto>>> getCategories() async {
    try {
      final categories = await _remoteDataSource.getCategories();
      return Success(categories);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<List<ServiceDto>>> getAllServices() async {
    try {
      final services = await _remoteDataSource.getAllServices();
      return Success(services);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<List<ServiceDto>>> getServicesByCategory(int categoryId) async {
    try {
      final services = await _remoteDataSource.getServicesByCategory(categoryId);
      return Success(services);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<List<ServiceDto>>> searchServices(String query) async {
    try {
      final services = await _remoteDataSource.searchServices(query);
      return Success(services);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<ServiceDto>> getServiceById(int id) async {
    try {
      final service = await _remoteDataSource.getServiceById(id);
      return Success(service);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }
}

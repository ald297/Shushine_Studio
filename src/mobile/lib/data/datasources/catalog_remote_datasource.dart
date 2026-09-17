import 'package:dio/dio.dart';
import '../../core/error/failures.dart';
import '../../core/network/dio_client.dart';
import '../models/category_dto.dart';
import '../models/service_dto.dart';

abstract class CatalogRemoteDataSource {
  Future<List<CategoryDto>> getCategories();
  Future<List<ServiceDto>> getAllServices();
  Future<List<ServiceDto>> getServicesByCategory(int categoryId);
  Future<List<ServiceDto>> searchServices(String query);
  Future<ServiceDto> getServiceById(int id);
}

class CatalogRemoteDataSourceImpl implements CatalogRemoteDataSource {
  final DioClient _dioClient;

  CatalogRemoteDataSourceImpl({required DioClient dioClient})
      : _dioClient = dioClient;

  @override
  Future<List<CategoryDto>> getCategories() async {
    try {
      final response = await _dioClient.get('/categorias/lista');
      if (response.statusCode == 404 || response.data == null) {
        return [];
      }
      final list = response.data as List<dynamic>;
      return list.map((item) => CategoryDto.fromJson(item as Map<String, dynamic>)).toList();
    } on DioException catch (e) {
      if (e.response?.statusCode == 404) return [];
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al obtener las categorías del catálogo.',
        statusCode: e.response?.statusCode,
      );
    }
  }

  @override
  Future<List<ServiceDto>> getAllServices() async {
    try {
      final response = await _dioClient.get('/servicios/lista');
      if (response.statusCode == 404 || response.data == null) {
        return [];
      }
      final list = response.data as List<dynamic>;
      return list.map((item) => ServiceDto.fromJson(item as Map<String, dynamic>)).toList();
    } on DioException catch (e) {
      if (e.response?.statusCode == 404) return [];
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al obtener los servicios del salón.',
        statusCode: e.response?.statusCode,
      );
    }
  }

  @override
  Future<List<ServiceDto>> getServicesByCategory(int categoryId) async {
    try {
      final response = await _dioClient.get('/servicios/categoria/$categoryId');
      if (response.statusCode == 404 || response.data == null) {
        return [];
      }
      final list = response.data as List<dynamic>;
      return list.map((item) => ServiceDto.fromJson(item as Map<String, dynamic>)).toList();
    } on DioException catch (e) {
      if (e.response?.statusCode == 404) return [];
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al filtrar los servicios por categoría.',
        statusCode: e.response?.statusCode,
      );
    }
  }

  @override
  Future<List<ServiceDto>> searchServices(String query) async {
    try {
      final response = await _dioClient.get(
        '/servicios/buscar',
        queryParameters: {'query': query},
      );
      if (response.statusCode == 404 || response.data == null) {
        return [];
      }
      final list = response.data as List<dynamic>;
      return list.map((item) => ServiceDto.fromJson(item as Map<String, dynamic>)).toList();
    } on DioException catch (e) {
      if (e.response?.statusCode == 404) return [];
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al buscar servicios.',
        statusCode: e.response?.statusCode,
      );
    }
  }

  @override
  Future<ServiceDto> getServiceById(int id) async {
    try {
      final response = await _dioClient.get('/servicios/$id');
      final data = response.data as Map<String, dynamic>;
      return ServiceDto.fromJson(data);
    } on DioException catch (e) {
      if (e.error is Failure) throw e.error as Failure;
      throw ServerFailure(
        message: e.message ?? 'Error al consultar la información del servicio.',
        statusCode: e.response?.statusCode,
      );
    }
  }
}

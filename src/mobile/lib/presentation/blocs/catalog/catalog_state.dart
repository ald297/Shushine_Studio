import 'package:equatable/equatable.dart';
import '../../../data/models/category_dto.dart';
import '../../../data/models/service_dto.dart';

abstract class CatalogState extends Equatable {
  const CatalogState();

  @override
  List<Object?> get props => [];
}

class CatalogInitial extends CatalogState {}

class CatalogLoading extends CatalogState {}

class CatalogLoaded extends CatalogState {
  final List<CategoryDto> categories;
  final List<ServiceDto> services;
  final int? selectedCategoryId;
  final String searchQuery;

  const CatalogLoaded({
    required this.categories,
    required this.services,
    this.selectedCategoryId,
    this.searchQuery = '',
  });

  CatalogLoaded copyWith({
    List<CategoryDto>? categories,
    List<ServiceDto>? services,
    int? selectedCategoryId,
    bool clearCategory = false,
    String? searchQuery,
  }) {
    return CatalogLoaded(
      categories: categories ?? this.categories,
      services: services ?? this.services,
      selectedCategoryId: clearCategory ? null : (selectedCategoryId ?? this.selectedCategoryId),
      searchQuery: searchQuery ?? this.searchQuery,
    );
  }

  @override
  List<Object?> get props => [categories, services, selectedCategoryId, searchQuery];
}

class CatalogError extends CatalogState {
  final String message;

  const CatalogError(this.message);

  @override
  List<Object?> get props => [message];
}

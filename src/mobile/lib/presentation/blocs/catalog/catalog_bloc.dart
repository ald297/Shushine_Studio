import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../data/models/category_dto.dart';
import '../../../domain/usecases/catalog/get_services_catalog_usecase.dart';
import 'catalog_event.dart';
import 'catalog_state.dart';

class CatalogBloc extends Bloc<CatalogEvent, CatalogState> {
  final GetServicesCatalogUseCase _getCatalogUseCase;
  List<CategoryDto> _cachedCategories = [];

  CatalogBloc({required GetServicesCatalogUseCase getCatalogUseCase})
      : _getCatalogUseCase = getCatalogUseCase,
        super(CatalogInitial()) {
    on<CatalogFetchRequested>(_onCatalogFetchRequested);
    on<CatalogCategorySelected>(_onCatalogCategorySelected);
    on<CatalogSearchChanged>(_onCatalogSearchChanged);
  }

  Future<void> _onCatalogFetchRequested(
    CatalogFetchRequested event,
    Emitter<CatalogState> emit,
  ) async {
    emit(CatalogLoading());
    final catResult = await _getCatalogUseCase.getCategories();
    final servResult = await _getCatalogUseCase.getServices();

    List<CategoryDto> categories = [];
    catResult.when(
      onSuccess: (cats) {
        categories = cats;
        _cachedCategories = cats;
      },
      onError: (_) {},
    );

    servResult.when(
      onSuccess: (services) => emit(CatalogLoaded(
        categories: categories,
        services: services,
        selectedCategoryId: null,
      )),
      onError: (failure) => emit(CatalogError(failure.message)),
    );
  }

  Future<void> _onCatalogCategorySelected(
    CatalogCategorySelected event,
    Emitter<CatalogState> emit,
  ) async {
    emit(CatalogLoading());
    final servResult = await _getCatalogUseCase.getServices(categoryId: event.categoryId);

    servResult.when(
      onSuccess: (services) => emit(CatalogLoaded(
        categories: _cachedCategories,
        services: services,
        selectedCategoryId: event.categoryId,
      )),
      onError: (failure) => emit(CatalogError(failure.message)),
    );
  }

  Future<void> _onCatalogSearchChanged(
    CatalogSearchChanged event,
    Emitter<CatalogState> emit,
  ) async {
    emit(CatalogLoading());
    final servResult = await _getCatalogUseCase.getServices(search: event.query);

    servResult.when(
      onSuccess: (services) => emit(CatalogLoaded(
        categories: _cachedCategories,
        services: services,
        selectedCategoryId: null,
        searchQuery: event.query,
      )),
      onError: (failure) => emit(CatalogError(failure.message)),
    );
  }
}

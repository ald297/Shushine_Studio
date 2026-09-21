import 'package:equatable/equatable.dart';

abstract class CatalogEvent extends Equatable {
  const CatalogEvent();

  @override
  List<Object?> get props => [];
}

class CatalogFetchRequested extends CatalogEvent {}

class CatalogCategorySelected extends CatalogEvent {
  final int? categoryId;

  const CatalogCategorySelected(this.categoryId);

  @override
  List<Object?> get props => [categoryId];
}

class CatalogSearchChanged extends CatalogEvent {
  final String query;

  const CatalogSearchChanged(this.query);

  @override
  List<Object?> get props => [query];
}

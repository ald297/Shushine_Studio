import 'package:equatable/equatable.dart';
import '../../../data/models/dashboard_metrics_dto.dart';

abstract class DashboardState extends Equatable {
  const DashboardState();

  @override
  List<Object?> get props => [];
}

class DashboardInitial extends DashboardState {}

class DashboardLoading extends DashboardState {}

class DashboardLoaded extends DashboardState {
  final DashboardMetricsDto metrics;

  const DashboardLoaded(this.metrics);

  @override
  List<Object?> get props => [metrics];
}

class DashboardError extends DashboardState {
  final String message;

  const DashboardError(this.message);

  @override
  List<Object?> get props => [message];
}

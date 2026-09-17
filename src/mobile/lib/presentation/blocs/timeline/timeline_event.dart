import 'package:equatable/equatable.dart';
import '../../../data/models/timeline_item_dto.dart';

abstract class TimelineEvent extends Equatable {
  const TimelineEvent();

  @override
  List<Object?> get props => [];
}

class FetchTimelineRequested extends TimelineEvent {
  final DateTime date;
  final int? stylistId;

  const FetchTimelineRequested({required this.date, this.stylistId});

  @override
  List<Object?> get props => [date, stylistId];
}

class TimelineFilterStylistChanged extends TimelineEvent {
  final int? stylistId;

  const TimelineFilterStylistChanged(this.stylistId);

  @override
  List<Object?> get props => [stylistId];
}

class TimelineDateChanged extends TimelineEvent {
  final DateTime date;

  const TimelineDateChanged(this.date);

  @override
  List<Object?> get props => [date];
}

class UpdateAppointmentStatusRequested extends TimelineEvent {
  final int citaId;
  final String nuevoEstado;
  final String? motivoCancelacion;

  const UpdateAppointmentStatusRequested({
    required this.citaId,
    required this.nuevoEstado,
    this.motivoCancelacion,
  });

  @override
  List<Object?> get props => [citaId, nuevoEstado, motivoCancelacion];
}

class RegisterWalkinRequested extends TimelineEvent {
  final CreateWalkinRequest request;

  const RegisterWalkinRequested(this.request);

  @override
  List<Object?> get props => [request];
}

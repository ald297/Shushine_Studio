import 'package:equatable/equatable.dart';

abstract class AppointmentEvent extends Equatable {
  const AppointmentEvent();

  @override
  List<Object?> get props => [];
}

class FetchMyAppointmentsRequested extends AppointmentEvent {}

class RefreshAppointmentsRequested extends AppointmentEvent {}

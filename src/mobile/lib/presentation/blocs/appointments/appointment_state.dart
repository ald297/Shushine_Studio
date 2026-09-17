import 'package:equatable/equatable.dart';
import '../../../data/models/appointment_dto.dart';

abstract class AppointmentState extends Equatable {
  const AppointmentState();

  @override
  List<Object?> get props => [];
}

class AppointmentInitial extends AppointmentState {}

class AppointmentLoading extends AppointmentState {}

class AppointmentsLoaded extends AppointmentState {
  final List<AppointmentDto> appointments;
  final List<AppointmentDto> activeAppointments;
  final List<AppointmentDto> pastAppointments;

  AppointmentsLoaded({required this.appointments})
      : activeAppointments = appointments.where((a) => a.esActiva).toList(),
        pastAppointments = appointments.where((a) => !a.esActiva).toList();

  @override
  List<Object?> get props => [appointments, activeAppointments, pastAppointments];
}

class AppointmentError extends AppointmentState {
  final String message;

  const AppointmentError(this.message);

  @override
  List<Object?> get props => [message];
}

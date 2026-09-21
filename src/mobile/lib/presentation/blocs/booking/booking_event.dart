import 'package:equatable/equatable.dart';
import '../../../data/models/appointment_dto.dart';
import '../../../data/models/stylist_dto.dart';

abstract class BookingEvent extends Equatable {
  const BookingEvent();

  @override
  List<Object?> get props => [];
}

class BookingStylistsFetchRequested extends BookingEvent {}

class BookingStylistSelected extends BookingEvent {
  final StylistDto stylist;

  const BookingStylistSelected(this.stylist);

  @override
  List<Object?> get props => [stylist];
}

class BookingDateSelected extends BookingEvent {
  final DateTime date;
  final int? duracionMinutos;

  const BookingDateSelected(this.date, {this.duracionMinutos});

  @override
  List<Object?> get props => [date, duracionMinutos];
}

class BookingTimeSlotSelected extends BookingEvent {
  final TimeSlotDto slot;

  const BookingTimeSlotSelected(this.slot);

  @override
  List<Object?> get props => [slot];
}

class BookingCreateSubmitted extends BookingEvent {
  final CreateAppointmentRequest request;

  const BookingCreateSubmitted(this.request);

  @override
  List<Object?> get props => [request];
}

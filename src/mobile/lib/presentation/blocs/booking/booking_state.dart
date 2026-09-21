import 'package:equatable/equatable.dart';
import '../../../data/models/appointment_dto.dart';
import '../../../data/models/stylist_dto.dart';

abstract class BookingState extends Equatable {
  const BookingState();

  @override
  List<Object?> get props => [];
}

class BookingInitial extends BookingState {}

class BookingLoading extends BookingState {}

class BookingSubmitting extends BookingState {}

class BookingAvailabilityLoaded extends BookingState {
  final List<StylistDto> stylists;
  final StylistDto? selectedStylist;
  final DateTime selectedDate;
  final List<TimeSlotDto> slots;
  final TimeSlotDto? selectedSlot;

  const BookingAvailabilityLoaded({
    required this.stylists,
    this.selectedStylist,
    required this.selectedDate,
    required this.slots,
    this.selectedSlot,
  });

  BookingAvailabilityLoaded copyWith({
    List<StylistDto>? stylists,
    StylistDto? selectedStylist,
    DateTime? selectedDate,
    List<TimeSlotDto>? slots,
    TimeSlotDto? selectedSlot,
  }) {
    return BookingAvailabilityLoaded(
      stylists: stylists ?? this.stylists,
      selectedStylist: selectedStylist ?? this.selectedStylist,
      selectedDate: selectedDate ?? this.selectedDate,
      slots: slots ?? this.slots,
      selectedSlot: selectedSlot ?? this.selectedSlot,
    );
  }

  @override
  List<Object?> get props => [stylists, selectedStylist, selectedDate, slots, selectedSlot];
}

class BookingCreationSuccess extends BookingState {
  final AppointmentDto appointment;

  const BookingCreationSuccess(this.appointment);

  @override
  List<Object?> get props => [appointment];
}

class BookingCreationFailure extends BookingState {
  final String message;

  const BookingCreationFailure(this.message);

  @override
  List<Object?> get props => [message];
}

class BookingError extends BookingState {
  final String message;

  const BookingError(this.message);

  @override
  List<Object?> get props => [message];
}

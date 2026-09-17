import 'package:equatable/equatable.dart';
import '../../../data/models/timeline_item_dto.dart';

abstract class TimelineState extends Equatable {
  const TimelineState();

  @override
  List<Object?> get props => [];
}

class TimelineInitial extends TimelineState {}

class TimelineLoading extends TimelineState {}

class TimelineLoaded extends TimelineState {
  final List<TimelineItemDto> items;
  final DateTime selectedDate;
  final int? selectedStylistId;
  final bool isUpdatingStatus;
  final String? successMessage;

  const TimelineLoaded({
    required this.items,
    required this.selectedDate,
    this.selectedStylistId,
    this.isUpdatingStatus = false,
    this.successMessage,
  });

  TimelineLoaded copyWith({
    List<TimelineItemDto>? items,
    DateTime? selectedDate,
    int? selectedStylistId,
    bool? isUpdatingStatus,
    String? successMessage,
    bool clearStylist = false,
  }) {
    return TimelineLoaded(
      items: items ?? this.items,
      selectedDate: selectedDate ?? this.selectedDate,
      selectedStylistId: clearStylist ? null : (selectedStylistId ?? this.selectedStylistId),
      isUpdatingStatus: isUpdatingStatus ?? this.isUpdatingStatus,
      successMessage: successMessage,
    );
  }

  @override
  List<Object?> get props => [items, selectedDate, selectedStylistId, isUpdatingStatus, successMessage];
}

class TimelineError extends TimelineState {
  final String message;

  const TimelineError(this.message);

  @override
  List<Object?> get props => [message];
}

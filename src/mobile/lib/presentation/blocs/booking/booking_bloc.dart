import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:intl/intl.dart';
import '../../../data/models/stylist_dto.dart';
import '../../../domain/usecases/appointments/create_appointment_usecase.dart';
import '../../../domain/usecases/stylists/get_active_stylists_usecase.dart';
import '../../../domain/usecases/stylists/get_stylist_availability_usecase.dart';
import 'booking_event.dart';
import 'booking_state.dart';

class BookingBloc extends Bloc<BookingEvent, BookingState> {
  final GetActiveStylistsUseCase _getActiveStylistsUseCase;
  final GetStylistAvailabilityUseCase _getAvailabilityUseCase;
  final CreateAppointmentUseCase _createAppointmentUseCase;

  List<StylistDto> _cachedStylists = [];
  StylistDto? _currentStylist;
  DateTime _currentDate = DateTime.now();
  int? _duracionMinutos;

  BookingBloc({
    required GetActiveStylistsUseCase getActiveStylistsUseCase,
    required GetStylistAvailabilityUseCase getAvailabilityUseCase,
    required CreateAppointmentUseCase createAppointmentUseCase,
  })  : _getActiveStylistsUseCase = getActiveStylistsUseCase,
        _getAvailabilityUseCase = getAvailabilityUseCase,
        _createAppointmentUseCase = createAppointmentUseCase,
        super(BookingInitial()) {
    on<BookingStylistsFetchRequested>(_onStylistsFetchRequested);
    on<BookingStylistSelected>(_onStylistSelected);
    on<BookingDateSelected>(_onDateSelected);
    on<BookingTimeSlotSelected>(_onTimeSlotSelected);
    on<BookingCreateSubmitted>(_onCreateSubmitted);
  }

  Future<void> _onStylistsFetchRequested(
    BookingStylistsFetchRequested event,
    Emitter<BookingState> emit,
  ) async {
    emit(BookingLoading());
    final result = await _getActiveStylistsUseCase();

    await result.when(
      onSuccess: (stylists) async {
        _cachedStylists = stylists;
        if (stylists.isNotEmpty) {
          _currentStylist = stylists.first;
          await _loadAvailability(emit);
        } else {
          emit(BookingAvailabilityLoaded(
            stylists: const [],
            selectedDate: _currentDate,
            slots: const [],
          ));
        }
      },
      onError: (failure) async => emit(BookingError(failure.message)),
    );
  }

  Future<void> _onStylistSelected(
    BookingStylistSelected event,
    Emitter<BookingState> emit,
  ) async {
    _currentStylist = event.stylist;
    emit(BookingLoading());
    await _loadAvailability(emit);
  }

  Future<void> _onDateSelected(
    BookingDateSelected event,
    Emitter<BookingState> emit,
  ) async {
    _currentDate = event.date;
    if (event.duracionMinutos != null) {
      _duracionMinutos = event.duracionMinutos;
    }
    emit(BookingLoading());
    await _loadAvailability(emit);
  }

  void _onTimeSlotSelected(
    BookingTimeSlotSelected event,
    Emitter<BookingState> emit,
  ) {
    if (state is BookingAvailabilityLoaded) {
      final current = state as BookingAvailabilityLoaded;
      emit(current.copyWith(selectedSlot: event.slot));
    }
  }

  Future<void> _onCreateSubmitted(
    BookingCreateSubmitted event,
    Emitter<BookingState> emit,
  ) async {
    emit(BookingSubmitting());
    final result = await _createAppointmentUseCase(event.request);
    result.when(
      onSuccess: (appointment) => emit(BookingCreationSuccess(appointment)),
      onError: (failure) => emit(BookingCreationFailure(failure.message)),
    );
  }

  Future<void> _loadAvailability(Emitter<BookingState> emit) async {
    if (_currentStylist == null) {
      emit(BookingAvailabilityLoaded(
        stylists: _cachedStylists,
        selectedDate: _currentDate,
        slots: const [],
      ));
      return;
    }

    final dateStr = DateFormat('yyyy-MM-dd').format(_currentDate);
    final result = await _getAvailabilityUseCase(
      stylistId: _currentStylist!.id,
      fecha: dateStr,
      duracionMinutos: _duracionMinutos,
    );

    result.when(
      onSuccess: (availability) => emit(BookingAvailabilityLoaded(
        stylists: _cachedStylists,
        selectedStylist: _currentStylist,
        selectedDate: _currentDate,
        slots: availability.franjas,
        selectedSlot: null,
      )),
      onError: (failure) => emit(BookingError(failure.message)),
    );
  }
}

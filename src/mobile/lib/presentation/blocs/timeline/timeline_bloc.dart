import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:intl/intl.dart';
import '../../../domain/usecases/timeline/get_timeline_usecase.dart';
import '../../../domain/usecases/timeline/register_walkin_usecase.dart';
import '../../../domain/usecases/timeline/update_appointment_status_usecase.dart';
import 'timeline_event.dart';
import 'timeline_state.dart';

class TimelineBloc extends Bloc<TimelineEvent, TimelineState> {
  final GetTimelineUseCase _getTimelineUseCase;
  final UpdateAppointmentStatusUseCase _updateStatusUseCase;
  final RegisterWalkinUseCase _registerWalkinUseCase;

  DateTime _currentDate = DateTime.now();
  int? _currentStylistId;

  TimelineBloc({
    required GetTimelineUseCase getTimelineUseCase,
    required UpdateAppointmentStatusUseCase updateStatusUseCase,
    required RegisterWalkinUseCase registerWalkinUseCase,
  })  : _getTimelineUseCase = getTimelineUseCase,
        _updateStatusUseCase = updateStatusUseCase,
        _registerWalkinUseCase = registerWalkinUseCase,
        super(TimelineInitial()) {
    on<FetchTimelineRequested>(_onFetchTimeline);
    on<TimelineFilterStylistChanged>(_onFilterStylistChanged);
    on<TimelineDateChanged>(_onDateChanged);
    on<UpdateAppointmentStatusRequested>(_onUpdateStatus);
    on<RegisterWalkinRequested>(_onRegisterWalkin);
  }

  Future<void> _onFetchTimeline(
    FetchTimelineRequested event,
    Emitter<TimelineState> emit,
  ) async {
    _currentDate = event.date;
    _currentStylistId = event.stylistId;
    emit(TimelineLoading());
    await _loadTimeline(emit);
  }

  Future<void> _onFilterStylistChanged(
    TimelineFilterStylistChanged event,
    Emitter<TimelineState> emit,
  ) async {
    _currentStylistId = event.stylistId;
    emit(TimelineLoading());
    await _loadTimeline(emit);
  }

  Future<void> _onDateChanged(
    TimelineDateChanged event,
    Emitter<TimelineState> emit,
  ) async {
    _currentDate = event.date;
    emit(TimelineLoading());
    await _loadTimeline(emit);
  }

  Future<void> _onUpdateStatus(
    UpdateAppointmentStatusRequested event,
    Emitter<TimelineState> emit,
  ) async {
    final result = await _updateStatusUseCase(
      citaId: event.citaId,
      nuevoEstado: event.nuevoEstado,
      motivoCancelacion: event.motivoCancelacion,
    );

    await result.when(
      onSuccess: (updated) async {
        await _loadTimeline(emit, message: 'Estado actualizado a ${updated.estado}');
      },
      onError: (failure) async => emit(TimelineError(failure.message)),
    );
  }

  Future<void> _onRegisterWalkin(
    RegisterWalkinRequested event,
    Emitter<TimelineState> emit,
  ) async {
    emit(TimelineLoading());
    final result = await _registerWalkinUseCase(event.request);
    await result.when(
      onSuccess: (appointment) async {
        await _loadTimeline(emit, message: 'Cita walk-in #${appointment.codigoCita} registrada con éxito');
      },
      onError: (failure) async => emit(TimelineError(failure.message)),
    );
  }

  Future<void> _loadTimeline(Emitter<TimelineState> emit, {String? message}) async {
    final dateStr = DateFormat('yyyy-MM-dd').format(_currentDate);
    final result = await _getTimelineUseCase(
      fecha: dateStr,
      estilistaId: _currentStylistId,
    );

    result.when(
      onSuccess: (items) => emit(TimelineLoaded(
        items: items,
        selectedDate: _currentDate,
        selectedStylistId: _currentStylistId,
        successMessage: message,
      )),
      onError: (failure) => emit(TimelineError(failure.message)),
    );
  }
}

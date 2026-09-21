import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../domain/usecases/appointments/get_my_appointments_usecase.dart';
import 'appointment_event.dart';
import 'appointment_state.dart';

class AppointmentBloc extends Bloc<AppointmentEvent, AppointmentState> {
  final GetMyAppointmentsUseCase _getMyAppointmentsUseCase;

  AppointmentBloc({required GetMyAppointmentsUseCase getMyAppointmentsUseCase})
      : _getMyAppointmentsUseCase = getMyAppointmentsUseCase,
        super(AppointmentInitial()) {
    on<FetchMyAppointmentsRequested>(_onFetchMyAppointments);
    on<RefreshAppointmentsRequested>(_onRefreshAppointments);
  }

  Future<void> _onFetchMyAppointments(
    FetchMyAppointmentsRequested event,
    Emitter<AppointmentState> emit,
  ) async {
    emit(AppointmentLoading());
    final result = await _getMyAppointmentsUseCase();
    result.when(
      onSuccess: (appointments) => emit(AppointmentsLoaded(appointments: appointments)),
      onError: (failure) => emit(AppointmentError(failure.message)),
    );
  }

  Future<void> _onRefreshAppointments(
    RefreshAppointmentsRequested event,
    Emitter<AppointmentState> emit,
  ) async {
    final result = await _getMyAppointmentsUseCase();
    result.when(
      onSuccess: (appointments) => emit(AppointmentsLoaded(appointments: appointments)),
      onError: (failure) => emit(AppointmentError(failure.message)),
    );
  }
}

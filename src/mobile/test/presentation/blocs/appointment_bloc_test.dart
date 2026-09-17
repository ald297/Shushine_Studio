import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:shushine_studio_mobile/core/error/failures.dart';
import 'package:shushine_studio_mobile/data/models/appointment_dto.dart';
import 'package:shushine_studio_mobile/domain/usecases/appointments/get_my_appointments_usecase.dart';
import 'package:shushine_studio_mobile/presentation/blocs/appointments/appointment_bloc.dart';
import 'package:shushine_studio_mobile/presentation/blocs/appointments/appointment_event.dart';
import 'package:shushine_studio_mobile/presentation/blocs/appointments/appointment_state.dart';

class MockGetMyAppointmentsUseCase extends Mock implements GetMyAppointmentsUseCase {}

void main() {
  late MockGetMyAppointmentsUseCase mockGetMyAppointmentsUseCase;
  late AppointmentBloc appointmentBloc;

  const tAppointment = AppointmentDto(
    id: 1,
    codigoCita: 'SHU-2026-1001',
    estilistaId: 1,
    estilistaNombre: 'Valeria Rivas',
    fechaCita: '2026-09-20',
    horaInicio: '10:00:00',
    horaFin: '11:00:00',
    estado: 'Confirmed',
    subtotal: 50.0,
    iva: 6.5,
    total: 56.5,
  );

  setUp(() {
    mockGetMyAppointmentsUseCase = MockGetMyAppointmentsUseCase();
    appointmentBloc = AppointmentBloc(
      getMyAppointmentsUseCase: mockGetMyAppointmentsUseCase,
    );
  });

  tearDown(() {
    appointmentBloc.close();
  });

  test('initial state should be AppointmentInitial', () {
    expect(appointmentBloc.state, equals(AppointmentInitial()));
  });

  group('FetchMyAppointmentsRequested', () {
    blocTest<AppointmentBloc, AppointmentState>(
      'emits [AppointmentLoading, AppointmentsLoaded] when appointments fetch succeeds',
      build: () {
        when(() => mockGetMyAppointmentsUseCase())
            .thenAnswer((_) async => const Success([tAppointment]));
        return appointmentBloc;
      },
      act: (bloc) => bloc.add(FetchMyAppointmentsRequested()),
      expect: () => [
        AppointmentLoading(),
        AppointmentsLoaded(appointments: const [tAppointment]),
      ],
      verify: (_) {
        verify(() => mockGetMyAppointmentsUseCase()).called(1);
      },
    );

    blocTest<AppointmentBloc, AppointmentState>(
      'emits [AppointmentLoading, AppointmentError] when appointments fetch fails',
      build: () {
        when(() => mockGetMyAppointmentsUseCase())
            .thenAnswer((_) async => const ErrorResult(ServerFailure(message: 'Error de servidor')));
        return appointmentBloc;
      },
      act: (bloc) => bloc.add(FetchMyAppointmentsRequested()),
      expect: () => [
        AppointmentLoading(),
        const AppointmentError('Error de servidor'),
      ],
    );
  });
}

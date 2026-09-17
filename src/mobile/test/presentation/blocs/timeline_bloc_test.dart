import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:shushine_studio_mobile/core/error/failures.dart';
import 'package:shushine_studio_mobile/data/models/appointment_dto.dart';
import 'package:shushine_studio_mobile/data/models/timeline_item_dto.dart';
import 'package:shushine_studio_mobile/domain/usecases/timeline/get_timeline_usecase.dart';
import 'package:shushine_studio_mobile/domain/usecases/timeline/register_walkin_usecase.dart';
import 'package:shushine_studio_mobile/domain/usecases/timeline/update_appointment_status_usecase.dart';
import 'package:shushine_studio_mobile/presentation/blocs/timeline/timeline_bloc.dart';
import 'package:shushine_studio_mobile/presentation/blocs/timeline/timeline_event.dart';
import 'package:shushine_studio_mobile/presentation/blocs/timeline/timeline_state.dart';

class MockGetTimelineUseCase extends Mock implements GetTimelineUseCase {}
class MockUpdateAppointmentStatusUseCase extends Mock implements UpdateAppointmentStatusUseCase {}
class MockRegisterWalkinUseCase extends Mock implements RegisterWalkinUseCase {}

void main() {
  late MockGetTimelineUseCase mockGetTimelineUseCase;
  late MockUpdateAppointmentStatusUseCase mockUpdateStatusUseCase;
  late MockRegisterWalkinUseCase mockRegisterWalkinUseCase;
  late TimelineBloc timelineBloc;

  final tDate = DateTime(2026, 9, 20);
  const tTimelineItem = TimelineItemDto(
    citaId: 10,
    codigoCita: 'SHU-2026-1001',
    clienteNombre: 'Camila Calderón',
    fechaCita: '2026-09-20',
    horaInicio: '09:00:00',
    horaFin: '10:00:00',
    estado: 'Confirmed',
    total: 25.0,
  );

  const tAppointment = AppointmentDto(
    id: 10,
    codigoCita: 'SHU-2026-1001',
    estilistaId: 1,
    fechaCita: '2026-09-20',
    horaInicio: '09:00:00',
    horaFin: '10:00:00',
    estado: 'InProgress',
    subtotal: 25.0,
    iva: 3.25,
    total: 28.25,
  );

  setUp(() {
    mockGetTimelineUseCase = MockGetTimelineUseCase();
    mockUpdateStatusUseCase = MockUpdateAppointmentStatusUseCase();
    mockRegisterWalkinUseCase = MockRegisterWalkinUseCase();

    timelineBloc = TimelineBloc(
      getTimelineUseCase: mockGetTimelineUseCase,
      updateStatusUseCase: mockUpdateStatusUseCase,
      registerWalkinUseCase: mockRegisterWalkinUseCase,
    );
  });

  tearDown(() {
    timelineBloc.close();
  });

  test('initial state should be TimelineInitial', () {
    expect(timelineBloc.state, equals(TimelineInitial()));
  });

  group('FetchTimelineRequested', () {
    blocTest<TimelineBloc, TimelineState>(
      'emits [TimelineLoading, TimelineLoaded] when fetch succeeds',
      build: () {
        when(() => mockGetTimelineUseCase(fecha: '2026-09-20', estilistaId: null))
            .thenAnswer((_) async => const Success([tTimelineItem]));
        return timelineBloc;
      },
      act: (bloc) => bloc.add(FetchTimelineRequested(date: tDate)),
      expect: () => [
        TimelineLoading(),
        TimelineLoaded(
          items: const [tTimelineItem],
          selectedDate: tDate,
        ),
      ],
    );

    blocTest<TimelineBloc, TimelineState>(
      'emits [TimelineLoading, TimelineError] when fetch fails',
      build: () {
        when(() => mockGetTimelineUseCase(fecha: '2026-09-20', estilistaId: null))
            .thenAnswer((_) async => const ErrorResult(ServerFailure(message: 'Error al cargar agenda')));
        return timelineBloc;
      },
      act: (bloc) => bloc.add(FetchTimelineRequested(date: tDate)),
      expect: () => [
        TimelineLoading(),
        const TimelineError('Error al cargar agenda'),
      ],
    );
  });

  group('UpdateAppointmentStatusRequested', () {
    blocTest<TimelineBloc, TimelineState>(
      'updates appointment status and reloads timeline with success message',
      build: () {
        when(() => mockUpdateStatusUseCase(
              citaId: 10,
              nuevoEstado: 'InProgress',
              motivoCancelacion: null,
            )).thenAnswer((_) async => const Success(tAppointment));
        when(() => mockGetTimelineUseCase(fecha: any(named: 'fecha'), estilistaId: any(named: 'estilistaId')))
            .thenAnswer((_) async => const Success([tTimelineItem]));
        return timelineBloc;
      },
      act: (bloc) => bloc.add(const UpdateAppointmentStatusRequested(
        citaId: 10,
        nuevoEstado: 'InProgress',
      )),
      expect: () => [
        isA<TimelineLoaded>()
            .having((s) => s.successMessage, 'successMessage', contains('InProgress')),
      ],
    );
  });
}

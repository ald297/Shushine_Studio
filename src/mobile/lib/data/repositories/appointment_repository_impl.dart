import '../../core/error/failures.dart';
import '../../domain/repositories/appointment_repository.dart';
import '../datasources/appointment_remote_datasource.dart';
import '../models/appointment_dto.dart';

class AppointmentRepositoryImpl implements AppointmentRepository {
  final AppointmentRemoteDataSource _remoteDataSource;

  AppointmentRepositoryImpl({required AppointmentRemoteDataSource remoteDataSource})
      : _remoteDataSource = remoteDataSource;

  @override
  Future<Result<AppointmentDto>> createAppointment(CreateAppointmentRequest request) async {
    try {
      final appointment = await _remoteDataSource.createAppointment(request);
      return Success(appointment);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<List<AppointmentDto>>> getMyAppointments() async {
    try {
      final appointments = await _remoteDataSource.getMyAppointments();
      return Success(appointments);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Result<AppointmentDto>> getAppointmentById(int id) async {
    try {
      final appointment = await _remoteDataSource.getAppointmentById(id);
      return Success(appointment);
    } on Failure catch (f) {
      return ErrorResult(f);
    } catch (e) {
      return ErrorResult(ServerFailure(message: e.toString()));
    }
  }
}

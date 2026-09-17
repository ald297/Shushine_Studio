import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:shushine_studio_mobile/core/error/failures.dart';
import 'package:shushine_studio_mobile/data/models/user_dto.dart';
import 'package:shushine_studio_mobile/domain/usecases/auth/get_current_user_usecase.dart';
import 'package:shushine_studio_mobile/domain/usecases/auth/login_usecase.dart';
import 'package:shushine_studio_mobile/domain/usecases/auth/logout_usecase.dart';
import 'package:shushine_studio_mobile/domain/usecases/auth/register_usecase.dart';
import 'package:shushine_studio_mobile/presentation/blocs/auth/auth_bloc.dart';
import 'package:shushine_studio_mobile/presentation/blocs/auth/auth_event.dart';
import 'package:shushine_studio_mobile/presentation/blocs/auth/auth_state.dart';

class MockLoginUseCase extends Mock implements LoginUseCase {}
class MockRegisterUseCase extends Mock implements RegisterUseCase {}
class MockGetCurrentUserUseCase extends Mock implements GetCurrentUserUseCase {}
class MockLogoutUseCase extends Mock implements LogoutUseCase {}

void main() {
  late MockLoginUseCase mockLoginUseCase;
  late MockRegisterUseCase mockRegisterUseCase;
  late MockGetCurrentUserUseCase mockGetCurrentUserUseCase;
  late MockLogoutUseCase mockLogoutUseCase;
  late AuthBloc authBloc;

  const tUser = UserDto(
    id: 1,
    login: 'cliente',
    nombre: 'Camila',
    apellido: 'Calderon',
    telefono: '70003344',
    rol: 'CLIENTE',
    activo: true,
    token: 'fake_jwt_token',
  );

  setUp(() {
    mockLoginUseCase = MockLoginUseCase();
    mockRegisterUseCase = MockRegisterUseCase();
    mockGetCurrentUserUseCase = MockGetCurrentUserUseCase();
    mockLogoutUseCase = MockLogoutUseCase();

    authBloc = AuthBloc(
      loginUseCase: mockLoginUseCase,
      registerUseCase: mockRegisterUseCase,
      getCurrentUserUseCase: mockGetCurrentUserUseCase,
      logoutUseCase: mockLogoutUseCase,
    );
  });

  tearDown(() {
    authBloc.close();
  });

  test('initial state should be AuthInitial', () {
    expect(authBloc.state, equals(AuthInitial()));
  });

  group('AuthLoginRequested', () {
    blocTest<AuthBloc, AuthState>(
      'emits [AuthLoading, Authenticated] when login is successful',
      build: () {
        when(() => mockLoginUseCase(login: 'cliente', clave: 'cliente123'))
            .thenAnswer((_) async => const Success(tUser));
        return authBloc;
      },
      act: (bloc) => bloc.add(const AuthLoginRequested(
        login: 'cliente',
        clave: 'cliente123',
      )),
      expect: () => [
        AuthLoading(),
        const Authenticated(tUser),
      ],
    );

    blocTest<AuthBloc, AuthState>(
      'emits [AuthLoading, AuthError] when login fails',
      build: () {
        when(() => mockLoginUseCase(login: 'cliente', clave: 'wrong_pass'))
            .thenAnswer((_) async => const ErrorResult(
                  AuthFailure(message: 'Credenciales inválidas', statusCode: 401),
                ));
        return authBloc;
      },
      act: (bloc) => bloc.add(const AuthLoginRequested(
        login: 'cliente',
        clave: 'wrong_pass',
      )),
      expect: () => [
        AuthLoading(),
        const AuthError(
          message: 'Credenciales inválidas',
          failure: AuthFailure(message: 'Credenciales inválidas', statusCode: 401),
        ),
      ],
    );
  });

  group('AuthLogoutRequested', () {
    blocTest<AuthBloc, AuthState>(
      'emits [AuthLoading, Unauthenticated] on logout',
      build: () {
        when(() => mockLogoutUseCase())
            .thenAnswer((_) async => const Success(null));
        return authBloc;
      },
      act: (bloc) => bloc.add(AuthLogoutRequested()),
      expect: () => [
        AuthLoading(),
        Unauthenticated(),
      ],
    );
  });
}

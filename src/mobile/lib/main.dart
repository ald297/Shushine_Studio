import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'core/di/injection_container.dart';
import 'presentation/blocs/auth/auth_bloc.dart';
import 'presentation/blocs/auth/auth_event.dart';
import 'presentation/blocs/auth/auth_state.dart';
import 'presentation/screens/auth/login_screen.dart';
import 'presentation/screens/profile/profile_screen.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await initDependencies();
  runApp(const ShushineStudioApp());
}

class ShushineStudioApp extends StatelessWidget {
  const ShushineStudioApp({super.key});

  @override
  Widget build(BuildContext context) {
    const primaryGold = Color(0xFFC5A059);
    const darkBackground = Color(0xFF121214);

    return BlocProvider<AuthBloc>(
      create: (_) => sl<AuthBloc>()..add(AuthCheckRequested()),
      child: MaterialApp(
        title: 'Shushine Studio',
        debugShowCheckedModeBanner: false,
        theme: ThemeData(
          brightness: Brightness.dark,
          scaffoldBackgroundColor: darkBackground,
          colorScheme: const ColorScheme.dark(
            primary: primaryGold,
            surface: Color(0xFF1E1E24),
          ),
          useMaterial3: true,
        ),
        home: BlocBuilder<AuthBloc, AuthState>(
          builder: (context, state) {
            if (state is Authenticated) {
              return ProfileScreen(user: state.user);
            }
            if (state is AuthInitial || state is AuthLoading) {
              return const Scaffold(
                backgroundColor: darkBackground,
                body: Center(
                  child: CircularProgressIndicator(color: primaryGold),
                ),
              );
            }
            return const LoginScreen();
          },
        ),
      ),
    );
  }
}

import 'package:equatable/equatable.dart';

abstract class AuthEvent extends Equatable {
  const AuthEvent();

  @override
  List<Object?> get props => [];
}

class AuthCheckRequested extends AuthEvent {}

class AuthLoginRequested extends AuthEvent {
  final String login;
  final String clave;

  const AuthLoginRequested({required this.login, required this.clave});

  @override
  List<Object?> get props => [login, clave];
}

class AuthRegisterRequested extends AuthEvent {
  final String nombre;
  final String? apellido;
  final String? telefono;
  final String login;
  final String clave;

  const AuthRegisterRequested({
    required this.nombre,
    this.apellido,
    this.telefono,
    required this.login,
    required this.clave,
  });

  @override
  List<Object?> get props => [nombre, apellido, telefono, login, clave];
}

class AuthLogoutRequested extends AuthEvent {}

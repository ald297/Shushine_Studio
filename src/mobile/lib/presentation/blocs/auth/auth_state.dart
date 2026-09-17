import 'package:equatable/equatable.dart';
import '../../../core/error/failures.dart';
import '../../../data/models/user_dto.dart';

abstract class AuthState extends Equatable {
  const AuthState();

  @override
  List<Object?> get props => [];
}

class AuthInitial extends AuthState {}

class AuthLoading extends AuthState {}

class Authenticated extends AuthState {
  final UserDto user;

  const Authenticated(this.user);

  @override
  List<Object?> get props => [user];
}

class Unauthenticated extends AuthState {}

class AuthError extends AuthState {
  final String message;
  final Failure? failure;

  const AuthError({required this.message, this.failure});

  @override
  List<Object?> get props => [message, failure];
}

import 'package:equatable/equatable.dart';

class UserDto extends Equatable {
  final int id;
  final String login;
  final String nombre;
  final String? apellido;
  final String? telefono;
  final String rol;
  final bool activo;
  final String? token;

  const UserDto({
    required this.id,
    required this.login,
    required this.nombre,
    this.apellido,
    this.telefono,
    required this.rol,
    this.activo = true,
    this.token,
  });

  String get nombreCompleto {
    if (apellido != null && apellido!.isNotEmpty) {
      return '$nombre $apellido';
    }
    return nombre;
  }

  bool get isAdmin => rol.toUpperCase() == 'ADMIN';
  bool get isCliente => rol.toUpperCase() == 'CLIENTE';

  factory UserDto.fromJson(Map<String, dynamic> json, {String? token}) {
    return UserDto(
      id: json['id'] as int? ?? 0,
      login: json['login']?.toString() ?? '',
      nombre: json['nombre']?.toString() ?? '',
      apellido: json['apellido']?.toString(),
      telefono: json['telefono']?.toString(),
      rol: json['rol']?.toString() ?? 'CLIENTE',
      activo: json['activo'] as bool? ?? true,
      token: token ?? json['token']?.toString(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'login': login,
      'nombre': nombre,
      'apellido': apellido,
      'telefono': telefono,
      'rol': rol,
      'activo': activo,
      if (token != null) 'token': token,
    };
  }

  @override
  List<Object?> get props => [id, login, nombre, apellido, telefono, rol, activo, token];
}

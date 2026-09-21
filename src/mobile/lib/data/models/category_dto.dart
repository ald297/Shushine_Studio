import 'package:equatable/equatable.dart';

class CategoryDto extends Equatable {
  final int id;
  final String nombre;
  final String? descripcion;
  final String? iconoUrl;
  final String tipo;
  final bool activo;

  const CategoryDto({
    required this.id,
    required this.nombre,
    this.descripcion,
    this.iconoUrl,
    this.tipo = 'Servicio',
    this.activo = true,
  });

  factory CategoryDto.fromJson(Map<String, dynamic> json) {
    return CategoryDto(
      id: json['id'] as int? ?? 0,
      nombre: json['nombre']?.toString() ?? '',
      descripcion: json['descripcion']?.toString(),
      iconoUrl: json['iconoUrl']?.toString(),
      tipo: json['tipo']?.toString() ?? 'Servicio',
      activo: json['activo'] as bool? ?? true,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'nombre': nombre,
      'descripcion': descripcion,
      'iconoUrl': iconoUrl,
      'tipo': tipo,
      'activo': activo,
    };
  }

  @override
  List<Object?> get props => [id, nombre, descripcion, iconoUrl, tipo, activo];
}

import 'package:equatable/equatable.dart';

class ServiceDto extends Equatable {
  final int id;
  final String codigoServicio;
  final int? categoriaId;
  final String? categoriaNombre;
  final String nombre;
  final String descripcion;
  final double precioBase;
  final bool esPrecioVariable;
  final int duracionMinutos;
  final int intervaloSeguimientoDias;
  final String? imagenUrl;
  final double? costoInsumos;
  final bool activo;

  const ServiceDto({
    required this.id,
    required this.codigoServicio,
    this.categoriaId,
    this.categoriaNombre,
    required this.nombre,
    required this.descripcion,
    required this.precioBase,
    this.esPrecioVariable = false,
    required this.duracionMinutos,
    this.intervaloSeguimientoDias = 21,
    this.imagenUrl,
    this.costoInsumos,
    this.activo = true,
  });

  String get precioFormateado {
    final precioStr = '\$${precioBase.toStringAsFixed(2)}';
    return esPrecioVariable ? 'Desde $precioStr' : precioStr;
  }

  String get duracionFormateada {
    if (duracionMinutos >= 60) {
      final horas = duracionMinutos ~/ 60;
      final mins = duracionMinutos % 60;
      if (mins == 0) {
        return '$horas h';
      }
      return '$horas h $mins min';
    }
    return '$duracionMinutos min';
  }

  factory ServiceDto.fromJson(Map<String, dynamic> json) {
    return ServiceDto(
      id: json['id'] as int? ?? 0,
      codigoServicio: json['codigoServicio']?.toString() ?? '',
      categoriaId: json['categoriaId'] as int?,
      categoriaNombre: json['categoriaNombre']?.toString(),
      nombre: json['nombre']?.toString() ?? '',
      descripcion: json['descripcion']?.toString() ?? '',
      precioBase: (json['precioBase'] as num?)?.toDouble() ?? 0.0,
      esPrecioVariable: json['esPrecioVariable'] as bool? ?? false,
      duracionMinutos: json['duracionMinutos'] as int? ?? 30,
      intervaloSeguimientoDias: json['intervaloSeguimientoDias'] as int? ?? 21,
      imagenUrl: json['imagenUrl']?.toString(),
      costoInsumos: (json['costoInsumos'] as num?)?.toDouble(),
      activo: json['activo'] as bool? ?? true,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'codigoServicio': codigoServicio,
      'categoriaId': categoriaId,
      'categoriaNombre': categoriaNombre,
      'nombre': nombre,
      'descripcion': descripcion,
      'precioBase': precioBase,
      'esPrecioVariable': esPrecioVariable,
      'duracionMinutos': duracionMinutos,
      'intervaloSeguimientoDias': intervaloSeguimientoDias,
      'imagenUrl': imagenUrl,
      'costoInsumos': costoInsumos,
      'activo': activo,
    };
  }

  @override
  List<Object?> get props => [
        id,
        codigoServicio,
        categoriaId,
        categoriaNombre,
        nombre,
        descripcion,
        precioBase,
        esPrecioVariable,
        duracionMinutos,
        intervaloSeguimientoDias,
        imagenUrl,
        activo,
      ];
}

import 'package:equatable/equatable.dart';

class TimeSlotDto extends Equatable {
  final String horaInicio;
  final String horaFin;
  final bool disponible;
  final String? motivoNoDisponible;

  const TimeSlotDto({
    required this.horaInicio,
    required this.horaFin,
    required this.disponible,
    this.motivoNoDisponible,
  });

  String get etiquetaSlot => '$horaInicio - $horaFin';

  factory TimeSlotDto.fromJson(Map<String, dynamic> json) {
    return TimeSlotDto(
      horaInicio: json['horaInicio']?.toString() ?? '',
      horaFin: json['horaFin']?.toString() ?? '',
      disponible: json['disponible'] as bool? ?? false,
      motivoNoDisponible: json['motivoNoDisponible']?.toString(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'horaInicio': horaInicio,
      'horaFin': horaFin,
      'disponible': disponible,
      'motivoNoDisponible': motivoNoDisponible,
    };
  }

  @override
  List<Object?> get props => [horaInicio, horaFin, disponible, motivoNoDisponible];
}

class StylistDto extends Equatable {
  final int id;
  final String nombreCompleto;
  final String especialidadPrincipal;
  final String? biografia;
  final String? avatarUrl;
  final String colorAgenda;
  final bool activo;

  const StylistDto({
    required this.id,
    required this.nombreCompleto,
    required this.especialidadPrincipal,
    this.biografia,
    this.avatarUrl,
    this.colorAgenda = '#C5A059',
    this.activo = true,
  });

  factory StylistDto.fromJson(Map<String, dynamic> json) {
    return StylistDto(
      id: json['id'] as int? ?? 0,
      nombreCompleto: json['nombreCompleto']?.toString() ?? '',
      especialidadPrincipal: json['especialidadPrincipal']?.toString() ?? 'Estilista',
      biografia: json['biografia']?.toString(),
      avatarUrl: json['avatarUrl']?.toString(),
      colorAgenda: json['colorAgenda']?.toString() ?? '#C5A059',
      activo: json['activo'] as bool? ?? true,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'nombreCompleto': nombreCompleto,
      'especialidadPrincipal': especialidadPrincipal,
      'biografia': biografia,
      'avatarUrl': avatarUrl,
      'colorAgenda': colorAgenda,
      'activo': activo,
    };
  }

  @override
  List<Object?> get props => [id, nombreCompleto, especialidadPrincipal, biografia, avatarUrl, colorAgenda, activo];
}

class AvailabilityDto extends Equatable {
  final int estilistaId;
  final String estilistaNombre;
  final String fecha;
  final List<TimeSlotDto> franjas;

  const AvailabilityDto({
    required this.estilistaId,
    required this.estilistaNombre,
    required this.fecha,
    required this.franjas,
  });

  factory AvailabilityDto.fromJson(Map<String, dynamic> json) {
    final franjasList = (json['franjas'] as List<dynamic>? ?? [])
        .map((e) => TimeSlotDto.fromJson(e as Map<String, dynamic>))
        .toList();

    return AvailabilityDto(
      estilistaId: json['estilistaId'] as int? ?? 0,
      estilistaNombre: json['estilistaNombre']?.toString() ?? '',
      fecha: json['fecha']?.toString() ?? '',
      franjas: franjasList,
    );
  }

  @override
  List<Object?> get props => [estilistaId, estilistaNombre, fecha, franjas];
}

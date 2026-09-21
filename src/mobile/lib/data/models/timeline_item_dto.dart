import 'package:equatable/equatable.dart';

class TimelineItemDto extends Equatable {
  final int citaId;
  final String codigoCita;
  final int? clienteId;
  final String clienteNombre;
  final String? clienteTelefono;
  final int? estilistaId;
  final String? estilistaNombre;
  final String fechaCita;
  final String horaInicio;
  final String horaFin;
  final String estado;
  final double total;
  final bool esWalkin;
  final String? notas;
  final List<String> servicios;

  const TimelineItemDto({
    required this.citaId,
    required this.codigoCita,
    this.clienteId,
    required this.clienteNombre,
    this.clienteTelefono,
    this.estilistaId,
    this.estilistaNombre,
    required this.fechaCita,
    required this.horaInicio,
    required this.horaFin,
    required this.estado,
    required this.total,
    this.esWalkin = false,
    this.notas,
    this.servicios = const [],
  });

  factory TimelineItemDto.fromJson(Map<String, dynamic> json) {
    return TimelineItemDto(
      citaId: json['citaId'] as int? ?? 0,
      codigoCita: json['codigoCita']?.toString() ?? '',
      clienteId: json['clienteId'] as int?,
      clienteNombre: json['clienteNombre']?.toString() ?? 'Cliente',
      clienteTelefono: json['clienteTelefono']?.toString(),
      estilistaId: json['estilistaId'] as int?,
      estilistaNombre: json['estilistaNombre']?.toString(),
      fechaCita: json['fechaCita']?.toString() ?? '',
      horaInicio: json['horaInicio']?.toString() ?? '',
      horaFin: json['horaFin']?.toString() ?? '',
      estado: json['estado']?.toString() ?? 'Confirmed',
      total: (json['total'] as num?)?.toDouble() ?? 0.0,
      esWalkin: json['esWalkin'] as bool? ?? false,
      notas: json['notas']?.toString(),
      servicios: (json['servicios'] as List<dynamic>?)
              ?.map((e) => e.toString())
              .toList() ??
          const [],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'citaId': citaId,
      'codigoCita': codigoCita,
      'clienteId': clienteId,
      'clienteNombre': clienteNombre,
      'clienteTelefono': clienteTelefono,
      'estilistaId': estilistaId,
      'estilistaNombre': estilistaNombre,
      'fechaCita': fechaCita,
      'horaInicio': horaInicio,
      'horaFin': horaFin,
      'estado': estado,
      'total': total,
      'esWalkin': esWalkin,
      'notas': notas,
      'servicios': servicios,
    };
  }

  @override
  List<Object?> get props => [
        citaId,
        codigoCita,
        clienteNombre,
        estilistaId,
        fechaCita,
        horaInicio,
        horaFin,
        estado,
      ];
}

class CreateWalkinRequest extends Equatable {
  final String nombreCliente;
  final String? telefonoCliente;
  final int estilistaId;
  final String fechaCita;
  final String horaInicio;
  final List<int> servicioIds;
  final String metodoPagoPreferente;
  final String? notas;

  const CreateWalkinRequest({
    required this.nombreCliente,
    this.telefonoCliente,
    required this.estilistaId,
    required this.fechaCita,
    required this.horaInicio,
    required this.servicioIds,
    this.metodoPagoPreferente = 'Efectivo',
    this.notas,
  });

  Map<String, dynamic> toJson() {
    return {
      'nombreCliente': nombreCliente,
      'telefonoCliente': telefonoCliente,
      'estilistaId': estilistaId,
      'fechaCita': fechaCita,
      'horaInicio': horaInicio.length == 5 ? '$horaInicio:00' : horaInicio,
      'servicioIds': servicioIds,
      'metodoPagoPreferente': metodoPagoPreferente,
      'notas': notas,
    };
  }

  @override
  List<Object?> get props => [
        nombreCliente,
        telefonoCliente,
        estilistaId,
        fechaCita,
        horaInicio,
        servicioIds,
      ];
}

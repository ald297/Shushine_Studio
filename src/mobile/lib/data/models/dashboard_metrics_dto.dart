import 'package:equatable/equatable.dart';

class DashboardMetricsDto extends Equatable {
  final int totalCitasHoy;
  final int totalCitasSemana;
  final double ingresosHoy;
  final double ingresosMes;
  final int citasEnProceso;
  final int citasCompletadas;
  final int citasCanceladas;
  final int estilistasActivos;

  const DashboardMetricsDto({
    required this.totalCitasHoy,
    required this.totalCitasSemana,
    required this.ingresosHoy,
    required this.ingresosMes,
    required this.citasEnProceso,
    required this.citasCompletadas,
    required this.citasCanceladas,
    required this.estilistasActivos,
  });

  factory DashboardMetricsDto.fromJson(Map<String, dynamic> json) {
    return DashboardMetricsDto(
      totalCitasHoy: (json['totalCitasHoy'] as num?)?.toInt() ?? 0,
      totalCitasSemana: (json['totalCitasSemana'] as num?)?.toInt() ?? 0,
      ingresosHoy: (json['ingresosHoy'] as num?)?.toDouble() ?? 0.0,
      ingresosMes: (json['ingresosMes'] as num?)?.toDouble() ?? 0.0,
      citasEnProceso: (json['citasEnProceso'] as num?)?.toInt() ?? 0,
      citasCompletadas: (json['citasCompletadas'] as num?)?.toInt() ?? 0,
      citasCanceladas: (json['citasCanceladas'] as num?)?.toInt() ?? 0,
      estilistasActivos: (json['estilistasActivos'] as num?)?.toInt() ?? 0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'totalCitasHoy': totalCitasHoy,
      'totalCitasSemana': totalCitasSemana,
      'ingresosHoy': ingresosHoy,
      'ingresosMes': ingresosMes,
      'citasEnProceso': citasEnProceso,
      'citasCompletadas': citasCompletadas,
      'citasCanceladas': citasCanceladas,
      'estilistasActivos': estilistasActivos,
    };
  }

  @override
  List<Object?> get props => [
        totalCitasHoy,
        totalCitasSemana,
        ingresosHoy,
        ingresosMes,
        citasEnProceso,
        citasCompletadas,
        citasCanceladas,
        estilistasActivos,
      ];
}

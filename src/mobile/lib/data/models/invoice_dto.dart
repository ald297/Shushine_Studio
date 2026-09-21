import 'package:equatable/equatable.dart';

class InvoiceDto extends Equatable {
  final int id;
  final int citaId;
  final String codigoCita;
  final String numeroFactura;
  final String fechaEmision;
  final String clienteNombre;
  final double subtotal;
  final double iva;
  final double total;
  final String? metodoPago;

  const InvoiceDto({
    required this.id,
    required this.citaId,
    required this.codigoCita,
    required this.numeroFactura,
    required this.fechaEmision,
    required this.clienteNombre,
    required this.subtotal,
    required this.iva,
    required this.total,
    this.metodoPago,
  });

  factory InvoiceDto.fromJson(Map<String, dynamic> json) {
    return InvoiceDto(
      id: json['id'] as int? ?? 0,
      citaId: json['citaId'] as int? ?? 0,
      codigoCita: json['codigoCita']?.toString() ?? '',
      numeroFactura: json['numeroFactura']?.toString() ?? '',
      fechaEmision: json['fechaEmision']?.toString() ?? '',
      clienteNombre: json['clienteNombre']?.toString() ?? 'Consumidor Final',
      subtotal: (json['subtotal'] as num?)?.toDouble() ?? 0.0,
      iva: (json['iva'] as num?)?.toDouble() ?? 0.0,
      total: (json['total'] as num?)?.toDouble() ?? 0.0,
      metodoPago: json['metodoPago']?.toString(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'citaId': citaId,
      'codigoCita': codigoCita,
      'numeroFactura': numeroFactura,
      'fechaEmision': fechaEmision,
      'clienteNombre': clienteNombre,
      'subtotal': subtotal,
      'iva': iva,
      'total': total,
      'metodoPago': metodoPago,
    };
  }

  @override
  List<Object?> get props => [id, citaId, numeroFactura, total];
}

class RegisterPaymentRequest extends Equatable {
  final int citaId;
  final double monto;
  final String metodoPago;
  final String? referenciaPos;
  final String? motivoAjustePrecio;

  const RegisterPaymentRequest({
    required this.citaId,
    required this.monto,
    required this.metodoPago,
    this.referenciaPos,
    this.motivoAjustePrecio,
  });

  Map<String, dynamic> toJson() {
    return {
      'citaId': citaId,
      'monto': monto,
      'metodoPago': metodoPago,
      'referenciaPos': referenciaPos,
      'motivoAjustePrecio': motivoAjustePrecio,
    };
  }

  @override
  List<Object?> get props => [citaId, monto, metodoPago, referenciaPos];
}

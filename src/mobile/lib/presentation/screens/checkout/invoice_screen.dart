import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../data/models/invoice_dto.dart';
import '../../blocs/billing/billing_bloc.dart';
import '../../blocs/billing/billing_event.dart';
import '../../blocs/billing/billing_state.dart';

class InvoiceScreen extends StatefulWidget {
  final int citaId;
  final double? appointmentTotal;
  final String? appointmentCode;

  const InvoiceScreen({
    super.key,
    required this.citaId,
    this.appointmentTotal,
    this.appointmentCode,
  });

  @override
  State<InvoiceScreen> createState() => _InvoiceScreenState();
}

class _InvoiceScreenState extends State<InvoiceScreen> {
  String _metodoPago = 'Efectivo';
  final _posController = TextEditingController();

  @override
  void initState() {
    super.initState();
    context.read<BillingBloc>().add(FetchInvoiceRequested(widget.citaId));
  }

  @override
  void dispose() {
    _posController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    const primaryColor = Color(0xFFC5A059);
    const backgroundColor = Color(0xFF121214);
    const surfaceColor = Color(0xFF1E1E24);
    const cardColor = Color(0xFF26262E);
    const textColor = Color(0xFFF5F5F7);
    const subtitleColor = Color(0xFFA0A0AB);

    return Scaffold(
      backgroundColor: backgroundColor,
      appBar: AppBar(
        backgroundColor: surfaceColor,
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back_ios_new, color: primaryColor, size: 20),
          onPressed: () => Navigator.of(context).pop(),
        ),
        title: const Text(
          'Comprobante Fiscal',
          style: TextStyle(color: textColor, fontWeight: FontWeight.bold),
        ),
        centerTitle: true,
      ),
      body: BlocConsumer<BillingBloc, BillingState>(
        listener: (context, state) {
          if (state is PaymentProcessingSuccess) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text(state.message),
                backgroundColor: primaryColor,
                behavior: SnackBarBehavior.floating,
              ),
            );
            context.read<BillingBloc>().add(FetchInvoiceRequested(widget.citaId));
          }
        },
        builder: (context, state) {
          if (state is BillingLoading) {
            return const Center(child: CircularProgressIndicator(color: primaryColor));
          }

          if (state is InvoiceLoaded) {
            final inv = state.invoice;
            return _buildInvoiceView(inv);
          }

          // If no invoice yet exists, show checkout/register payment form
          return SingleChildScrollView(
            padding: const EdgeInsets.all(20),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Container(
                  width: double.infinity,
                  padding: const EdgeInsets.all(20),
                  decoration: BoxDecoration(
                    color: surfaceColor,
                    borderRadius: BorderRadius.circular(18),
                    border: Border.all(color: cardColor),
                  ),
                  child: Column(
                    children: [
                      const Icon(Icons.point_of_sale, color: primaryColor, size: 48),
                      const SizedBox(height: 12),
                      const Text(
                        'Registrar Cobro en Caja',
                        style: TextStyle(color: textColor, fontSize: 18, fontWeight: FontWeight.bold),
                      ),
                      const SizedBox(height: 6),
                      Text(
                        'Cita #${widget.appointmentCode ?? widget.citaId}',
                        style: const TextStyle(color: subtitleColor, fontSize: 13),
                      ),
                      const SizedBox(height: 16),
                      Text(
                        '\$${(widget.appointmentTotal ?? 0.0).toStringAsFixed(2)}',
                        style: const TextStyle(color: primaryColor, fontSize: 32, fontWeight: FontWeight.bold),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 24),
                const Text(
                  'Método de Pago',
                  style: TextStyle(color: textColor, fontSize: 15, fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 10),
                DropdownButtonFormField<String>(
                  initialValue: _metodoPago,
                  dropdownColor: cardColor,
                  style: const TextStyle(color: textColor, fontSize: 14),
                  decoration: InputDecoration(
                    filled: true,
                    fillColor: surfaceColor,
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(12),
                      borderSide: BorderSide.none,
                    ),
                  ),
                  items: const [
                    DropdownMenuItem(value: 'Efectivo', child: Text('Efectivo')),
                    DropdownMenuItem(value: 'Tarjeta', child: Text('Tarjeta Débito/Crédito')),
                    DropdownMenuItem(value: 'Transferencia', child: Text('Transferencia Bancaria')),
                  ],
                  onChanged: (val) {
                    if (val != null) setState(() => _metodoPago = val);
                  },
                ),
                const SizedBox(height: 16),
                if (_metodoPago == 'Tarjeta') ...[
                  TextField(
                    controller: _posController,
                    style: const TextStyle(color: textColor, fontSize: 14),
                    decoration: InputDecoration(
                      labelText: 'Número de Referencia / Voucher POS',
                      labelStyle: const TextStyle(color: subtitleColor, fontSize: 13),
                      filled: true,
                      fillColor: surfaceColor,
                      border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(12),
                        borderSide: BorderSide.none,
                      ),
                    ),
                  ),
                  const SizedBox(height: 16),
                ],
                const SizedBox(height: 20),
                SizedBox(
                  width: double.infinity,
                  height: 52,
                  child: ElevatedButton(
                    onPressed: () {
                      final req = RegisterPaymentRequest(
                        citaId: widget.citaId,
                        monto: widget.appointmentTotal ?? 0.0,
                        metodoPago: _metodoPago,
                        referenciaPos: _posController.text.trim().isEmpty ? null : _posController.text.trim(),
                      );
                      context.read<BillingBloc>().add(SubmitPaymentRequested(req));
                    },
                    style: ElevatedButton.styleFrom(
                      backgroundColor: primaryColor,
                      foregroundColor: Colors.black,
                      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
                    ),
                    child: const Text('Procesar Cobro y Emitir Factura', style: TextStyle(fontWeight: FontWeight.bold)),
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildInvoiceView(InvoiceDto inv) {
    const primaryColor = Color(0xFFC5A059);
    const surfaceColor = Color(0xFF1E1E24);
    const cardColor = Color(0xFF26262E);
    const textColor = Color(0xFFF5F5F7);
    const subtitleColor = Color(0xFFA0A0AB);

    return SingleChildScrollView(
      padding: const EdgeInsets.all(20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Receipt Card
          Container(
            padding: const EdgeInsets.all(24),
            decoration: BoxDecoration(
              color: surfaceColor,
              borderRadius: BorderRadius.circular(20),
              border: Border.all(color: primaryColor.withValues(alpha: 0.3)),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                const Text(
                  'SHUSHINE STUDIO S.A. DE C.V.',
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    color: primaryColor,
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                    letterSpacing: 1.1,
                  ),
                ),
                const SizedBox(height: 4),
                const Text(
                  'Factura de Consumidor Final • IVA 13%',
                  style: TextStyle(color: subtitleColor, fontSize: 12),
                ),
                const SizedBox(height: 16),
                const Divider(color: cardColor, height: 1),
                const SizedBox(height: 16),
                _row('N° Factura', inv.numeroFactura, isBold: true),
                const SizedBox(height: 8),
                _row('Cita N°', inv.codigoCita),
                const SizedBox(height: 8),
                _row('Cliente', inv.clienteNombre),
                const SizedBox(height: 8),
                _row('Fecha Emisión', inv.fechaEmision.split('T').first),
                const SizedBox(height: 8),
                _row('Método de Pago', inv.metodoPago ?? 'Efectivo'),
                const SizedBox(height: 16),
                const Divider(color: cardColor, height: 1),
                const SizedBox(height: 16),
                _row('Subtotal', '\$${inv.subtotal.toStringAsFixed(2)}'),
                const SizedBox(height: 8),
                _row('IVA (13%)', '\$${inv.iva.toStringAsFixed(2)}'),
                const SizedBox(height: 12),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    const Text('TOTAL PAGADO', style: TextStyle(color: textColor, fontSize: 16, fontWeight: FontWeight.bold)),
                    Text('\$${inv.total.toStringAsFixed(2)}', style: const TextStyle(color: primaryColor, fontSize: 22, fontWeight: FontWeight.bold)),
                  ],
                ),
                const SizedBox(height: 20),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
                  decoration: BoxDecoration(
                    color: const Color(0xFF4CAF50).withValues(alpha: 0.15),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: const Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Icon(Icons.check_circle, color: Color(0xFF4CAF50), size: 16),
                      SizedBox(width: 6),
                      Text('Comprobante Pagado', style: TextStyle(color: Color(0xFF4CAF50), fontSize: 12, fontWeight: FontWeight.bold)),
                    ],
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 24),
          SizedBox(
            width: double.infinity,
            height: 50,
            child: OutlinedButton.icon(
              onPressed: () {
                Clipboard.setData(ClipboardData(text: inv.numeroFactura));
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(
                    content: Text('Número de factura copiado'),
                    backgroundColor: primaryColor,
                    behavior: SnackBarBehavior.floating,
                  ),
                );
              },
              icon: const Icon(Icons.copy, color: primaryColor, size: 18),
              label: const Text('Copiar N° de Comprobante', style: TextStyle(color: textColor)),
              style: OutlinedButton.styleFrom(
                side: const BorderSide(color: cardColor),
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(14)),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _row(String label, String value, {bool isBold = false}) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(label, style: const TextStyle(color: Color(0xFFA0A0AB), fontSize: 13)),
        Text(
          value,
          style: TextStyle(
            color: const Color(0xFFF5F5F7),
            fontSize: 13,
            fontWeight: isBold ? FontWeight.bold : FontWeight.normal,
          ),
        ),
      ],
    );
  }
}

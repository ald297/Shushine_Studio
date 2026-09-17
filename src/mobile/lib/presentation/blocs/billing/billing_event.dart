import 'package:equatable/equatable.dart';
import '../../../data/models/invoice_dto.dart';

abstract class BillingEvent extends Equatable {
  const BillingEvent();

  @override
  List<Object?> get props => [];
}

class FetchInvoiceRequested extends BillingEvent {
  final int citaId;

  const FetchInvoiceRequested(this.citaId);

  @override
  List<Object?> get props => [citaId];
}

class SubmitPaymentRequested extends BillingEvent {
  final RegisterPaymentRequest request;

  const SubmitPaymentRequested(this.request);

  @override
  List<Object?> get props => [request];
}

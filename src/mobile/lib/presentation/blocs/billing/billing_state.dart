import 'package:equatable/equatable.dart';
import '../../../data/models/invoice_dto.dart';

abstract class BillingState extends Equatable {
  const BillingState();

  @override
  List<Object?> get props => [];
}

class BillingInitial extends BillingState {}

class BillingLoading extends BillingState {}

class InvoiceLoaded extends BillingState {
  final InvoiceDto invoice;

  const InvoiceLoaded(this.invoice);

  @override
  List<Object?> get props => [invoice];
}

class PaymentProcessingSuccess extends BillingState {
  final String message;

  const PaymentProcessingSuccess(this.message);

  @override
  List<Object?> get props => [message];
}

class BillingError extends BillingState {
  final String message;

  const BillingError(this.message);

  @override
  List<Object?> get props => [message];
}

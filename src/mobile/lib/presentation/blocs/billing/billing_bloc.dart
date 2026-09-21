import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../domain/usecases/billing/get_invoice_usecase.dart';
import '../../../domain/usecases/billing/register_payment_usecase.dart';
import 'billing_event.dart';
import 'billing_state.dart';

class BillingBloc extends Bloc<BillingEvent, BillingState> {
  final GetInvoiceUseCase _getInvoiceUseCase;
  final RegisterPaymentUseCase _registerPaymentUseCase;

  BillingBloc({
    required GetInvoiceUseCase getInvoiceUseCase,
    required RegisterPaymentUseCase registerPaymentUseCase,
  })  : _getInvoiceUseCase = getInvoiceUseCase,
        _registerPaymentUseCase = registerPaymentUseCase,
        super(BillingInitial()) {
    on<FetchInvoiceRequested>(_onFetchInvoice);
    on<SubmitPaymentRequested>(_onSubmitPayment);
  }

  Future<void> _onFetchInvoice(
    FetchInvoiceRequested event,
    Emitter<BillingState> emit,
  ) async {
    emit(BillingLoading());
    final result = await _getInvoiceUseCase(event.citaId);
    result.when(
      onSuccess: (invoice) => emit(InvoiceLoaded(invoice)),
      onError: (failure) => emit(BillingError(failure.message)),
    );
  }

  Future<void> _onSubmitPayment(
    SubmitPaymentRequested event,
    Emitter<BillingState> emit,
  ) async {
    emit(BillingLoading());
    final result = await _registerPaymentUseCase(event.request);
    result.when(
      onSuccess: (_) => emit(const PaymentProcessingSuccess('Pago registrado y factura emitida con éxito.')),
      onError: (failure) => emit(BillingError(failure.message)),
    );
  }
}

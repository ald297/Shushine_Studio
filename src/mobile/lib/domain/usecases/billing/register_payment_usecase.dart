import '../../../../core/error/failures.dart';
import '../../../data/models/invoice_dto.dart';
import '../../repositories/billing_repository.dart';

class RegisterPaymentUseCase {
  final BillingRepository _repository;

  RegisterPaymentUseCase(this._repository);

  Future<Result<bool>> call(RegisterPaymentRequest request) async {
    return await _repository.registerPayment(request);
  }
}

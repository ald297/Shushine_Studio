import '../../../../core/error/failures.dart';
import '../../../data/models/invoice_dto.dart';
import '../../repositories/billing_repository.dart';

class GetInvoiceUseCase {
  final BillingRepository _repository;

  GetInvoiceUseCase(this._repository);

  Future<Result<InvoiceDto>> call(int citaId) async {
    return await _repository.getInvoiceByAppointmentId(citaId);
  }
}

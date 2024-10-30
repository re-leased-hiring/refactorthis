using RefactorThis.Domain.Entities;
using RefactorThis.Domain.RepositoryInterfaces;

namespace RefactorThis.Infrastructure.Persistence.Repository {
	public class InvoiceRepository : IInvoiceRepository
	{
		private Invoice _invoice;

		public Invoice GetInvoice(string reference)
		{
			return _invoice;
		}

		public void SaveInvoice(Invoice invoice)
		{
			//saves the invoice to the database
		}

		public void Add(Invoice invoice)
		{
			_invoice = invoice;
		}
	}
}
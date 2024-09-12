using System;
using System.Linq;
using RefactorThis.Persistence;

namespace RefactorThis.Domain {
  public class InvoiceService {
    private readonly InvoiceRepository _invoiceRepository;

    public InvoiceService(InvoiceRepository invoiceRepository) {
      _invoiceRepository = invoiceRepository;
    }

    public string ProcessPayment(Payment payment) {
      var inv = _invoiceRepository.GetInvoice(payment.Reference);

      var responseMessage = string.Empty;

      var invoicePayment   = inv.Payments;
      var invoiceAmount    = inv.Amount;
      var invoiceAmtPaid   = inv.AmountPaid;
      var invoiceType      = inv.Type;
      var invoiceTax       = inv.TaxAmount;
      var InvPaymentSum    = inv.Payments.Sum(x => x.Amount)
      var InvBalance       = inv.Amount - inv.AmountPaid
      var InvAmtPaid       = inv.AmountPaid;
      var tax              = 0.14m;
      var paymentAmount    = payment.Amount;

      if (inv == null) //No Invoices
	{  
      		throw new InvalidOperationException("There is no invoice matching this payment");
	}

      if (invoiceAmount == 0) && (invoicePayment.Any() != true)  //No Payment
	{
        	responseMessage = "no payment needed";
      	}
      else 
	{
        	throw new InvalidOperationException("The invoice is in an invalid state, it has an invoiceAmount of 0 and it has payments.");
      	}
	  

      if (invoiceAmount && invoicePayment.Any() == true) 
	{
		if (invPaymentSum != 0 && invoiceAmount == InvPaymentSum) 
		{
			responseMessage = "invoice was already fully paid";
		} 
		else if (InvPaymentSum != 0 && paymentAmount > InvBalance) 
		{
		  responseMessage = "the payment is greater than the partial invoiceAmount remaining";
		} 
		else 
		{
		InvAmtPaid += paymentAmount;
		invoicePayment.Add( payment );
	
			switch (inv.Type) 
			{
				case InvoiceType.Standard:
					InvBalance == paymentAmount ? 
					responseMessage = "final partial payment received, invoice is now fully paid" : responseMessage = "another partial payment received, still not fully paid";
					break;
				case InvoiceType.Commercial:
					invoiceTax += paymentAmount * tax;
					InvBalance == paymentAmount ? 
					responseMessage = "final partial payment received, invoice is now fully paid" : responseMessage = "another partial payment received, still not fully paid";
					break;
				default:
				throw new ArgumentOutOfRangeException();
		       }
		}
          }
	  else 
	  {
		if(invoicePayment > invoiceAmount) 
		{
			responseMessage = "the payment is greater than the invoice amount";
		}
		else 
		{
			InvAmtPaid = paymentAmount;
			invoiceTax = InvAmtPaid * tax;
			invoicePayment.Add(payment);
			
			switch (inv.Type) 
			{
				case InvoiceType.Standard:
					invoiceAmount == paymentAmount ? 
					responseMessage = "invoice is now fully paid" : responseMessage = "invoice is now partially paid";
					break;
				case InvoiceType.Commercial:
					invoiceAmount == paymentAmount ? 
					responseMessage = "invoice is now fully paid" : responseMessage = "invoice is now partially paid";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
      }
      inv.Save();

      return responseMessage;
    }
  }
}

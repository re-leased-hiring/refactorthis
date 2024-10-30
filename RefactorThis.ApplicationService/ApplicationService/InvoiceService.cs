using System;
using System.Linq;
using RefactorThis.ApplicationService.Interface;
using RefactorThis.Domain.Entities;
using RefactorThis.Domain.RepositoryInterfaces;
using RefactorThis.Common.Enums;

namespace RefactorThis.ApplicationService
{
	public class InvoiceService : IInvoiceService
	{
		private readonly IInvoiceRepository _invoiceRepository;

		public InvoiceService(IInvoiceRepository invoiceRepository )
		{
			_invoiceRepository = invoiceRepository;
		}

		private void ProcessPaymentNoTax(Invoice inv, Payment payment)
		{
			inv.AmountPaid += payment.Amount;
			inv.Payments.Add(payment);
		}

		private void ProcessPaymentWithTax(Invoice inv, Payment payment)
		{
			inv.AmountPaid += payment.Amount;
			inv.TaxAmount += payment.Amount * 0.14m;
			inv.Payments.Add(payment);
		}

		public string ProcessPayment(Payment payment)
		{
			var inv = _invoiceRepository.GetInvoice(payment.Reference);

			var responseMessage = string.Empty;

			if ( inv == null )
			{
				throw new InvalidOperationException(PaymentResponse.InvoiceNotFound);
			}

			if (inv.Amount == 0)
			{
				if (inv.Payments == null || !inv.Payments.Any())
				{
					responseMessage = PaymentResponse.NoPaymentNeeded;

					_invoiceRepository.SaveInvoice(inv);

					return responseMessage;
				}
				else
				{
					throw new InvalidOperationException(PaymentResponse.InvalidInvoiceState);
				}
			}

			if ( inv.Payments != null && inv.Payments.Any( ) )
			{
				if ( inv.Payments.Sum( x => x.Amount ) != 0 && inv.Amount == inv.Payments.Sum( x => x.Amount ) )
				{
					responseMessage = PaymentResponse.InvoiceAlreadyFullyPaid;

					_invoiceRepository.SaveInvoice(inv);

					return responseMessage;
				}
				
				if ( inv.Payments.Sum( x => x.Amount ) != 0 && payment.Amount > ( inv.Amount - inv.AmountPaid ) )
				{
					responseMessage = PaymentResponse.PaymentGreaterThanRemaining;

					_invoiceRepository.SaveInvoice(inv);

					return responseMessage;
				}

				if ( ( inv.Amount - inv.AmountPaid ) == payment.Amount )
				{
					switch ( inv.Type )
					{
						case InvoiceType.Standard:
							ProcessPaymentNoTax(inv, payment);
							responseMessage = PaymentResponse.FinalPartialPaymentReceived;
							break;
						case InvoiceType.Commercial:
							ProcessPaymentWithTax(inv, payment);
							inv.Payments.Add( payment );
							responseMessage = PaymentResponse.FinalPartialPaymentReceived;
							break;
						default:
							throw new ArgumentOutOfRangeException( );
					}		
				}
				else
				{
					switch ( inv.Type )
					{
						case InvoiceType.Standard:
							ProcessPaymentNoTax(inv, payment);
							responseMessage = PaymentResponse.AnotherPartialPaymentReceived;
							break;
						case InvoiceType.Commercial:
							ProcessPaymentWithTax(inv, payment);
							inv.Payments.Add( payment );
							responseMessage = PaymentResponse.AnotherPartialPaymentReceived;
							break;
						default:
							throw new ArgumentOutOfRangeException( );
					}
				}
			}
			else
			{
				if ( payment.Amount > inv.Amount )
				{
					responseMessage = PaymentResponse.PaymentGreaterThanInvoiceAmount;

					_invoiceRepository.SaveInvoice(inv);

					return responseMessage;
				}

				if ( inv.Amount == payment.Amount )
				{
					switch ( inv.Type )
					{
						case InvoiceType.Standard:
							ProcessPaymentNoTax(inv, payment);
							responseMessage = PaymentResponse.InvoiceFullyPaid;
							break;
						case InvoiceType.Commercial:
							ProcessPaymentWithTax(inv, payment);
							responseMessage = PaymentResponse.InvoiceFullyPaid;
							break;
						default:
							throw new ArgumentOutOfRangeException( );
					}
				}
				else
				{
					switch ( inv.Type )
					{
						case InvoiceType.Standard:
							ProcessPaymentWithTax(inv, payment);
							inv.Payments.Add( payment );
							responseMessage = PaymentResponse.InvoicePartiallyPaid;
							break;
						case InvoiceType.Commercial:
							ProcessPaymentWithTax(inv, payment);
							inv.Payments.Add( payment );
							responseMessage = PaymentResponse.InvoicePartiallyPaid;
							break;
						default:
							throw new ArgumentOutOfRangeException( );
					}
				}
			}

			_invoiceRepository.SaveInvoice(inv);

			return responseMessage;
		}
	}
}
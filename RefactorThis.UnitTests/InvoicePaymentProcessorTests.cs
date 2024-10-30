using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RefactorThis.Domain.Entities;
using RefactorThis.Domain.RepositoryInterfaces;
using RefactorThis.Common.Enums;
using RefactorThis.ApplicationService.Interface;
using RefactorThis.ApplicationService;

namespace RefactorThis.UnitTest
{
	[TestFixture]
	public class InvoicePaymentProcessorTests
	{
		private Mock<IInvoiceRepository> _mockInvoiceRepository;
		private IInvoiceService _paymentProcessor;

		[SetUp]
		public void SetUp()
		{
			_mockInvoiceRepository = new Mock<IInvoiceRepository>();
			_paymentProcessor = new InvoiceService(_mockInvoiceRepository.Object);
		}

		[Test]
		public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference( )
		{
			var payment = new Payment();
			var failureMessage = "";

			try
			{
				var result = _paymentProcessor.ProcessPayment(payment);
			}
			catch ( InvalidOperationException e )
			{
				failureMessage = e.Message;
			}

			Assert.AreEqual( PaymentResponse.InvoiceNotFound, failureMessage);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded( )
		{
			var invoice = new Invoice()
			{
				Amount = 0,
				AmountPaid = 0,
				Payments = null
			};

			var payment = new Payment();

			_mockInvoiceRepository.Object.Add(invoice);
			_mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var result = _paymentProcessor.ProcessPayment(payment);

			Assert.AreEqual( PaymentResponse.NoPaymentNeeded, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid( )
		{
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 10,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 10
					}
				}
			};

			var payment = new Payment();

			_mockInvoiceRepository.Object.Add( invoice );
			_mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var result = _paymentProcessor.ProcessPayment(payment);

			Assert.AreEqual(PaymentResponse.InvoiceAlreadyFullyPaid, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue( )
		{
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 5,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 5
					}
				}
			};

			var payment = new Payment()
			{
				Amount = 6
			};

			_mockInvoiceRepository.Object.Add( invoice );
		    _mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var result = _paymentProcessor.ProcessPayment(payment);

			Assert.AreEqual(PaymentResponse.PaymentGreaterThanRemaining, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount( )
		{
			var invoice = new Invoice()
			{
				Amount = 5,
				AmountPaid = 0,
				Payments = new List<Payment>( )
			};

			var payment = new Payment()
			{
				Amount = 6
			};

			_mockInvoiceRepository.Object.Add( invoice );
			_mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var result = _paymentProcessor.ProcessPayment(payment);

			Assert.AreEqual(PaymentResponse.PaymentGreaterThanInvoiceAmount, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue( )
		{
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 5,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 5
					}
				}
			};

			var payment = new Payment()
			{
				Amount = 5
			};

			_mockInvoiceRepository.Object.Add( invoice );
			_mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var result = _paymentProcessor.ProcessPayment(payment);

			Assert.AreEqual(PaymentResponse.FinalPartialPaymentReceived, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount( )
		{
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>( ) { new Payment( ) { Amount = 10 } }
			};

			var payment = new Payment()
			{
				Amount = 10
			};

			_mockInvoiceRepository.Object.Add( invoice );
			_mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var result = _paymentProcessor.ProcessPayment(payment);

			Assert.AreEqual(PaymentResponse.InvoiceAlreadyFullyPaid, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue( )
		{
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 5,
				Payments = new List<Payment>
				{
					new Payment
					{
						Amount = 5
					}
				}
			};

			var payment = new Payment()
			{
				Amount = 1
			};

			_mockInvoiceRepository.Object.Add( invoice );
			_mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var result = _paymentProcessor.ProcessPayment(payment);

			Assert.AreEqual(PaymentResponse.AnotherPartialPaymentReceived, result);
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount( )
		{
			var invoice = new Invoice()
			{
				Amount = 10,
				AmountPaid = 0,
				Payments = new List<Payment>( )
			};

			var payment = new Payment()
			{
				Amount = 1
			};

			_mockInvoiceRepository.Object.Add( invoice );
			_mockInvoiceRepository.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

			var result = _paymentProcessor.ProcessPayment(payment);

			Assert.AreEqual(PaymentResponse.InvoicePartiallyPaid, result);
		}
	}
}
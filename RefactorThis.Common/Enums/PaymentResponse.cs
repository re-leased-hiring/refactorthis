using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Common.Enums
{
    public class PaymentResponse
    {
        public const string NoPaymentNeeded = "no payment needed";
        public const string InvoiceNotFound = "There is no invoice matching this payment";
        public const string InvalidInvoiceState = "The invoice is in an invalid state, it has an amount of 0 and it has payments.";
        public const string InvoiceAlreadyFullyPaid = "invoice was already fully paid";
        public const string PaymentGreaterThanRemaining = "the payment is greater than the partial amount remaining";
        public const string FinalPartialPaymentReceived = "final partial payment received, invoice is now fully paid";
        public const string AnotherPartialPaymentReceived = "another partial payment received, still not fully paid";
        public const string PaymentGreaterThanInvoiceAmount = "the payment is greater than the invoice amount";
        public const string InvoiceFullyPaid = "invoice is now fully paid";
        public const string InvoicePartiallyPaid = "invoice is now partially paid";
    }
}

using RefactorThis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.ApplicationService.Interface
{
    public interface IInvoiceService
    {
        string ProcessPayment(Payment payment);
    }
}

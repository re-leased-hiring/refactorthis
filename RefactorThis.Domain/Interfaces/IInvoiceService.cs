﻿using RefactorThis.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain.Interfaces
{
    public interface IInvoiceService
    {
        string ProcessPayment(Payment payment);
    }
}

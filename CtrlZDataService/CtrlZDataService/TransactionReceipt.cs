using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CtrlZDataService
{
    public class TransactionReceipt
    {
        public Guid transactionId;
        public string duration;
        public float matchPercent;
        public bool match;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyoDataTest
{
    public class EmgData
    {
        public Guid transactionId { get; set; }
        public int emg1 { get; set; }
        public int emg2 { get; set; }
        public int emg3 { get; set; }
        public int emg4 { get; set; }
        public int emg5 { get; set; }
        public int emg6 { get; set; }
        public int emg7 { get; set; }
        public int emg8 { get; set; }
        public int orderReceived { get; set; }
        public long timestamp { get; set; }
        public DateTime createdDate { get; set; }

    }
}

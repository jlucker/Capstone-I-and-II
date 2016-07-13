using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CtrlZDataService
{
    [DataContract]
    public class EmgData
    {
        [DataMember]
        public Guid transactionId { get; set; }

        [DataMember]
        public int emg1 { get; set; }

        [DataMember]
        public int emg2 { get; set; }

        [DataMember]
        public int emg3 { get; set; }

        [DataMember]
        public int emg4 { get; set; }

        [DataMember]
        public int emg5 { get; set; }

        [DataMember]
        public int emg6 { get; set; }

        [DataMember]
        public int emg7 { get; set; }

        [DataMember]
        public int emg8 { get; set; }

        [DataMember]
        public int orderReceived { get; set; }

        [DataMember]
        public long timestamp { get; set; }

        [DataMember]
        public DateTime createdDate { get; set; }

    }
}

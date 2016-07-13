using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CtrlZDataService
{
    [DataContract]
    public class MotionData
    {
        [DataMember]
        public Guid transactionId { get; set; }

        [DataMember]
        public double gyroX { get; set; }

        [DataMember]
        public double gyroY { get; set; }

        [DataMember]
        public double gyroZ { get; set; }

        [DataMember]
        public double accelX { get; set; }

        [DataMember]
        public double accelY { get; set; }

        [DataMember]
        public double accelZ { get; set; }

        [DataMember]
        public double orientationX { get; set; }

        [DataMember]
        public double orientationY { get; set; }

        [DataMember]
        public double orientationZ { get; set; }

        [DataMember]
        public double orientationW { get; set; }

        [DataMember]
        public double orientationRoll { get; set; }

        [DataMember]
        public double orientationPitch { get; set; }

        [DataMember]
        public double orientationYaw { get; set; }

        [DataMember]
        public int orderReceived { get; set; }

        [DataMember]
        public long timestamp { get; set; }

        [DataMember]
        public DateTime createdDate { get; set; }

    }
}

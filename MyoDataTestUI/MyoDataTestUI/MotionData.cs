using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyoDataTest
{
    public class MotionData
    {
        public Guid transactionId { get; set; }
        public double gyroX { get; set; }
        public double gyroY { get; set; }
        public double gyroZ { get; set; }
        public double accelX { get; set; }
        public double accelY { get; set; }
        public double accelZ { get; set; }
        public double orientationX { get; set; }
        public double orientationY { get; set; }
        public double orientationZ { get; set; }
        public double orientationW { get; set; }
        public double orientationRoll { get; set; }
        public double orientationPitch { get; set; }
        public double orientationYaw { get; set; }
        public int orderReceived { get; set; }
        public long timestamp { get; set; }
        public DateTime createdDate { get; set; }

    }
}

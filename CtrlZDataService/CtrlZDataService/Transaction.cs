using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CtrlZDataService
{
    [DataContract]
    public class Transaction
    {
        [DataMember]
        public Guid id { get; set; }

        [DataMember]
        public string username { get; set; }

        [DataMember]
        public string gesture { get; set; }

        [DataMember]
        public DateTime createdDate { get; set; }

        [DataMember]
        public List<EmgData> emgList = new List<EmgData>();

        [DataMember]
        public List<MotionData> motionList = new List<MotionData>();

        
        public void addEmg(Guid transactionId, int emg1, int emg2, int emg3, int emg4, int emg5, int emg6, int emg7,
            int emg8, int orderReceived, long timestamp)
        {
            var emg = new EmgData();
            emg.transactionId = transactionId;
            emg.emg1 = emg1;
            emg.emg2 = emg2;
            emg.emg3 = emg3;
            emg.emg4 = emg4;
            emg.emg5 = emg5;
            emg.emg6 = emg6;
            emg.emg7 = emg7;
            emg.emg8 = emg8;
            emg.orderReceived = orderReceived;
            emg.timestamp = timestamp;
            emg.createdDate = DateTime.Now;
            emgList.Add(emg);
        }

        public void addEmg(EmgData e1)
        {
            var emg = new EmgData();
            emg.transactionId = e1.transactionId;
            emg.emg1 = e1.emg1;
            emg.emg2 = e1.emg2;
            emg.emg3 = e1.emg3;
            emg.emg4 = e1.emg4;
            emg.emg5 = e1.emg5;
            emg.emg6 = e1.emg6;
            emg.emg7 = e1.emg7;
            emg.emg8 = e1.emg8;
            emg.orderReceived = e1.orderReceived;
            emg.timestamp = e1.timestamp;
            emg.createdDate = DateTime.Now;
            emgList.Add(emg);
        }

        public void addMotion(Guid transactionId, float gyroX, float gyroY, float gyroZ, float accelX, float accelY,
            float accelZ, float orientationX, float orientationY, float orientationZ, float orientationW,
            float orientationRoll, float orientationPitch, float orientationYaw, int orderReceived, long timestamp)
        {
            var motion = new MotionData();
            motion.transactionId = transactionId;
            motion.gyroX = gyroX;
            motion.gyroY = gyroY;
            motion.gyroZ = gyroZ;
            motion.accelX = accelX;
            motion.accelY = accelY;
            motion.accelZ = accelZ;
            motion.orientationX = orientationX;
            motion.orientationY = orientationY;
            motion.orientationZ = orientationZ;
            motion.orientationRoll = orientationRoll;
            motion.orientationPitch = orientationPitch;
            motion.orientationYaw = orientationYaw;
            motion.orderReceived = orderReceived;
            motion.timestamp = timestamp;
            motion.createdDate = DateTime.Now;
            motionList.Add(motion);

        }
        public void addMotion(MotionData m1)
        {
            var motion = new MotionData();
            motion.transactionId = m1.transactionId;
            motion.gyroX = m1.gyroX;
            motion.gyroY = m1.gyroY;
            motion.gyroZ = m1.gyroZ;
            motion.accelX = m1.accelX;
            motion.accelY = m1.accelY;
            motion.accelZ = m1.accelZ;
            motion.orientationX = m1.orientationX;
            motion.orientationY = m1.orientationY;
            motion.orientationZ = m1.orientationZ;
            motion.orientationRoll = m1.orientationRoll;
            motion.orientationPitch = m1.orientationPitch;
            motion.orientationYaw = m1.orientationYaw;
            motion.orderReceived = m1.orderReceived;
            motion.timestamp = m1.timestamp;
            motion.createdDate = DateTime.Now;
            motionList.Add(motion);

        }
    }
}

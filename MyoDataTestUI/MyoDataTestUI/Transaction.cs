using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyoDataTest
{
    class Transaction
    {
        public Guid id { get; set; }
        public string username { get; set; }
        public string gesture { get; set; }
        public DateTime createdDate { get; set; }

        public List<EmgData> emgList = new List<EmgData>();
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
    }
}

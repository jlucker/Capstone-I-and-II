using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyoSharp;
using MyoSharp.Communication;
using MyoSharp.ConsoleSample.Internal;
using MyoSharp.Device;
using MyoSharp.Exceptions;

namespace MyoDataServiceTestUI
{
    class MyoConnection
    {
        private IChannel _channel;
        private IHub _hub;

        public CtrlZDataService.Transaction transaction = new CtrlZDataService.Transaction();
        public List<CtrlZDataService.EmgData> emgResults = new List<CtrlZDataService.EmgData>();
        public List<CtrlZDataService.MotionData> gyroResults = new List<CtrlZDataService.MotionData>();
        public List<CtrlZDataService.MotionData> accelResults = new List<CtrlZDataService.MotionData>();
        public List<CtrlZDataService.MotionData> orientationResults = new List<CtrlZDataService.MotionData>();
        public List<CtrlZDataService.MotionData> orientationEulerResults = new List<CtrlZDataService.MotionData>();
        public List<CtrlZDataService.MotionData> motionResults = new List<CtrlZDataService.MotionData>();

        int emgOrderReceived = 0;
        int motionOrderReceived = 0;

        // Creating now rather than calling DateTime.Now.Date 100000 times
        // Greatly reduces the calls to the OS
        private DateTime now = DateTime.Now.Date;
        private DateTime calculateTimestamp = new DateTime(1970, 1, 1);

        public CtrlZDataService.Transaction CreateConnection()
        {
            var inputTime = DateTime.UtcNow;
            _channel = Channel.Create(ChannelDriver.Create(ChannelBridge.Create()));
            _hub = Hub.Create(_channel);
            _channel.StartListening();

            // TODO Change to a request based system
            // The Thread.Sleep is set to 15 milliseconds which allows the Myo to update the data
            // The usual while loop requests data to quickly so there is a bunch of redundant data
            // in the final output.  

            while (DateTime.UtcNow - inputTime <= TimeSpan.FromSeconds(5))
            {
                Thread.Sleep(5);
                _hub.MyoConnected += Hub_MyoConnected;
                _hub.MyoDisconnected += Hub_MyoDisconnected;
            }

 
            Console.WriteLine("DONE!!!");

            _channel.StopListening();
            _channel.Dispose();
            _hub.Dispose();

            CombineData();

            transaction.username = "defaultUser";
            transaction.gesture = "defaultGesture";

            return transaction;
        }

        private void Myo_EmgDataAcquired(object sender, EmgDataEventArgs e)
        {
            CtrlZDataService.EmgData emg = new CtrlZDataService.EmgData();
            //emg.timestamp = Convert.ToInt64(e.Timestamp);
            emg.timestamp = (long)((DateTime.UtcNow - calculateTimestamp).TotalMilliseconds);
            emg.emg1 = e.EmgData.GetDataForSensor(1);
            emg.emg2 = e.EmgData.GetDataForSensor(2);
            emg.emg3 = e.EmgData.GetDataForSensor(3);
            emg.emg4 = e.EmgData.GetDataForSensor(4);
            emg.emg5 = e.EmgData.GetDataForSensor(5);
            emg.emg6 = e.EmgData.GetDataForSensor(6);
            emg.emg7 = e.EmgData.GetDataForSensor(7);
            emg.emg8 = e.EmgData.GetDataForSensor(8);
            emg.createdDate = now;

            if (emgResults.Count > 0 && emgResults.Last().timestamp != emg.timestamp)
            {
                emg.orderReceived = emgOrderReceived;
                emgOrderReceived++;
                emgResults.Add(emg);
            }
            else if (emgResults.Count == 0)
            {
                emg.orderReceived = emgOrderReceived;
                emgOrderReceived++;
                emgResults.Add(emg);
            }
        }

        private void Myo_OrientationDataAcquired(object sender, OrientationDataEventArgs e)
        {
            const float PI = (float)System.Math.PI;
            
            // convert the values to a 0-9 scale (for easier digestion/understanding)
            var orientationRoll = (int)((e.Roll + PI) / (PI * 2.0f) * 10);
            var orientationPitch = (int)((e.Pitch + PI) / (PI * 2.0f) * 10);
            var orientationYaw = (int)((e.Yaw + PI) / (PI * 2.0f) * 10);

            CtrlZDataService.MotionData newData = new CtrlZDataService.MotionData();
            newData.createdDate = now;
            newData.timestamp = (long)((DateTime.UtcNow - calculateTimestamp).TotalMilliseconds);
            newData.orientationX = Convert.ToDouble(e.Orientation.X);
            newData.orientationY = Convert.ToDouble(e.Orientation.Y);
            newData.orientationZ = Convert.ToDouble(e.Orientation.Z);
            newData.orientationW = Convert.ToDouble(e.Orientation.W);
            newData.orientationRoll = Convert.ToDouble(orientationRoll);
            newData.orientationPitch = Convert.ToDouble(orientationPitch);
            newData.orientationYaw = Convert.ToDouble(orientationYaw);
            
            if (orientationResults.Count > 0 && orientationResults.Last().timestamp != newData.timestamp)
            {
                orientationResults.Add(newData);
            }
            else if (orientationResults.Count == 0)
            {
                orientationResults.Add(newData);
            }
        }

        private void Myo_AccelerometerDataAcquired(object sender, AccelerometerDataEventArgs e)
        {
            CtrlZDataService.MotionData newData = new CtrlZDataService.MotionData();
            newData.createdDate = now;
            newData.timestamp = (long)((DateTime.UtcNow - calculateTimestamp).TotalMilliseconds);
            newData.accelX = Convert.ToDouble(e.Accelerometer.X);
            newData.accelY = Convert.ToDouble(e.Accelerometer.Y);
            newData.accelZ = Convert.ToDouble(e.Accelerometer.Z);
            
            if (accelResults.Count > 0 &&  accelResults.Last().timestamp != newData.timestamp)
            {
                accelResults.Add(newData);
            }
            else if (accelResults.Count == 0)
            {
                accelResults.Add(newData);
            }
        }

        private void Myo_GyroscopeDataAcquired(object sender, GyroscopeDataEventArgs e)
        {
            CtrlZDataService.MotionData newData = new CtrlZDataService.MotionData();
            newData.createdDate = now;
            newData.timestamp = (long)((DateTime.UtcNow - calculateTimestamp).TotalMilliseconds);
            newData.gyroX = Convert.ToDouble(e.Gyroscope.X);
            newData.gyroY = Convert.ToDouble(e.Gyroscope.Y);
            newData.gyroZ = Convert.ToDouble(e.Gyroscope.Z);
            

            if (gyroResults.Count > 0 &&  gyroResults.Last().timestamp != newData.timestamp)
            {
                newData.orderReceived = motionOrderReceived;
                motionOrderReceived++;
                gyroResults.Add(newData);
            }
            else if (gyroResults.Count == 0)
            {
                newData.orderReceived = motionOrderReceived;
                motionOrderReceived++;
                gyroResults.Add(newData);
            }
        }

        private void Hub_MyoDisconnected(object sender, MyoEventArgs e)
        {
            e.Myo.EmgDataAcquired -= Myo_EmgDataAcquired;
            e.Myo.OrientationDataAcquired -= Myo_OrientationDataAcquired;
            e.Myo.EmgDataAcquired -= Myo_EmgDataAcquired;
            e.Myo.AccelerometerDataAcquired -= Myo_AccelerometerDataAcquired;
            e.Myo.GyroscopeDataAcquired -= Myo_GyroscopeDataAcquired;
        }

        private void Hub_MyoConnected(object sender, MyoEventArgs e)
        {
            e.Myo.EmgDataAcquired += Myo_EmgDataAcquired;
            e.Myo.OrientationDataAcquired += Myo_OrientationDataAcquired;
            e.Myo.AccelerometerDataAcquired += Myo_AccelerometerDataAcquired;
            e.Myo.GyroscopeDataAcquired += Myo_GyroscopeDataAcquired;
            e.Myo.SetEmgStreaming(true);
        }

    
        private void CombineData()
        {
            Console.WriteLine("EMG: " + emgResults.Count);
            Console.WriteLine("Gyro: " + gyroResults.Count);
            Console.WriteLine("Accel: " + accelResults.Count);
            Console.WriteLine("Orientation: " + orientationResults.Count);

            for (var i = 0; i < gyroResults.Count; i++)
            {
                CtrlZDataService.MotionData mData = new CtrlZDataService.MotionData();
                mData.gyroX = gyroResults.ElementAt(i).gyroX;
                mData.gyroY = gyroResults.ElementAt(i).gyroY;
                mData.gyroZ = gyroResults.ElementAt(i).gyroZ;
                mData.accelX = accelResults.ElementAt(i).accelX;
                mData.accelY = accelResults.ElementAt(i).accelY;
                mData.accelZ = accelResults.ElementAt(i).accelZ;
                mData.orientationX = orientationResults.ElementAt(i).orientationX;
                mData.orientationY = orientationResults.ElementAt(i).orientationY;
                mData.orientationZ = orientationResults.ElementAt(i).orientationZ;
                mData.orientationW = orientationResults.ElementAt(i).orientationW;
                mData.orientationRoll = orientationResults.ElementAt(i).orientationRoll;
                mData.orientationPitch = orientationResults.ElementAt(i).orientationPitch;
                mData.orientationYaw = orientationResults.ElementAt(i).orientationYaw;
                mData.timestamp = gyroResults.ElementAt(i).timestamp;
                mData.createdDate = now;
                mData.orderReceived = gyroResults.ElementAt(i).orderReceived;
                motionResults.Add(mData);
            }
            transaction.emgList = emgResults.ToArray();
            transaction.motionList = motionResults.ToArray();
        }
    }
}

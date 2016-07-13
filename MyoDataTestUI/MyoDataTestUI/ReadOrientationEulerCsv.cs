using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyoDataTest
{
    class ReadOrientationEulerCsv
    {
        public string filepath { get; set; }

        public List<MotionData> ReadData()
        {
            List<MotionData> data = new List<MotionData>();
            var reader = new StreamReader(File.OpenRead(filepath));
            int i = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                var val = values[0].Split(',');

                if (!val[0].Contains("timestamp"))
                {
                    MotionData newData = new MotionData();
                    newData.orderReceived = i;
                    newData.createdDate = DateTime.Now;
                    newData.timestamp = Convert.ToInt64(val[0]);
                    newData.orientationRoll = Convert.ToDouble(val[1]);
                    newData.orientationPitch = Convert.ToDouble(val[2]);
                    newData.orientationYaw = Convert.ToDouble(val[3]);
                    data.Add(newData);
                    i++;
                }
            }

            return data;
        }
    }
}

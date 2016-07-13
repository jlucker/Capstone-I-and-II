using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyoDataTest
{
    class ReadEmgCsv
    {
        public string filepath { get; set; }

        public List<EmgData> ReadData()
        {
            List<EmgData>data = new List<EmgData>();
            var reader = new StreamReader(File.OpenRead(filepath));
            int i = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                var val = values[0].Split(',');

                if (!val[0].Contains("timestamp"))
                {
                    EmgData newData = new EmgData();
                    newData.orderReceived = i;
                    newData.createdDate = DateTime.Now;
                    newData.timestamp = Convert.ToInt64(val[0]);
                    newData.emg1 = Convert.ToInt32(val[1]);
                    newData.emg2 = Convert.ToInt32(val[2]);
                    newData.emg3 = Convert.ToInt32(val[3]);
                    newData.emg4 = Convert.ToInt32(val[4]);
                    newData.emg5 = Convert.ToInt32(val[5]);
                    newData.emg6 = Convert.ToInt32(val[6]);
                    newData.emg7 = Convert.ToInt32(val[7]);
                    newData.emg8 = Convert.ToInt32(val[8]);
                    data.Add(newData);
                    i++; 
                }
            }

            return data;
        } 
    }
}

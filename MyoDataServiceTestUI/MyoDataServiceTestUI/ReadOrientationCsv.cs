﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyoDataServiceTestUI
{
    class ReadOrientationCsv
    {
        public string filepath { get; set; }


        public List<CtrlZDataService.MotionData> ReadData()
        {
            List<CtrlZDataService.MotionData> data = new List<CtrlZDataService.MotionData>();
            var reader = new StreamReader(File.OpenRead(filepath));
            int i = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                var val = values[0].Split(',');

                if (!val[0].Contains("timestamp"))
                {
                    CtrlZDataService.MotionData newData = new CtrlZDataService.MotionData();
                    newData.orderReceived = i;
                    newData.createdDate = DateTime.Now.Date;
                    newData.timestamp = Convert.ToInt64(val[0]);
                    newData.orientationX = Convert.ToDouble(val[1]);
                    newData.orientationY = Convert.ToDouble(val[2]);
                    newData.orientationZ = Convert.ToDouble(val[3]);
                    newData.orientationW = Convert.ToDouble(val[4]);
                    data.Add(newData);
                    i++;
                }
            }

            return data;
        }
    }
}

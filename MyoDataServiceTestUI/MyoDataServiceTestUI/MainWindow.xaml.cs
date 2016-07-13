using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.ServiceModel;
using CtrlZDataService;
using MyoSharp;

namespace MyoDataServiceTestUI
{

    public partial class MainWindow : Window
    {
        public string gyroCsvFilename;
        public string emgCsvFilename;
        public string accelCsvFilename;
        public string orientationCsvFilename;
        public string orientationEulerCsvFilename;

        public List<CtrlZDataService.EmgData> emgResults = new List<CtrlZDataService.EmgData>();
        public List<CtrlZDataService.MotionData> gyroResults = new List<CtrlZDataService.MotionData>();
        public List<CtrlZDataService.MotionData> accelResults = new List<CtrlZDataService.MotionData>();
        public List<CtrlZDataService.MotionData> orientationResults = new List<CtrlZDataService.MotionData>();
        public List<CtrlZDataService.MotionData> orientationEulerResults = new List<CtrlZDataService.MotionData>();
        public List<CtrlZDataService.MotionData> motionResults = new List<CtrlZDataService.MotionData>();

        public CtrlZDataService.Transaction transaction = new CtrlZDataService.Transaction();
        public CtrlZServiceClient client = new CtrlZServiceClient();

        Stopwatch transactionTimer = new Stopwatch();
        Stopwatch myoCaptureTimer = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
            transaction.createdDate = DateTime.Now.Date;
        }

        private void EmgButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                if (dlg.FileName.Contains("emg"))
                {
                    emgCsvFilename = dlg.FileName;
                    EmgLabel.Content = emgCsvFilename;
                    ReadEmgCsv newCsv = new ReadEmgCsv();
                    newCsv.filepath = emgCsvFilename;
                    emgResults = newCsv.ReadData();
                    Trace.WriteLine("EMG Count: " + emgResults.Count);
                }
                else
                {
                    EmgLabel.Content = "Please select the EMG file";
                }
            }
        }

        private void GyroButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                if (dlg.FileName.Contains("gyro"))
                {
                    gyroCsvFilename = dlg.FileName;
                    GyroLabel.Content = gyroCsvFilename;
                    ReadGyroCsv newCsv = new ReadGyroCsv();
                    newCsv.filepath = gyroCsvFilename;
                    gyroResults = newCsv.ReadData();
                    Trace.WriteLine("Gyro Count: " + gyroResults.Count);
                }
                else
                {
                    GyroLabel.Content = "Please select the Gyro file";
                }
            }
        }

        private void AccelButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                if (dlg.FileName.Contains("accelerometer"))
                {
                    accelCsvFilename = dlg.FileName;
                    AccelLabel.Content = accelCsvFilename;
                    ReadAccelerometerCsv newCsv = new ReadAccelerometerCsv();
                    newCsv.filepath = accelCsvFilename;
                    accelResults = newCsv.ReadData();
                    Trace.WriteLine("Accel Count: " + accelResults.Count);
                }
                else
                {
                    AccelLabel.Content = "Please select the Accelerometer file";
                }
            }
        }

        private void OrientationButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                if (dlg.FileName.Contains("orientation") && !dlg.FileName.Contains("Euler"))
                {
                    orientationCsvFilename = dlg.FileName;
                    OrientationLabel.Content = orientationCsvFilename;
                    ReadOrientationCsv newCsv = new ReadOrientationCsv();
                    newCsv.filepath = orientationCsvFilename;
                    orientationResults = newCsv.ReadData();
                    Trace.WriteLine("Orientation Count: " + orientationResults.Count);
                }
                else
                {
                    OrientationLabel.Content = "Please select the Orientation file";
                }
            }
        }

        private void OrientationEulerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                if (dlg.FileName.Contains("orientationEuler"))
                {
                    orientationEulerCsvFilename = dlg.FileName;
                    OrientationEulerLabel.Content = orientationEulerCsvFilename;
                    ReadOrientationEulerCsv newCsv = new ReadOrientationEulerCsv();
                    newCsv.filepath = orientationEulerCsvFilename;
                    orientationEulerResults = newCsv.ReadData();
                    Trace.WriteLine("Orientation Euler Count: " + orientationEulerResults.Count);
                }
                else
                {
                    OrientationEulerLabel.Content = "Please select the Orientation Euler file";
                }
            }
        }

        private void CombineButton_Click(object sender, RoutedEventArgs e)
        {
            InfoLabel.Visibility = Visibility.Visible;
            //  If the username or gesture is empty, then they are set to default values
            if (string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                transaction.username = "defaultUser";
                Trace.WriteLine(transaction.username);
            }
            else
            {
                transaction.username = UsernameTextBox.Text;
                Trace.WriteLine(transaction.username);
            }
            if (string.IsNullOrEmpty(GestureTextBox.Text))
            {
                transaction.gesture = "defaultGesture";
                Trace.WriteLine(transaction.gesture);
            }
            else
            {
                transaction.gesture = GestureTextBox.Text;
                Trace.WriteLine(transaction.gesture);
            }

            if (!String.IsNullOrEmpty(emgCsvFilename) && !String.IsNullOrEmpty(gyroCsvFilename) && !String.IsNullOrEmpty(accelCsvFilename) && !String.IsNullOrEmpty(orientationCsvFilename) && !String.IsNullOrEmpty(orientationEulerCsvFilename))
            {
                Thread tCombine = new Thread(new ThreadStart(CombineData));
                tCombine.Start();
            }
            else
            {
                MessageBox.Show("Make sure all files have been Selected", "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // This is for reading from .csv files only
        private void CombineData()
        {
            //  Combine the data from the motion data files
            for (var i = 0; i < gyroResults.Count; i++)
            {
                CtrlZDataService.MotionData mData = new CtrlZDataService.MotionData();
                mData.transactionId = transaction.id;
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
                mData.orientationRoll = orientationEulerResults.ElementAt(i).orientationRoll;
                mData.orientationPitch = orientationEulerResults.ElementAt(i).orientationPitch;
                mData.orientationYaw = orientationEulerResults.ElementAt(i).orientationYaw;
                mData.orderReceived = i;
                mData.timestamp = gyroResults.ElementAt(i).timestamp;
                mData.createdDate = DateTime.Now;

                //  Add the populated motion data object to the motion data list
                motionResults.Add(mData);
            }

            // Sends the data to the CtrlZDataService
            transaction.emgList = emgResults.ToArray();
            transaction.motionList = motionResults.ToArray();
            transactionTimer.Reset();
            transactionTimer.Start();
            var transactionResult = client.AddTransaction(transaction);
            Trace.WriteLine(transactionResult);
            transactionTimer.Stop();
            Trace.WriteLine(transactionTimer.Elapsed);
            InfoLabel.Dispatcher.BeginInvoke((Action) (() => InfoLabel.Content = "Complete"));
            TimeTakenLabel.Dispatcher.BeginInvoke((Action)(() => TimeTakenLabel.Content = transactionTimer.Elapsed));
        }

        private void MyoCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            Thread tMyoConnect = new Thread(new ThreadStart(StartMyoCapture));
            tMyoConnect.Start();

            myoCaptureTimer.Start();
            while (myoCaptureTimer.Elapsed <= TimeSpan.FromMilliseconds(5001))
            {
                CaptureTimeLabel.Content = myoCaptureTimer.Elapsed.Seconds;
                CaptureTimeLabel.Refresh();
            }
            // Wait for the Myo Data Capture Thread to finish
            tMyoConnect.Join();

            myoCaptureTimer.Stop();
            transactionTimer.Reset();

            GetUserAndGesture();

            transactionTimer.Start();

            //WriteXML();

            var transactionResult = client.AddTransaction(transaction);
            Console.WriteLine(transactionResult);
            transactionTimer.Stop();
            MyoTimeLabel.Content = transactionTimer.Elapsed;
        }

        private void GetUserAndGesture()
        {
            if (string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                transaction.username = "defaultUser";
                Console.WriteLine(transaction.username);
            }
            else
            {
                transaction.username = UsernameTextBox.Text;
                Console.WriteLine(transaction.username);
            }
            if (string.IsNullOrEmpty(GestureTextBox.Text))
            {
                transaction.gesture = "defaultGesture";
                Console.WriteLine(transaction.gesture);
            }
            else
            {
                transaction.gesture = GestureTextBox.Text;
                Console.WriteLine(transaction.gesture);
            }
        }
        private void StartMyoCapture()
        {
            MyoConnection myo = new MyoConnection();
            transaction = myo.CreateConnection();

            transaction.username = "defaultUser";
            transaction.gesture = "defaultGesture";

            Console.WriteLine("EMGList Count: " + transaction.emgList.Length);
            Console.WriteLine("MotionList Count: " + transaction.motionList.Length);
        }

        // Used for Testing the size of the XML produced by the app
        public void WriteXML()
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(CtrlZDataService.Transaction));

            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//SerializationOverview.xml";
            System.IO.FileStream file = System.IO.File.Create(path);

            writer.Serialize(file, transaction);
            file.Close();
        }
    }
}

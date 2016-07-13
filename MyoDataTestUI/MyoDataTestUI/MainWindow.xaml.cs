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
using MyoDataTest;

namespace MyoDataTestUI
{

    public partial class MainWindow : Window
    {
        public string gyroCsvFilename;
        public string emgCsvFilename;
        public string accelCsvFilename;
        public string orientationCsvFilename;
        public string orientationEulerCsvFilename;

        public List<EmgData> emgResults = new List<EmgData>();
        public List<MotionData> gyroResults = new List<MotionData>();
        public List<MotionData> accelResults = new List<MotionData>();
        public List<MotionData> orientationResults = new List<MotionData>();
        public List<MotionData> orientationEulerResults = new List<MotionData>();
        public List<MotionData> motionResults = new List<MotionData>();

        Transaction transaction = new Transaction();

        Stopwatch emgTimer = new Stopwatch();
        Stopwatch motionTimer = new Stopwatch();
        Stopwatch blkEmgTimer = new Stopwatch();
        Stopwatch blkMotionTimer = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
            transaction.createdDate = DateTime.Now;
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


            if(!String.IsNullOrEmpty(emgCsvFilename) && !String.IsNullOrEmpty(gyroCsvFilename) && !String.IsNullOrEmpty(accelCsvFilename) && !String.IsNullOrEmpty(orientationCsvFilename) && !String.IsNullOrEmpty(orientationEulerCsvFilename))
            {
                Thread tCreate = new Thread(new ThreadStart(CreateTransaction));
                tCreate.Start();

                Thread tCombine = new Thread(new ThreadStart(CombineData));
                tCombine.Start();
            }
            else
            {
                MessageBox.Show("Make sure all files have been Selected","File Error", MessageBoxButton.OK, MessageBoxImage.Error );
            }

            DatabaseButton.Visibility = Visibility.Visible;
        }

        private void CreateTransaction()
        {
            DataConnection connection = new DataConnection();
            transaction.id = connection.CreateTransaction(transaction.username, transaction.gesture, transaction.createdDate);
        }

        private void CombineData()
        {
            //  Add all of the EMG data to the transaction 
            transaction.emgList.AddRange(emgResults);
          
            //  Combine the data from the motion data files
            for (var i = 0; i < gyroResults.Count; i++)
            {
                MotionData mData = new MotionData();
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
                transaction.motionList.Add(mData);
                
            }

            //  Clear the lists after they have been used
            Thread tClear = new Thread(new ThreadStart(ClearLists));
            tClear.Start();
        }

        private void ClearLists()
        {
            emgResults.Clear();
            gyroResults.Clear();
            accelResults.Clear();
            orientationResults.Clear();
            orientationEulerResults.Clear();
            motionResults.Clear(); 
        }

        private void DatabaseButton_Click(object sender, RoutedEventArgs e)
        {

            Thread tBlkEmg = new Thread(new ThreadStart(StartBulkEmg));
            tBlkEmg.Start();

            Thread tBlkMotion = new Thread(new ThreadStart(StartBulkMotion));
            tBlkMotion.Start();

        }

        private void StartEmg()
        {
                   
            DataConnection connection = new DataConnection();
            Trace.WriteLine("Starting EMG!");
            emgTimer.Start();
            connection.InsertEmg(transaction);
            emgTimer.Stop();
            Trace.WriteLine("EMG Timer: " + emgTimer.Elapsed);
           
        }
        private void StartMotion()
        {
            DataConnection connection = new DataConnection();
            Trace.WriteLine("Starting Motion!");
            motionTimer.Start();
            connection.InsertMotion(transaction);
            motionTimer.Stop();
            Trace.WriteLine("Motion Timer: " + motionTimer.Elapsed);
        }

        private void StartBulkEmg()
        {
            DataConnection connection = new DataConnection();
            Trace.WriteLine("Starting Bulk EMG!");
            blkEmgTimer.Start();
            connection.BulkInsertEmg(transaction);
            blkEmgTimer.Stop();
            Trace.WriteLine("Bulk EMG Timer: " + blkEmgTimer.Elapsed);
        }

        private void StartBulkMotion()
        {
            DataConnection connection = new DataConnection();
            Trace.WriteLine("Starting Bulk Motion!");
            blkMotionTimer.Start();
            connection.BulkInsertMotion(transaction);
            blkMotionTimer.Stop();
            Trace.WriteLine("Bulk Motion Timer: " + blkMotionTimer.Elapsed);
        }
    }
}

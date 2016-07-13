using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
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

namespace MyoCaptureUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public CtrlZDataService.Transaction transaction = new CtrlZDataService.Transaction();
        public string currentUser = "defaultUser";
        Stopwatch myoCaptureTimer = new Stopwatch();
        public bool fistGestureDetected = false;
        public static bool detect = false;

        public MainWindow()
        {
            InitializeComponent();

            //Pattern alePass = null;
            //Pattern amjPass = null;
            //Pattern jusPass = null;
            //Pattern sanPass = null;
            //Pattern janPass = null;

            //  Replaced with the populateComboBox method 

            //User alejandra = new User("Alejandra Torres", alePass);
            //User amjad = new User("Amjad Alqahtani", amjPass);
            //User justin = new User("Justin Lucker", jusPass);
            //User sanket = new User("Sanket Dhamala", sanPass);
            //User jan = new User("Jan Van Nimwegen", janPass);

            //usersComboBox.Items.Add(alejandra);
            //usersComboBox.Items.Add(amjad);
            //usersComboBox.Items.Add(justin);
            //usersComboBox.Items.Add(sanket);
            //usersComboBox.Items.Add(jan);

            populateComboBox();
            //Thread tMyo = new Thread(new ThreadStart(detectFistGesture));
            //tMyo.Start();
        }


        private void usersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentUser = usersComboBox.SelectedValue.ToString();
        }

        private void populateComboBox()
        {
            CtrlZServiceClient client = new CtrlZServiceClient();
            var users = client.GetUsers();

            foreach (var user in users.users)
            {
                User newUser = new User();
                newUser.name = user.username;
                usersComboBox.Items.Add(newUser);

            }
        }

        private void detectFistGesture()
        {
            MyoConnection myo1 = new MyoConnection();
            myoStatusLabel.Dispatcher.BeginInvoke((Action)(() => myoStatusLabel.Content = "CONNECTED"));
            fistGestureDetected = myo1.DetectFistGesture();
            if (fistGestureDetected)
            {
                myoStatusLabel.Dispatcher.BeginInvoke((Action)(() => myoStatusLabel.Content = "GESTURE - FIST"));
                StartCapture();
            }
        }

        //private void ConnectToMyo()
        //{
        //    MyoConnection myo1 = new MyoConnection();
        //    transaction = myo1.ConnectMyo();
        //    test();
        //}
        private void tempButton_Click(object sender, RoutedEventArgs e)
        {
            CtrlZServiceClient client = new CtrlZServiceClient();
            statusLabel.Content = "CAPTURING";

            Thread tMyoConnect = new Thread(new ThreadStart(StartMyoCapture));
            tMyoConnect.Start();

            myoCaptureTimer.Start();

            while (myoCaptureTimer.Elapsed <= TimeSpan.FromMilliseconds(5001))
            {
                gestureProgressBar.Value = myoCaptureTimer.Elapsed.TotalMilliseconds;
                gestureProgressBar.Refresh();
            }

            // Wait for the Myo Data Capture Thread to finish
            tMyoConnect.Join();

            myoCaptureTimer.Stop();
            statusLabel.Content = "SENDING";
            var transactionResult = client.AddTransaction(transaction);
            statusLabel.Content = "SUCCESS ";
        }

        private void test()
        {
            CtrlZServiceClient client = new CtrlZServiceClient();
            statusLabel.Content = "CAPTURING";

            myoCaptureTimer.Start();

            while (myoCaptureTimer.Elapsed <= TimeSpan.FromMilliseconds(5001))
            {
                gestureProgressBar.Value = myoCaptureTimer.Elapsed.TotalMilliseconds;
                gestureProgressBar.Refresh();
            }
            myoCaptureTimer.Stop();
            statusLabel.Content = "SENDING";
            var transactionResult = client.AddTransaction(transaction);
            statusLabel.Content = "SUCCESS ";
        }
        private void StartCapture()
        {
            CtrlZServiceClient client = new CtrlZServiceClient();
            statusLabel.Dispatcher.BeginInvoke((Action)(() => statusLabel.Content = "CAPTURING"));

            //Thread tMyoConnect = new Thread(new ThreadStart(StartMyoCapture));
            //tMyoConnect.Start();

            myoCaptureTimer.Start();

            while (myoCaptureTimer.Elapsed <= TimeSpan.FromMilliseconds(5001))
            {
                Thread.Sleep(10);
                gestureProgressBar.Dispatcher.BeginInvoke(
                    (Action) (() => gestureProgressBar.Value = myoCaptureTimer.Elapsed.TotalMilliseconds));
                gestureProgressBar.Dispatcher.BeginInvoke(
                   (Action)(() => gestureProgressBar.Refresh()));
            }

            // Wait for the Myo Data Capture Thread to finish
            //tMyoConnect.Join();

            myoCaptureTimer.Stop();
            statusLabel.Dispatcher.BeginInvoke((Action)(() => statusLabel.Content = "SENDING"));
            //var transactionResult = client.AddTransaction(transaction);
            statusLabel.Dispatcher.BeginInvoke((Action)(() => statusLabel.Content = "SUCCESS"));
        }

        private void StartMyoCapture()
        {
            MyoConnection myo2 = new MyoConnection();
            transaction = myo2.CreateConnection();

            if (string.IsNullOrEmpty(currentUser))
            {
                transaction.username = currentUser;
                transaction.gesture = "Close Bracket";
            }
            else
            {
                transaction.username = "defaultUser";
                transaction.gesture = "Close Bracket";
            }
        }
    }
}

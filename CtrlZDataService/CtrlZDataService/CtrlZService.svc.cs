
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Threading;

namespace CtrlZDataService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class CtrlZService : ICtrlZService
    {
        Transaction transaction = new Transaction();
        private bool usernameTaken = false;
        Stopwatch blkEmgTimer = new Stopwatch();
        Stopwatch blkMotionTimer = new Stopwatch();

        public bool AddTransaction(Transaction t1)
        {
           
            transaction = t1;
            InitialTransaction();

            Trace.WriteLine("EMG Data: " + transaction.emgList.Count);
            Trace.WriteLine("Motion Data: " + transaction.motionList.Count);

            foreach (var emg in transaction.emgList)
            {
                emg.transactionId = transaction.id;
            }

            foreach (var motion in transaction.motionList)
            {
                motion.transactionId = transaction.id;
            }

            Thread tBulkEmg = new Thread(new ThreadStart(StartBulkEmg));
            tBulkEmg.Start();

            Thread tBulkMotion = new Thread(new ThreadStart(StartBulkMotion));
            tBulkMotion.Start();

            return CompareData();
        }

        private void InitialTransaction()
        {
            transaction.createdDate = DateTime.Now.Date;
            DataConnection connection = new DataConnection();
            transaction.id = connection.CreateTransaction(transaction.username, transaction.gesture, transaction.createdDate);
            Trace.WriteLine(transaction.id + " " + transaction.username + " " + transaction.gesture + " " + transaction.createdDate);
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

        public Users GetUsers()
        {
            DataConnection connection = new DataConnection();
            var users = connection.GetUsers();

            return users;
        }

        public bool CheckUsername(string s1)
        {
            DataConnection connection = new DataConnection();
            var result = connection.CheckUsername(s1);

            return result;
        }

        private bool CompareData()
        {
            return true;
        }
    }
}
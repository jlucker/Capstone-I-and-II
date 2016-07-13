using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using MyoDataTest;

namespace MyoDataTestUI
{
    class DataConnection
    {
        public Guid id = Guid.Empty;
        private string connectionString = "Server=tcp:ctrlzapp.database.windows.net,1433;Database=CtrlzAppDatabase;User ID=ctrlzadmin@ctrlzapp;Password=BootsFennecTariffHexose#6;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        public Guid CreateTransaction(string username, string gesture, DateTime createdDate)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                var cmd = connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO TransactionData (Username, Gesture, CreatedDate)
                                    OUTPUT INSERTED.ID
                                    VALUES(@username, @gesture, @createdDate)";

                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@gesture", gesture);
                cmd.Parameters.AddWithValue("@createdDate", createdDate.Date);

                connection.Open();

                id = (Guid) cmd.ExecuteScalar();
                Trace.WriteLine("New Tranactions ID: " + id);
                return id;

            }
        }

        // Insert EMG Strategy that opens a new connection every time (For Example Only - Not Used)
        public Guid InsertEmg(Guid transactionId, int emg1, int emg2, int emg3, int emg4, int emg5, int emg6, int emg7, int emg8, int orderReceived, long timestamp, DateTime createdDate)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO EMGData (TransactionID, EMG1, EMG2, EMG3, EMG4, EMG5, EMG6, EMG7, EMG8, OrderReceived, Timestamp, CreatedDate)
                                    OUTPUT INSERTED.ID
                                    VALUES(@transactionId, @emg1, @emg2, @emg3, @emg4, @emg5, @emg6, @emg7, @emg8, @orderReceived, @timestamp, @createdDate)";
                Trace.WriteLine(transactionId);
                cmd.Parameters.AddWithValue("@transactionId", transactionId);
                cmd.Parameters.AddWithValue("@emg1", emg1);
                cmd.Parameters.AddWithValue("@emg2", emg2);
                cmd.Parameters.AddWithValue("@emg3", emg3);
                cmd.Parameters.AddWithValue("@emg4", emg4);
                cmd.Parameters.AddWithValue("@emg5", emg5);
                cmd.Parameters.AddWithValue("@emg6", emg6);
                cmd.Parameters.AddWithValue("@emg7", emg7);
                cmd.Parameters.AddWithValue("@emg8", emg8);
                cmd.Parameters.AddWithValue("@orderReceived", orderReceived);
                cmd.Parameters.AddWithValue("@timestamp", timestamp);
                cmd.Parameters.AddWithValue("@createdDate", createdDate.Date);
                Trace.WriteLine(cmd.CommandText);
                connection.Open();
                //Trace.WriteLine("New EMG ID: " + (Guid)cmd.ExecuteScalar());
                return (Guid)cmd.ExecuteScalar();
            }
        }

        // Insert EMG Strategy that opens a new connection every time (For Example Only - Not Used)
        public void InsertEmg(Transaction transaction)
        {
            foreach (var t in transaction.emgList)
            {
                using (var connection = new SqlConnection(connectionString))
                {

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"INSERT INTO EMGData (TransactionID, EMG1, EMG2, EMG3, EMG4, EMG5, EMG6, EMG7, EMG8, OrderReceived, Timestamp, CreatedDate)
                                    OUTPUT INSERTED.ID
                                    VALUES(@transactionId, @emg1, @emg2, @emg3, @emg4, @emg5, @emg6, @emg7, @emg8, @orderReceived, @timestamp, @createdDate)";

                    cmd.Parameters.AddWithValue("@transactionId", transaction.id);
                    cmd.Parameters.AddWithValue("@emg1", t.emg1);
                    cmd.Parameters.AddWithValue("@emg2", t.emg2);
                    cmd.Parameters.AddWithValue("@emg3", t.emg3);
                    cmd.Parameters.AddWithValue("@emg4", t.emg4);
                    cmd.Parameters.AddWithValue("@emg5", t.emg5);
                    cmd.Parameters.AddWithValue("@emg6", t.emg6);
                    cmd.Parameters.AddWithValue("@emg7", t.emg7);
                    cmd.Parameters.AddWithValue("@emg8", t.emg8);
                    cmd.Parameters.AddWithValue("@orderReceived", t.orderReceived);
                    cmd.Parameters.AddWithValue("@timestamp", t.timestamp);
                    cmd.Parameters.AddWithValue("@createdDate", t.createdDate.Date);

                    connection.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //  Uses SQL Bulk Copy to copy the EMG data to the SQL Server very quickly
        public void BulkInsertEmg(Transaction transaction)
        {
            foreach (var emg in transaction.emgList)
            {
                emg.transactionId = transaction.id;
                emg.createdDate = emg.createdDate.Date;
            }

            ConvertToDataTable converter = new ConvertToDataTable();
            DataTable dataTable = converter.ToDataTable(transaction.emgList);

            using (SqlBulkCopy s = new SqlBulkCopy(connectionString))
            {
                s.DestinationTableName = "EMGData";
                s.NotifyAfter = 10000;

                s.ColumnMappings.Add("transactionId", "TransactionID");
                s.ColumnMappings.Add("emg1", "EMG1");
                s.ColumnMappings.Add("emg2", "EMG2");
                s.ColumnMappings.Add("emg3", "EMG3");
                s.ColumnMappings.Add("emg4", "EMG4");
                s.ColumnMappings.Add("emg5", "EMG5");
                s.ColumnMappings.Add("emg6", "EMG6");
                s.ColumnMappings.Add("emg7", "EMG7");
                s.ColumnMappings.Add("emg8", "EMG8");
                s.ColumnMappings.Add("orderReceived", "OrderReceived");
                s.ColumnMappings.Add("timestamp", "Timestamp");
                s.ColumnMappings.Add("createdDate", "CreatedDate");

                s.WriteToServer(dataTable);
                s.Close();
            }
        }


        // Insert Motion Strategy that opens a new connection every time (For Example Only - Not Used)
        public bool InsertMotion(Guid transactionId, double gyroX, double gyroY, double gyroZ, double accelX, double accelY, double accelZ, double orientationX, double orientationY, double orientationZ, double orientationW,
                                 double orientationRoll, double orientationPitch, double orientationYaw, int orderReceived, long timestamp, DateTime createdDate)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                var cmd = connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO MotionData (TransactionID, GyroX, GyroY, GyroZ, AccelX, AccelY, AccelZ, OrientationX, OrientationY, OrientationZ, OrientationW,
                                                            OreintationRoll, OrientationPitch, OrientationYaw, OrderReceived, Timestamp, CreatedDate)
                                    VALUES(@transactionId, @gyroX, @gyroY, @gyroZ, @accelX, @accelY, @accelZ, @orientationX, @orientationY, @orientationZ, @orientationW, 
                                           @orientationRoll, @orientationPitch, @orientationYaw, @orderedReceived, @timestamp, @createdDate)";

                cmd.Parameters.AddWithValue("@transactionId", transactionId);
                cmd.Parameters.AddWithValue("@gyroX", gyroX);
                cmd.Parameters.AddWithValue("@gyroY", gyroY);
                cmd.Parameters.AddWithValue("@gyroZ", gyroZ);
                cmd.Parameters.AddWithValue("@accelX", accelX);
                cmd.Parameters.AddWithValue("@accelY", accelY);
                cmd.Parameters.AddWithValue("@accelZ", accelZ);
                cmd.Parameters.AddWithValue("@orientationX", orientationX);
                cmd.Parameters.AddWithValue("@orientationY", orientationY);
                cmd.Parameters.AddWithValue("@orientationZ", orientationZ);
                cmd.Parameters.AddWithValue("@orientationW", orientationW);
                cmd.Parameters.AddWithValue("@orientationPitch", orientationRoll);
                cmd.Parameters.AddWithValue("@orientationRoll", orientationPitch);
                cmd.Parameters.AddWithValue("@orientationYaw", orientationYaw);
                cmd.Parameters.AddWithValue("@orderReceived", orderReceived);
                cmd.Parameters.AddWithValue("@timestamp", timestamp);
                cmd.Parameters.AddWithValue("@createdDate", createdDate.Date);

                connection.Open();

                return true;
            }
        }

        // Insert Motion Strategy that opens a new connection every time (For Example Only - Not Used)
        public void InsertMotion(Transaction transaction)
        {
            foreach (var t in transaction.motionList)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"INSERT INTO MotionData (TransactionID, GyroX, GyroY, GyroZ, AccelX, AccelY, AccelZ, OrientationX, OrientationY, OrientationZ, OrientationW,
                                                            OrientationRoll, OrientationPitch, OrientationYaw, OrderReceived, Timestamp, CreatedDate)
                                    VALUES(@transactionId, @gyroX, @gyroY, @gyroZ, @accelX, @accelY, @accelZ, @orientationX, @orientationY, @orientationZ, @orientationW, 
                                           @orientationRoll, @orientationPitch, @orientationYaw, @orderReceived, @timestamp, @createdDate)";

                    cmd.Parameters.AddWithValue("@transactionId", transaction.id);
                    cmd.Parameters.AddWithValue("@gyroX", t.gyroX);
                    cmd.Parameters.AddWithValue("@gyroY", t.gyroY);
                    cmd.Parameters.AddWithValue("@gyroZ", t.gyroZ);
                    cmd.Parameters.AddWithValue("@accelX", t.accelX);
                    cmd.Parameters.AddWithValue("@accelY", t.accelY);
                    cmd.Parameters.AddWithValue("@accelZ", t.accelZ);
                    cmd.Parameters.AddWithValue("@orientationX", t.orientationX);
                    cmd.Parameters.AddWithValue("@orientationY", t.orientationY);
                    cmd.Parameters.AddWithValue("@orientationZ", t.orientationZ);
                    cmd.Parameters.AddWithValue("@orientationW", t.orientationW);
                    cmd.Parameters.AddWithValue("@orientationPitch", t.orientationRoll);
                    cmd.Parameters.AddWithValue("@orientationRoll", t.orientationPitch);
                    cmd.Parameters.AddWithValue("@orientationYaw", t.orientationYaw);
                    cmd.Parameters.AddWithValue("@orderReceived", t.orderReceived);
                    cmd.Parameters.AddWithValue("@timestamp", t.timestamp);
                    cmd.Parameters.AddWithValue("@createdDate", t.createdDate.Date);

                    connection.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //  Uses SQL Bulk Copy to copy the motion data to the SQL Server very quickly
        public void BulkInsertMotion(Transaction transaction)
        {
            foreach (var motion in transaction.motionList)
            {
                motion.transactionId = transaction.id;
                motion.createdDate = motion.createdDate.Date;
            }

            ConvertToDataTable converter = new ConvertToDataTable();
            DataTable dataTable = converter.ToDataTable(transaction.motionList);

            using (SqlBulkCopy s = new SqlBulkCopy(connectionString))
            {
                s.DestinationTableName = "MotionData";
                s.NotifyAfter = 10000;

                s.ColumnMappings.Add("transactionId", "TransactionID");
                s.ColumnMappings.Add("gyroX", "GyroX");
                s.ColumnMappings.Add("gyroY", "GyroY");
                s.ColumnMappings.Add("gyroZ", "GyroZ");
                s.ColumnMappings.Add("accelX", "AccelX");
                s.ColumnMappings.Add("accelY", "AccelY");
                s.ColumnMappings.Add("accelZ", "AccelZ");
                s.ColumnMappings.Add("orientationX", "OrientationX");
                s.ColumnMappings.Add("orientationY", "OrientationY");
                s.ColumnMappings.Add("orientationZ", "OrientationZ");
                s.ColumnMappings.Add("orientationW", "OrientationW");
                s.ColumnMappings.Add("orientationRoll", "OrientationRoll");
                s.ColumnMappings.Add("orientationPitch", "OrientationPitch");
                s.ColumnMappings.Add("orientationYaw", "OrientationYaw");
                s.ColumnMappings.Add("orderReceived", "OrderReceived");
                s.ColumnMappings.Add("timestamp", "Timestamp");
                s.ColumnMappings.Add("createdDate", "CreatedDate");

                s.WriteToServer(dataTable);
                s.Close();
            }
        }
    }
}

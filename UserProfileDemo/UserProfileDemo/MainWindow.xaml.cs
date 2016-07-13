using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace UserProfileDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString = "Server=tcp:ctrlzapp.database.windows.net,1433;Database=CtrlzAppDatabase;User ID=ctrlzadmin@ctrlzapp;Password=BootsFennecTariffHexose#6;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private string imageLocation = System.Reflection.Assembly.GetExecutingAssembly().Location + "\\..\\..\\DefaultImage.png";
        private string username;
        private ImageSource image;
        private bool usernameTaken = false;
        private List<User> userList = new List<User>();

        public MainWindow()
        {
            InitializeComponent();
            // Loads the Default Image as a place holder in the Main Window
            ProfileImage.Source = new BitmapImage(new Uri("DefaultImage.png", UriKind.Relative));
            UsernameTextbox.MaxLength = 100;
            PopulateUsersGrid();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextbox.Text))
            {
                username = "defaultUser";
            }
            else
            {
                username = UsernameTextbox.Text;
            }

            try
            {
                byte[] image = null;
                FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                image = br.ReadBytes((int) fs.Length);

                User user = new User();
                user.username = username;
                user.image = image;

                if (usernameTaken == false)
                {
                    var newUser = CreateNewUser(user);
                    if (newUser)
                    {
                        MessageBox.Show("New user has been created!");
                        reset();
                    }
                }
                else
                {
                    MessageBox.Show("Username is taken!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            PopulateUsersGrid();
        }

        private void reset()
        {
            ProfileImage.Source = new BitmapImage(new Uri("DefaultImage.png", UriKind.Relative));
            imageLocation = string.Empty;
            username = string.Empty;
            usernameTaken = false;
            CheckUsernameLabel.Content = string.Empty;
            UsernameTextbox.Text = string.Empty;
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();

                dialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + 
                                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + 
                                "Portable Network Graphic (*.png)|*.png";

                dialog.Title = "Select Your Profile Image";
                if (dialog.ShowDialog() == true)
                {
                    imageLocation = dialog.FileName.ToString();
                    ProfileImage.Source = new BitmapImage(new Uri(imageLocation));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void UsernameTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            username = UsernameTextbox.Text;
            Thread tCheck = new Thread(new ThreadStart(CheckUsername));
            tCheck.Start();
        }

        private void CheckUsername()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"SELECT COUNT(1) FROM UserData WHERE Username = @username";
                cmd.Parameters.AddWithValue("@username", username);

                connection.Open();

                var result = (int)cmd.ExecuteScalar();

                // Makes a checkmark if the name is not taken (unicode checkmark)
                if (result == 0)
                {
                    usernameTaken = false;
                    CheckUsernameLabel.Dispatcher.BeginInvoke((Action)(() => CheckUsernameLabel.Content = "\u2714"));
                }

                // Makes an x-mark if the name is taken (unicode x-mark)
                else
                {
                    usernameTaken = true;
                    CheckUsernameLabel.Dispatcher.BeginInvoke((Action)(() => CheckUsernameLabel.Content = "\u2716"));
                }

                // Removes the indicator when the text box is empty
                if (string.IsNullOrWhiteSpace(username))
                {
                    usernameTaken = false;
                    CheckUsernameLabel.Dispatcher.BeginInvoke((Action)(() => CheckUsernameLabel.Content = string.Empty));
                }
            }
        }

        private void PopulateUsersGrid()
        {
            userList.Clear();

            using (var connection = new SqlConnection(connectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"SELECT Username, Image FROM UserData ORDER BY Username Asc";
                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User();
                        user.username = reader["Username"].ToString();
                        user.image = (byte[])reader["Image"];
                        user.byteArrayToImage();
                        userList.Add(user);
                    }
                }
            }

            image0.Source = userList.ElementAt(0).formattedImage;
            label0.Content = userList.ElementAt(0).username;

            image1.Source = userList.ElementAt(1).formattedImage;
            label1.Content = userList.ElementAt(1).username;

            image2.Source = userList.ElementAt(2).formattedImage;
            label2.Content = userList.ElementAt(2).username;

            image3.Source = userList.ElementAt(3).formattedImage;
            label3.Content = userList.ElementAt(3).username;

            image4.Source = userList.ElementAt(4).formattedImage;
            label4.Content = userList.ElementAt(4).username;
        }

        private bool CreateNewUser(User user)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                var cmd = connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO UserData (Username, Image, CreatedDate)
                                    OUTPUT INSERTED.ID
                                    VALUES(@username, @image, @date)";

                cmd.Parameters.AddWithValue("@username", user.username);
                cmd.Parameters.AddWithValue("@image", user.image);
                cmd.Parameters.AddWithValue("@date", DateTime.Now.Date);

                connection.Open();

                var id = (Guid)cmd.ExecuteScalar();
                Trace.WriteLine("New Tranactions ID: " + id);

                return true;
            }
        }
    }
}

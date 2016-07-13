using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UserProfileDemo
{
    class User
    {
        public string username;
        public byte[] image;
        public ImageSource formattedImage;

        public void byteArrayToImage()
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(image);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            formattedImage = biImg as ImageSource;
        }
    }
}

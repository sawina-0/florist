using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using ZXing;
using ZXing.Common;
using System.IO;
using florist.AppData;

namespace florist.MainPage
{
    /// <summary>
    /// Логика взаимодействия для QRpage.xaml
    /// </summary>
    public partial class QRpage : Page
    {

        public QRpage()
        {
            InitializeComponent();
            LoadQR();
        }
        private void LoadQR()
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 300
                }
            };
            var result = writer.Write(@"https://dzen.ru/a/aDLAn-R_eloBFF1G");
            var bitmap = new BitmapImage();
            using (var memoryStream = new MemoryStream())
            {
                result.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Position = 0;
                bitmap.BeginInit();
                bitmap.StreamSource = memoryStream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
            }
            imgQr.Source = bitmap;
        }

        private void btBack_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.FrameMain.Navigate(new MainPage());

        }
    }
}

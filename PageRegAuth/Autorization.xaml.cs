using florist.AppData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace florist.PageRegAuth
{
    /// <summary>
    /// Логика взаимодействия для Autorization.xaml
    /// </summary>
    public partial class Autorization : Page
    {
        private static user user = null;
        public Autorization()
        {
            InitializeComponent();
        }

        private void btEnter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                var UserObj = AppConnect.Model.user
                    .Include(u => u.role)
                    .FirstOrDefault(x => x.login == tbLogin.Text && x.password == tbPass.Password);
                if (UserObj == null)
                {
                    MessageBox.Show("Такого пользователя нет", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    tbLogin.Text = "";
                    tbPass.Password = "";
                    user = UserObj;
                    MainFrame.FrameMain.Navigate(new MainPage.MainPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка " + ex.Message.ToString(), "programm RIP, try later(", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static string getUserRole()
        {
            if (user == null || user.role == null)
            {
                return null;
            }
            return user.role.roleName;


            //string role = Autorization.getUserRole();
            //if (role == "admin")
            //{
            //}
        }
        
        private void btReg_Click(object sender, RoutedEventArgs e)
        {
            tbLogin.Text = "";
            tbPass.Password = "";
            MainFrame.FrameMain.Navigate(new Registration());
        }
    }
}

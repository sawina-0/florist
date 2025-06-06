using florist.AppData;
using System;
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

namespace florist.PageRegAuth
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void btAuth_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.FrameMain.Navigate(new Autorization());
        }
        private void ResetInputContent()
        {
            tbRegLog.Text = null;
            tbRegName.Text = null;
            tbRegSur.Text = null;
            tbRegPass.Password = null;
            tbRegPassAg.Password = null;
        }

        private void tbRegPassAg_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (tbRegPassAg.Password != tbRegPass.Password)
            {
                btReg.IsEnabled = false;
                tbRegPassAg.Background = Brushes.LightCoral;
                tbRegPassAg.BorderBrush = Brushes.Red;
            }
            else
            {
                btReg.IsEnabled = true;
                tbRegPassAg.Background = Brushes.LightGreen;
                tbRegPassAg.BorderBrush = Brushes.Green;
            }
        }

        private void tbRegPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (tbRegPass.Password != tbRegPassAg.Password)
            {
                btReg.IsEnabled = false;
                tbRegPassAg.Background = Brushes.LightCoral;
                tbRegPassAg.BorderBrush = Brushes.Red;
            }
            else
            {
                btReg.IsEnabled = true;
                tbRegPassAg.Background = Brushes.LightGreen;
                tbRegPassAg.BorderBrush = Brushes.Green;
            }
        }

        private void btReg_Click(object sender, RoutedEventArgs e)
        {
            if(AppConnect.Model.user.Count(x => x.login == tbRegLog.Text) > 0)
            {
                MessageBox.Show("Пользователь с таким логином уже есть!");
                return;
            }
            if (tbRegPass.Password.Length < 8 || tbRegPass.Password.Length > 15 || tbRegPassAg.Password.Length < 8 || tbRegPassAg.Password.Length > 15 )
            {
                MessageBox.Show("Пароль должен быть длинее 8 символов, но не длиннее 15!");
                return;
            }
            if (string.IsNullOrWhiteSpace(tbRegPass.Password) || string.IsNullOrWhiteSpace(tbRegLog.Text) || string.IsNullOrWhiteSpace(tbRegPassAg.Password) || string.IsNullOrWhiteSpace(tbRegName.Text) || string.IsNullOrWhiteSpace(tbRegSur.Text))
            {
                MessageBox.Show("Пожалуйста заполните все поля");
                return;
            }
            try
            {
                user userObj = new user()
                {
                    login = tbRegLog.Text,
                    name = tbRegName.Text,
                    password = tbRegPass.Password,
                    surname = tbRegSur.Text,
                    roleID = 1
                };
                AppConnect.Model.user.Add(userObj);
                AppConnect.Model.SaveChanges();
                MessageBox.Show("Данные успешно добавлены!");
                ResetInputContent();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении данных" + ex.Message);
            }
        }
    }
}

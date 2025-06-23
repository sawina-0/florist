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

namespace florist.PageAddEdit
{
    /// <summary>
    /// Логика взаимодействия для AddFlowerPage.xaml
    /// </summary>
    public partial class AddFlowerPage : Page
    {
        private flowers _newFlower = new flowers();
        private List<type> _flowerTypes;
        private List<color> _colors;
        private List<supplier> _suppliers;
        private List<blossomSize> _sizes;
        public AddFlowerPage()
        {
            InitializeComponent();
            Loaded += AddFlowerPage_Loaded;
        }
        private void AddFlowerPage_Loaded(object sender, RoutedEventArgs e)
        {
            _flowerTypes = AppConnect.Model.type.ToList();
            _colors = AppConnect.Model.color.ToList();
            _suppliers = AppConnect.Model.supplier.ToList();
            _sizes = AppConnect.Model.blossomSize.ToList();

            cbType.ItemsSource = _flowerTypes;
            cbType.DisplayMemberPath = "type1";
            cbType.SelectedValuePath = "typeID";

            cbColor.ItemsSource = _colors;
            cbColor.DisplayMemberPath = "color1";
            cbColor.SelectedValuePath = "colorID";

            cbSupplier.ItemsSource = _suppliers;
            cbSupplier.DisplayMemberPath = "companyName";
            cbSupplier.SelectedValuePath = "supplierID";

            cbSize.ItemsSource = _sizes;
            cbSize.DisplayMemberPath = "size";
            cbSize.SelectedValuePath = "sizeID";
        }
        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string fileName = ImageHelper.CopyImageToProject(dialog.FileName);
                    _newFlower.img = fileName;
                    imgPreview.Source = new BitmapImage(ImageHelper.GetImageUri(fileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {

            if (cbType.SelectedValue == null || cbColor.SelectedValue == null ||
                cbSupplier.SelectedValue == null || cbSize.SelectedValue == null || !decimal.TryParse(tbPrice.Text, out decimal price))
            {
                MessageBox.Show("Заполните все поля корректно!");
                return;
            }
            if (price <= 0)
            {
                MessageBox.Show("Цена должна быть положительным числом!");
                return;
            }

            // Проверка на максимальное значение decimal(8,2)
            if (price > 999999.99m)
            {
                MessageBox.Show("Цена не может превышать 999 999.99");
                return;
            }
            try
            {
                var newFlower = new flowers()
                {
                    typeID = (int)cbType.SelectedValue,
                    colorID = (int)cbColor.SelectedValue,
                    sizeID = (int)cbSize.SelectedValue,
                    supplierID = (int)cbSupplier.SelectedValue,
                    price = price,
                    img = _newFlower.img // сохраняем имя файла изображения
                };

                // Начинаем транзакцию
                AppConnect.Model.flowers.Add(newFlower);
                AppConnect.Model.SaveChanges();

                MessageBox.Show("Цветок успешно добавлен!");
                MainFrame.FrameMain.Navigate(new MainPage.MainPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        private void btBack_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.FrameMain.Navigate(new MainPage.MainPage());
        }
    }
}

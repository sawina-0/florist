using florist.AppData;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.IO;
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
    /// Логика взаимодействия для EditFlowerPage.xaml
    /// </summary>
    public partial class EditFlowerPage : Page
    {
        private flowers _currentFlower;
        private List<type> _flowerTypes;
        private List<color> _colors;
        private List<supplier> _suppliers;
        private List<blossomSize> _sizes;
        public EditFlowerPage(flowers flower = null)
        {
            if (flower == null) throw new ArgumentNullException(nameof(flower));

            InitializeComponent();
            _currentFlower = flower ?? new flowers();
            Loaded += EditFlowerPage_Loaded;
        }
        private void EditFlowerPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Загрузка данных
            _flowerTypes = AppConnect.Model.type.ToList();
            _colors = AppConnect.Model.color.ToList();
            _suppliers = AppConnect.Model.supplier.ToList();
            _sizes = AppConnect.Model.blossomSize.ToList();

            // Настройка ComboBox
            cbType.ItemsSource = _flowerTypes;
            cbType.DisplayMemberPath = "type1";
            cbType.SelectedValuePath = "typeID";
            cbType.SelectedValue = _currentFlower.typeID;

            cbColor.ItemsSource = _colors;
            cbColor.DisplayMemberPath = "color1";
            cbColor.SelectedValuePath = "colorID";
            cbColor.SelectedValue = _currentFlower.colorID;

            cbSupplier.ItemsSource = _suppliers;
            cbSupplier.DisplayMemberPath = "companyName";
            cbSupplier.SelectedValuePath = "supplierID";
            cbSupplier.SelectedValue = _currentFlower.supplierID;

            cbSize.ItemsSource = _sizes;
            cbSize.DisplayMemberPath = "size";
            cbSize.SelectedValuePath = "sizeID";
            cbSize.SelectedValue = _currentFlower.sizeID;

            tbPrice.Text = _currentFlower.price.ToString();

            // Загрузка изображения
            if (!string.IsNullOrEmpty(_currentFlower.img))
            {
                var imageUri = ImageHelper.GetImageUri(_currentFlower.img);
                if (imageUri != null)
                {
                    imgPreview.Source = new BitmapImage(imageUri);
                }
            }
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
                    _currentFlower.img = fileName;
                    imgPreview.Source = new BitmapImage(ImageHelper.GetImageUri(fileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        
        private void btBack_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.FrameMain.Navigate(new MainPage.MainPage());
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {

            if (cbType.SelectedValue == null || cbColor.SelectedValue == null ||
                cbSupplier.SelectedValue == null || cbSize.SelectedValue == null || !decimal.TryParse(tbPrice.Text, out decimal price))
            {
                MessageBox.Show("Заполните все поля корректно!");
                return;
            }

            try
            {
                _currentFlower.typeID = (int)cbType.SelectedValue;
                _currentFlower.colorID = (int)cbColor.SelectedValue;
                _currentFlower.sizeID = (int)cbSize.SelectedValue;
                _currentFlower.supplierID = (int)cbSupplier.SelectedValue;
                _currentFlower.price = price;

                AppConnect.Model.Entry(_currentFlower).State = EntityState.Modified;
                AppConnect.Model.SaveChanges();

                MessageBox.Show("Изменения сохранены!");
                MainFrame.FrameMain.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }

        }
    }
}

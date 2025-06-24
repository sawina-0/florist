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
    /// Логика взаимодействия для AddBouquetPage.xaml
    /// </summary>
    public partial class AddBouquetPage : Page
    {
        private List<flowers> _allFlowers;
        private List<flowers> _selectedFlowers = new List<flowers>();
        public AddBouquetPage()
        {
            InitializeComponent();
            Loaded += AddBouquetPage_Loaded;
        }
        private void AddBouquetPage_Loaded(object sender, RoutedEventArgs e)
        {
            _allFlowers = AppConnect.Model.flowers
                .Include("type")
                .Include("color")
                .ToList();

            lvAllFlowers.ItemsSource = _allFlowers;
            lvSelectedFlowers.ItemsSource = _selectedFlowers;
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
                    imgPreview.Source = new BitmapImage(ImageHelper.GetImageUri(fileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void AddFlower_Click(object sender, RoutedEventArgs e)
        {
            if (lvAllFlowers.SelectedItem is flowers selected && !_selectedFlowers.Contains(selected))
            {
                _selectedFlowers.Add(selected);
                lvSelectedFlowers.Items.Refresh();
            }
        }

        private void RemoveFlower_Click(object sender, RoutedEventArgs e)
        {
            if (lvSelectedFlowers.SelectedItem is flowers selected)
            {
                _selectedFlowers.Remove(selected);
                lvSelectedFlowers.Items.Refresh();
            }
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text) ||
                !decimal.TryParse(tbPrice.Text, out decimal price) ||
                _selectedFlowers.Count == 0)
            {
                MessageBox.Show("Заполните название, цену и выберите хотя бы один цветок!");
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
            if( tbName.Text.Length > 30)
            {
                MessageBox.Show("Максимальная длина названия 30 символов");
                return;
            }
            try
            {
                var newBouquet = new bouquet
                {
                    name = tbName.Text,
                    price = price,
                    img = System.IO.Path.GetFileName(((BitmapImage)imgPreview.Source)?.UriSource.LocalPath)
                };

                AppConnect.Model.bouquet.Add(newBouquet);
                AppConnect.Model.SaveChanges();

                foreach (var flower in _selectedFlowers)
                {
                    AppConnect.Model.bouquetStructure.Add(new bouquetStructure
                    {
                        bouquetID = newBouquet.bouquetID,
                        flowerID = flower.flowerID
                    });
                }

                AppConnect.Model.allProducts.Add(new allProducts
                {
                    bouquetID = newBouquet.bouquetID,
                    flowerID = null // Явно указываем null для flowerID
                });

                AppConnect.Model.SaveChanges();
                MessageBox.Show("Букет успешно добавлен!");
                MainFrame.FrameMain.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void btBack_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.FrameMain.Navigate(new MainPage.MainPage());
        }
    }
}

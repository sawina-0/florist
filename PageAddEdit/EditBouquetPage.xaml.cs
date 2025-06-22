using florist.AppData;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для EditBouquetPage.xaml
    /// </summary>
    public partial class EditBouquetPage : Page
    {
        private bouquet _currentBouquet;
        private List<flowers> _allFlowers;
        private List<flowers> _selectedFlowers = new List<flowers>();
        public EditBouquetPage(bouquet bouquet)
        {
            if (bouquet == null) throw new ArgumentNullException(nameof(bouquet));
            InitializeComponent();
            _currentBouquet = bouquet;
            Loaded += EditBouquetPage_Loaded;
        }
        private void EditBouquetPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Загрузка данных
            _allFlowers = AppConnect.Model.flowers
                .Include("type")
                .Include("color")
                .ToList();

            _selectedFlowers = AppConnect.Model.bouquetStructure
                    .Where(bs => bs.bouquetID == _currentBouquet.bouquetID)
                    .Select(bs => bs.flowers)
                    .ToList();

            lvAllFlowers.ItemsSource = _allFlowers;
            lvSelectedFlowers.ItemsSource = _selectedFlowers;

            tbName.Text = _currentBouquet.name;
            tbPrice.Text = _currentBouquet.price.ToString();

            if (!string.IsNullOrEmpty(_currentBouquet.img))
            {
                var imageUri = ImageHelper.GetImageUri(_currentBouquet.img);
                if (imageUri != null)
                {
                    imgPreview.Source = new BitmapImage(imageUri);
                }
            }
        }
        
        private void btBack_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.FrameMain.Navigate(new MainPage.MainPage());
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
            try
            {
                _currentBouquet.name = tbName.Text;
                _currentBouquet.price = price;

                //Удаляем старые связи
                var existing = AppConnect.Model.bouquetStructure
                    .Where(bs => bs.bouquetID == _currentBouquet.bouquetID);
                AppConnect.Model.bouquetStructure.RemoveRange(existing);

                // Добавляем новые связи
                foreach (var flower in _selectedFlowers)
                {
                    AppConnect.Model.bouquetStructure.Add(new bouquetStructure
                    {
                        bouquetID = _currentBouquet.bouquetID,
                        flowerID = flower.flowerID
                    });
                }
                AppConnect.Model.Entry(_currentBouquet).State = EntityState.Modified;
                AppConnect.Model.SaveChanges();
                MessageBox.Show("Изменения сохранены!");
                MainFrame.FrameMain.Navigate(new MainPage.MainPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
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
                    _currentBouquet.img = fileName;
                    imgPreview.Source = new BitmapImage(ImageHelper.GetImageUri(fileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}

using florist.AppData;
using florist.PageRegAuth;
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


namespace florist.MainPage
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private List<ProductItem> mixedProducts;
        private List<ProductItem> filtredProducts;
        public MainPage()
        {
            InitializeComponent();
            InitializeControls();
            LoadProducts();
        }
        private void InitializeControls()
        {
            cbSort.SelectionChanged += ApplyFilters;
            cbProduct.SelectionChanged += ApplyFilters;
            tbSearch.TextChanged += ApplyFilters;
        }
        
        private async void LoadProducts()
        {
            try
            {
                string userRole = Autorization.getUserRole();
                bool isAdmin = userRole == "администратор";
                mixedProducts = new List<ProductItem>();
                var flowers = await AppConnect.Model.flowers
                    .Include("type")
                    .Include("color")
                    .Include("blossomSize")
                    .Include("supplier.country")
                    .ToListAsync();
                foreach (var flower in flowers)
                {
                    mixedProducts.Add(new ProductItem
                    {
                        IsFlower = true,
                        FlowerId = flower.flowerID,
                        Img = !string.IsNullOrEmpty(flower.img) ? $"/Images/{flower.img}" : "/Images/none.png",
                        Type = flower.type.type1,
                        Color = flower.color.color1,
                        Size = flower.blossomSize.size,
                        Country = flower.supplier.country.country1,
                        Price = flower.price,
                        IsAdmin = isAdmin
                    });
                }
                var bouquets = await AppConnect.Model.bouquet
                    .Include("bouquetStructure.flowers.type")
                    .Include("bouquetStructure.flowers.color")
                    .Include("bouquetStructure.flowers.blossomSize")
                    .ToListAsync();
                foreach(var bouquet in bouquets)
                {
                    var composition = bouquet.bouquetStructure
                        .Select(bs => new FlowerInBouquet
                        {
                            FlowerName = bs.flowers.type.type1,
                            FlowerColor = bs.flowers.color.color1,
                            FlowerSize = bs.flowers.blossomSize.size
                        }).ToList();
                    mixedProducts.Add(new ProductItem
                    {
                        IsFlower = false,
                        BouquetId = bouquet.bouquetID,
                        Img = !string.IsNullOrEmpty(bouquet.img) ? $"/Images/{bouquet.img}" : "/Images/none.png",
                        BouquetName = bouquet.name,
                        BouquetPrice = bouquet.price,
                        Composition = composition,
                        IsAdmin = isAdmin
                    });
                }
                ApplyFilters(null, null);
            }
            catch(Exception ex) 
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbProduct.SelectedIndex == 0)
                {
                    filtredProducts = mixedProducts.Where(p => p.IsFlower).ToList();
                }
                else
                {
                    filtredProducts = mixedProducts.Where(p => !p.IsFlower).ToList();
                }
                if (!string.IsNullOrWhiteSpace(tbSearch.Text))
                {
                    string searchTerm = tbSearch.Text.ToLower();
                    filtredProducts = filtredProducts
                        .Where(p => (p.IsFlower &&
                                    (p.Type.ToLower().Contains(searchTerm) == true ||
                                     p.Color.ToLower().Contains(searchTerm) == true ||
                                     p.Size.ToLower().Contains(searchTerm) == true ||
                                     p.Country.ToLower().Contains(searchTerm) == true)) ||
                                     (!p.IsFlower &&
                                     p.BouquetName.ToLower().Contains(searchTerm) == true))
                        .ToList();
                }
                if(cbSort.SelectedIndex == 0)
                {
                    filtredProducts = filtredProducts
                        .OrderBy(p => p.IsFlower ? p.Price : p.BouquetPrice)
                        .ToList();
                }
                else
                {
                    filtredProducts = filtredProducts
                        .OrderByDescending(p => p.IsFlower ? p.Price : p.BouquetPrice)
                        .ToList ();
                }
                lvProducts.ItemsSource = filtredProducts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}");
            }
        }
        private void btToBasket_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btExit_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.FrameMain.Navigate(new Autorization());
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is ProductItem product)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Вы точно хотите удалить {(product.IsFlower ? "цветок" : "букет")}? Связанные объекты также удаляться!",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result != MessageBoxResult.Yes) return;
                    if (product.IsFlower) 
                    {
                        DeleteFlower(product.FlowerId);
                    }
                    else
                    {
                        DeleteBouquet(product.BouquetId);
                    }
                    mixedProducts.Remove(product);
                    MessageBox.Show("Удаление выполнено успешно", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    lvProducts.Items.Refresh();
                }
                catch (Exception ex) 
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}\n\nДетали:\n{ex.InnerException?.Message}",
                          "Ошибка",
                          MessageBoxButton.OK,
                          MessageBoxImage.Error);
                }
            }
            lvProducts.Items.Refresh();
        }
        private void DeleteFlower(int flowerId)
        {
            var relatedProducts = AppConnect.Model.allProducts
                .Where(ap => ap.flowerID == flowerId)
                .ToList();
            foreach (var product in relatedProducts)
            {
                AppConnect.Model.allProducts.Remove(product);
            }
            var flower = AppConnect.Model.flowers.Find(flowerId);
            if (flower != null)
            {
                AppConnect.Model.flowers.Remove(flower);
                AppConnect.Model.SaveChanges();
            }
        }
        private void DeleteBouquet(int bouquetId)
        {
            var relatedProducts = AppConnect.Model.allProducts
                .Where(ap => ap.bouquetID == bouquetId)
                .ToList();
            foreach (var product in relatedProducts)
            {
                AppConnect.Model.allProducts.Remove(product);
            }
            var bouquet = AppConnect.Model.bouquet.Find(bouquetId);
            if (bouquet != null)
            {
                AppConnect.Model.bouquet.Remove(bouquet);
                AppConnect.Model.SaveChanges();
            }
        }
    }
}

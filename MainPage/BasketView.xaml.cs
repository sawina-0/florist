using florist.AppData;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
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
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Fonts;

namespace florist.MainPage
{
    /// <summary>
    /// Логика взаимодействия для BasketView.xaml
    /// </summary>
    public partial class BasketView : Page
    {
        private int currentUserId;
        public BasketView()
        {
            InitializeComponent();
            currentUserId = GetCurrentUserId();
            LoadBasketData();
            LoadCities();
        }

        private void btBack_Click(object sender, RoutedEventArgs e)
        {
            AppData.MainFrame.FrameMain.Navigate(new MainPage());
        }
        public class BasketItem
        {
            public int BasketId { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public string Img { get; set; }
        }
        private int GetCurrentUserId()
        {
            return (int)Application.Current.Properties["CurrentUserId"];
        }
        private string GetUserName(int userId)
        {
            var user = AppConnect.Model.user.FirstOrDefault(u => u.userID == userId);
            return user.name;
        }
        private string GetUserSur(int userId)
        {
            var user = AppConnect.Model.user.FirstOrDefault(u => u.userID == userId);
            return user.surname;
        }
        private void LoadBasketData()
        {
            string userName = GetUserName(currentUserId);
            string userSur = GetUserSur(currentUserId);
            tblUserLogin.Text = $"Корзина пользователя: {userName} {userSur}";

            var basketItems = AppConnect.Model.basket
                .Include(b => b.allProducts.flowers.type)
                .Include(b => b.allProducts.flowers.color)
                .Include(b => b.allProducts.flowers.blossomSize)
                .Include(b => b.allProducts.bouquet)
                .Where(b => b.userID == currentUserId)
                .Select(b => new BasketItem
                {
                    BasketId = b.basketID,
                    Id = b.allProducts.productsID,
                    Name = b.allProducts.flowerID != null
                    ? b.allProducts.flowers.type.type1 + " (" +
                    b.allProducts.flowers.color.color1 + ", " +
                    b.allProducts.flowers.blossomSize.size + ")"
                    : b.allProducts.bouquet.name,
                    Price = b.allProducts.flowerID != null
                    ? b.allProducts.flowers.price
                    : b.allProducts.bouquet.price,
                    Img = b.allProducts.flowerID != null
                    ? "/Images/" + b.allProducts.flowers.img
                    : "/Images/" + b.allProducts.bouquet.img
                }).ToList();
            lvBasketItems.ItemsSource = basketItems;
            decimal total = basketItems.Sum(item => item.Price);
            tblTotalPrice.Text = $"Общая стоимость: {total:N2} руб.";

            if (basketItems.Count == 0)
            {
                tblEmptyBasket.Visibility = Visibility.Visible;
                lvBasketItems.Visibility = Visibility.Collapsed;
                tblTotalPrice.Visibility = Visibility.Collapsed;
                btCheckout.Visibility = Visibility.Collapsed;
            }
            else
            {
                tblEmptyBasket.Visibility = Visibility.Collapsed;
                lvBasketItems.Visibility = Visibility.Visible;
                btCheckout.Visibility = Visibility.Visible;
                tblTotalPrice.Visibility = Visibility.Visible;
            }
        }

        private void btCheckout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbCity.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите город.");
                    return; 
                }

                if (cbSP.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите адрес торговой точки.");
                    return; 
                }

                var selectedSalePoint = (salePoint)cbSP.SelectedItem;
                int salePointId = selectedSalePoint?.salePointID ?? 0;

                var order = new order
                {
                    basketID = ((BasketItem)lvBasketItems.Items[0]).BasketId,
                    salePointID = salePointId,
                    totalPrice = (int)((List<BasketItem>)lvBasketItems.ItemsSource).Sum(i => i.Price)
                };
                AppConnect.Model.order.Add(order);
                AppConnect.Model.SaveChanges();
                generateCheck();
                MessageBox.Show("Заказ оформлен успешно, можете убрать товар из корзины");
                LoadBasketData();
                
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка оформления: {ex.Message}");
            }
        }
        
        private void generateCheck()
        {
            try
            {
                GlobalFontSettings.FontResolver = new MyFontResolver();

                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont font = new XFont("Arial", 16);


                XRect rect = new XRect(180, 40, page.Width, page.Height);
                gfx.DrawString("КАССОВЫЙ ЧЕК АртФлора", font, XBrushes.Black, rect, XStringFormats.TopLeft);

                XRect separatorRect = new XRect(40, 140, page.Width, page.Height);
                gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, separatorRect, XStringFormats.TopLeft);

                XRect totalRect = new XRect(40, 180, page.Width, page.Height);
                gfx.DrawString("Сумма = " + ((List<BasketItem>)lvBasketItems.ItemsSource).Sum(i => i.Price).ToString("F2") + " р.", font, XBrushes.Black, totalRect, XStringFormats.TopLeft);

                DateTime currentDateTime = DateTime.Now;
                XRect dateRect = new XRect(40, 220, page.Width, page.Height);
                gfx.DrawString("Дата Время = " + currentDateTime.ToString(), font, XBrushes.Black, dateRect, XStringFormats.TopLeft);

                int prevHeight = 280;

                var basketItems = (List<BasketItem>)lvBasketItems.ItemsSource;
                foreach (var item in basketItems)
                {
                    XRect itemRect = new XRect(40, prevHeight, page.Width, page.Height);
                    gfx.DrawString($"Название: {item.Name}, Цена: {item.Price:F2} р.", font, XBrushes.Black, itemRect, XStringFormats.TopLeft);
                    prevHeight += 30; // Увеличиваем высоту для следующего элемента
                }

                XRect finalSeparatorRect = new XRect(40, prevHeight, page.Width, page.Height);
                gfx.DrawString("----------------------------------------------------------------------", font, XBrushes.Black, finalSeparatorRect, XStringFormats.TopLeft);
                prevHeight += 40;

                XRect thankYouRect = new XRect(180, prevHeight, page.Width, page.Height);
                gfx.DrawString("СПАСИБО ЗА ПОКУПКУ!", font, XBrushes.Black, thankYouRect, XStringFormats.TopLeft);

                string pdfFilename = "чек.pdf";
                document.Save(pdfFilename);
                System.Diagnostics.Process.Start(pdfFilename);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка генерации чека: {ex.Message}");
            }
            
        }
        private void btRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button && button.Tag is int basketId)
            {
                var result = MessageBox.Show("Удалить товар из корзины?","Подтверждение",MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var item = AppConnect.Model.basket.FirstOrDefault(b => b.basketID == basketId);
                        if (item != null)
                        {
                            AppConnect.Model.basket.Remove(item);
                            AppConnect.Model.SaveChanges();
                            LoadBasketData();
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}");
                    }
                }
            }
        }
        private void LoadCities()
        {
            var cities = AppConnect.Model.city.ToList();
            cbCity.ItemsSource = cities; 
        }
        private void cbCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCity.SelectedItem is city selectedCity)
            {
                int cityID = selectedCity.cityID;
                LoadSalePoints(cityID);
            }
        }
        private void LoadSalePoints(int cityID)
        {
            if (cityID <= 0)
            {
                MessageBox.Show("Пожалуйста, выберите город, чтобы увидеть доступные торговые точки.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbSP.ItemsSource = null; 
                return;
            }
            var salePoints = AppConnect.Model.salePoint
                .Where(sp => sp.cityID == cityID)
                .ToList();
            cbSP.ItemsSource = salePoints;
        }

        private void cbSP_DropDownOpened(object sender, EventArgs e)
        {
            if (cbCity.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите город перед выбором адреса.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbSP.IsDropDownOpen = false;
            }
        }
    }
}

using florist.AppData;
using System;
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace florist.AppData
{
    public class ProductItem
    {
        // Общие свойства
        public bool IsFlower { get; set; }
        public bool IsAdmin { get; set; }
        public string Img { get; set; }

        // Свойства для цветов

        public string Type { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Country { get; set; }
        public decimal Price { get; set; }

        // Свойства для букетов
        public string BouquetName { get; set; }
        public decimal BouquetPrice { get; set; }
        public List<FlowerInBouquet> Composition { get; set; }
    }

    public class FlowerInBouquet
    {
        public string FlowerName { get; set; }
        public string FlowerColor { get; set; }
        public string FlowerSize { get; set; }
    }
}

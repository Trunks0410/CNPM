using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_CK
{
    public class Product
    {
        public string maSp { get; set; }
        public string Ten { get; set; }
        public string Gia { get; set; }
        public string Type { get; set; }
        public int SoLuong { get; set; }
        public string SoKhung { get; set; } // Chỉ cho xe_may
        public string SoMay { get; set; } // Chỉ cho xe_may
        public string ImagePath { get; set; }
        public int Baohanh { get; set; }
        public string Mota { get; set; }
        public string Mau { get; set; }
        public string Phienban { get; set; }

        public Product(string maSp, string ten, string gia, int soluong, string mota, string imagePath)
        {
            this.maSp = maSp;
            Ten = ten;
            Gia = gia;
            this.SoLuong = soluong;
           
            Mota = mota;
            ImagePath = imagePath;

        }

        public Product(Product original)
        {

            this.Ten = original.Ten;
            this.Gia = original.Gia;
            this.Type = original.Type;
            this.maSp = original.maSp;
            this.Baohanh = original.Baohanh;
            this.Mota = original.Mota;
            this.ImagePath = original.ImagePath;
        }

    }
}


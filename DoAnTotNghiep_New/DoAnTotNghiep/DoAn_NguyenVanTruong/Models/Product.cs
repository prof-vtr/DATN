namespace DoAnNguyenVanTruong.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            Product_Size = new HashSet<Product_Size>();
            Rates = new HashSet<Rate>();
        }

        [Required]
        [StringLength(255)]
        public string ProductName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [StringLength(255)]
        public string Images { get; set; }

        public double SaleOff { get; set; }

        public double Price { get; set; }

        public int ProductID { get; set; }

        public int? CategoryID { get; set; }

        public int? BrandID { get; set; }

        public string Os {  get; set; }

        public string RAM { get; set; }

        public string CPU { get; set; }

        public string Storage { get; set; }

        public string DisplaySize { get; set; }

        public string BatteryCharge { get; set; }

        public double TotalRevenue { get; set; }

        public virtual Brand Brand { get; set; }

        public virtual Category Category { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_Size> Product_Size { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rate> Rates { get; set; }
        public double newprice(double price, double saleoff)
        {
            double n_price = 0;
            price = Price;
            saleoff = SaleOff;
            if(saleoff <= 0)
            {
                n_price=price;
            }
            else if(saleoff > 0)
            {
                n_price=price-( price * (saleoff / 100));
            }
            return n_price ;
        }
    }
}

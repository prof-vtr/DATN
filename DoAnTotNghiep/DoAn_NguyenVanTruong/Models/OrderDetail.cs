namespace DoAnNguyenVanTruong.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }

        public int? OrderQuantity { get; set; }

        public double? TotalPrice { get; set; }

        public int? OrderID { get; set; }

        public int? Product_SizeID { get; set; }

        public virtual Order Order { get; set; }


        public virtual Product_Size Product_Size { get; set; }
    }
}

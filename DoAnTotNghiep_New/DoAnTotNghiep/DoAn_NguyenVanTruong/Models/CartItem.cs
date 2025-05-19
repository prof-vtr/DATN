namespace DoAnNguyenVanTruong.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CartItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CartItemID { get; set; }

        public int QuantityProductSale { get; set; }

        public int Product_SizeID { get; set; }

        public virtual Product_Size Product_Size { get; set; }
    }
}

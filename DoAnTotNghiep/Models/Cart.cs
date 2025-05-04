namespace DoAn_TranTheAnh_2020607026
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Cart
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CartID { get; set; }

        public int? UserID { get; set; }

        public int? ProductID { get; set; }

        public int? QuantityInCart { get; set; }

        public decimal? TotalPrice { get; set; }

        public virtual Product Product { get; set; }

        public virtual User User { get; set; }
    }
}

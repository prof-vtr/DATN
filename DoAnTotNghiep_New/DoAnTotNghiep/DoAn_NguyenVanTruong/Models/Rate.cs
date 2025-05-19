namespace DoAnNguyenVanTruong.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Rate
    {
        public int RateID { get; set; }

        public int? UserID { get; set; }

        public int? RateValue { get; set; }

        public string Comment { get; set; }

        public DateTime? DateRate { get; set; }

        public int? ProductID { get; set; }

        public virtual Product Product { get; set; }

        public virtual User User { get; set; }
    }
}

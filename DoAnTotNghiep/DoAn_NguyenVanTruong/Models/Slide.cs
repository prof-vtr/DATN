namespace DoAnNguyenVanTruong.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Slide")]
    public partial class Slide
    {
        public int SlideID { get; set; }

        [StringLength(255)]
        public string Images { get; set; }
    }
}

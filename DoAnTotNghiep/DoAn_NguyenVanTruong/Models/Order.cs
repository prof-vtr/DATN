namespace DoAnNguyenVanTruong.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderID { get; set; }

        public DateTime OrderDate { get; set; }

      
        public string OrderStatus { get; set; }

        public double OrderTotalPrice { get; set; }

        [StringLength(255)]
        public string Address_Delivery { get; set; }

        public int? TypePayment { get; set; }

        [StringLength(255)]
        public string CustomerName { get; set; }

        [StringLength(255)]
        public string PhoneCustomer { get; set; }

        [StringLength(255)]
        public string OrderCode { get; set; }

        public int? UserID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual User User { get; set; }
    }
}

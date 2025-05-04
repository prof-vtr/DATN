using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DoAnNguyenVanTruong.Models
{
    public partial class BaseAppDbContext : DbContext
    {
        public BaseAppDbContext()
            : base("Mobile")
        {
        }

        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product_Size> Product_Size { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Rate> Rates { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<Slide> Slides { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>()
                .Property(e => e.BrandName)
                .IsUnicode(false);

            modelBuilder.Entity<Category>()
                .Property(e => e.Images)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.CustomerName)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.PhoneCustomer)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.OrderCode)
                .IsUnicode(false);

            modelBuilder.Entity<Product_Size>()
                .HasMany(e => e.CartItems)
                .WithRequired(e => e.Product_Size)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.Images)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.RoleName)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Size>()
                .Property(e => e.NameSize)
                .IsUnicode(false);

            modelBuilder.Entity<Slide>()
                .Property(e => e.Images)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);
        }
    }
}

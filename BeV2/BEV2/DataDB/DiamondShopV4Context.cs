using Microsoft.EntityFrameworkCore;

namespace BE_V2.DataDB
{
    public partial class DiamondShopV4Context : DbContext
    {
        public DiamondShopV4Context()
        {
        }

        public DiamondShopV4Context(DbContextOptions<DiamondShopV4Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Diamond> Diamonds { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductType> ProductTypes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }
        public virtual DbSet<WishlistItem> WishlistItems { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventItem> EventItems { get; set; }
        public virtual DbSet<PriceDetail> PriceDetails { get; set; }
        public virtual DbSet<CustomerPoints> CustomerPoints { get; set; }
        public virtual DbSet<OrderLog> OrderLogs { get; set; }
        public virtual DbSet<DiamondPriceTable> DiamondPriceTable { get; set; } // Added DiamondPriceTable
        public virtual DbSet<RingPriceTable> RingPriceTable { get; set; }
        public virtual DbSet<NecklacePriceTable> NecklacePriceTable { get; set; }
        public virtual DbSet<RingMold> RingMold { get; set; }
        public virtual DbSet<NecklaceMold> NecklaceMold { get; set; }
        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<Warranty> Warranties { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            => optionsBuilder.UseSqlServer("Server=tcp:luxehouse.database.windows.net,1433;Initial Catalog=Diamond_Shop_V4;Persist Security Info=False;User ID=phamnguyen1700;Password=NguyeN2004@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B8E2F39143");

                entity.ToTable("Customer");

                entity.HasIndex(e => e.UserId, "UQ__Customer__1788CCAD7E4B64B6").IsUnique();

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User).WithOne(p => p.Customer)
                    .HasForeignKey<Customer>(d => d.UserId)
                    .HasConstraintName("FK__Customer__UserID__571DF1D5");

                entity.HasMany(d => d.Wishlists)
                   .WithOne(p => p.Customer)
                   .HasForeignKey(d => d.CustomerId)
                   .HasConstraintName("FK__Wishlist__Custom__02FC7413");
            });

            modelBuilder.Entity<Diamond>(entity =>
            {
                entity.HasKey(e => e.DiamondId).HasName("PK__Diamond__23A8E7BBE47D50DC");
                entity.ToTable("Diamond");
                entity.Property(e => e.DiamondId).HasColumnName("DiamondID");
                entity.Property(e => e.CaratWeight).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Clarity).HasMaxLength(50);
                entity.Property(e => e.Color).HasMaxLength(50);
                entity.Property(e => e.Cut).HasMaxLength(50);
                entity.Property(e => e.Depth).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Fluorescence).HasMaxLength(50);
                entity.Property(e => e.Girdle).HasMaxLength(50);
                entity.Property(e => e.LengthWidthRatio).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Measurements).HasMaxLength(255);
                entity.Property(e => e.Shape).HasMaxLength(50);
                entity.Property(e => e.Symmetry).HasMaxLength(50);
                entity.Property(e => e.Tables).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Origin).HasMaxLength(255);

                entity.HasOne(d => d.Certificate)
                    .WithOne(c => c.Diamond)
                    .HasForeignKey<Certificate>(c => c.DiamondId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Certificate>(entity =>
            {
                entity.HasKey(e => e.CertificateId);

                entity.Property(e => e.ReportNumber).HasMaxLength(50);
                entity.Property(e => e.ClarityCharacteristics).HasMaxLength(100);
                entity.Property(e => e.Inscription).HasMaxLength(100);
                entity.Property(e => e.Comments).HasMaxLength(255);
            });


            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDF6BA84AC5C");

                entity.ToTable("Feedback");

                entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");
                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
                entity.Property(e => e.FeedbackText).HasMaxLength(1000);
                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.HasOne(d => d.Customer).WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Feedback__Custom__6477ECF3");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__Feedback__ProductID__649C50CB");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF84671313");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(15, 2)");

                entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Orders__Customer__59FA5E80");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30CF13D40EE");

                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.ProductName).HasMaxLength(255);
                entity.Property(e => e.ProductPrice).HasColumnType("decimal(15, 2)");

                entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__OrderDeta__Order__5CD6CB2B");

                entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__OrderDeta__Produ__5DCAEF64");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A58C7262DC1");

                entity.ToTable("Payment");

                entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
                entity.Property(e => e.AmountPaid).HasColumnType("decimal(15, 2)");
                entity.Property(e => e.Deposit).HasColumnType("decimal(15, 2)");
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.Total).HasColumnType("decimal(15, 2)");

                entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__Payment__OrderID__60A75C0F");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId).HasName("PK_Product");

                entity.ToTable("Product");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.ProductName).HasMaxLength(255);
                entity.Property(e => e.ProductType).HasColumnName("ProductType");
                entity.Property(e => e.Material).HasMaxLength(50);
                entity.Property(e => e.Size).HasMaxLength(50);
                entity.Property(e => e.Description).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ProcessingPrice).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Quantity).HasColumnName("Quantity");
                entity.Property(e => e.MainDiamondId).HasColumnName("MainDiamondId");
                entity.Property(e => e.Image1).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Image2).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Image3).HasColumnType("nvarchar(max)");
                entity.Property(e => e.SecondaryDiamondId).HasColumnName("SecondaryDiamondId");
                entity.Property(e => e.RingMoldId).HasColumnName("RingMoldId");
                entity.Property(e => e.NecklaceMoldId).HasColumnName("NecklaceMoldId");
                entity.Property(e => e.SecondaryDiamondCount).HasColumnName("SecondaryDiamondCount");

                entity.HasOne(d => d.MainDiamond)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.MainDiamondId)
                    .HasConstraintName("FK_Product_MainDiamond");

                entity.HasOne(d => d.SecondaryDiamond)
                    .WithMany()
                    .HasForeignKey(d => d.SecondaryDiamondId)
                    .HasConstraintName("FK_Product_SecondaryDiamond");

                entity.HasOne(d => d.ProductTypeNavigation)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductType)
                    .HasConstraintName("FK_Product_ProductType");

                entity.HasOne(d => d.RingMold)
                    .WithMany()
                    .HasForeignKey(d => d.RingMoldId)
                    .HasConstraintName("FK_Product_RingMold");

                entity.HasOne(d => d.NecklaceMold)
                    .WithMany()
                    .HasForeignKey(d => d.NecklaceMoldId)
                    .HasConstraintName("FK_Product_NecklaceMold");

                entity.HasMany(d => d.WishlistItems)
                    .WithOne(p => p.Product)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_WishlistItems_Product");

                entity.HasMany(d => d.EventItems)
                    .WithOne(ei => ei.Product)
                    .HasForeignKey(ei => ei.ProductID)
                    .HasConstraintName("FK_EventItems_Product");

                entity.HasMany(d => d.PriceDetails)
                    .WithOne(p => p.Product)
                    .HasForeignKey(p => p.ProductID)
                    .HasConstraintName("FK_PriceDetails_Product");

                entity.HasMany(d => d.Feedbacks)
                    .WithOne(f => f.Product)
                    .HasForeignKey(f => f.ProductId)
                    .HasConstraintName("FK_Feedbacks_Product");
            });

            modelBuilder.Entity<ProductType>(entity =>
            {
                entity.HasKey(e => e.ProductTypeId).HasName("PK__ProductT__A1312F4E6F0C4623");

                entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");
                entity.Property(e => e.ProductTypeName).HasMaxLength(50);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A3AEE977B");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");
                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC77FF890C");

                entity.Property(e => e.UserId).HasColumnName("UserID");
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Password).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.RoleId).HasColumnName("RoleID");
                entity.Property(e => e.Sex).HasMaxLength(5);
                entity.Property(e => e.Username).HasMaxLength(50);

                entity.HasOne(d => d.Role).WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Users__RoleID__4BAC3F29");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.CartID).HasName("PK__Cart__2F36C7C22AAEDC89");

                entity.ToTable("Cart");

                entity.Property(e => e.CartID).HasColumnName("CartID");
                entity.Property(e => e.UserID).HasColumnName("UserID");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.UserID)
                    .HasConstraintName("FK__Cart__UserID__2A4B4B5E");
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.CartItemID).HasName("PK__CartItem__3A4CA8E7AAB53760");

                entity.ToTable("CartItem");

                entity.Property(e => e.CartItemID).HasColumnName("CartItemID");
                entity.Property(e => e.CartID).HasColumnName("CartID");
                entity.Property(e => e.ProductID).HasColumnName("ProductID");
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(15, 2)").IsRequired();

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CartID)
                    .HasConstraintName("FK__CartItem__CartID__2B3F6F97");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.ProductID)
                    .HasConstraintName("FK__CartItem__Product__2C3393D0");
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.HasKey(e => e.WishlistId).HasName("PK__Wishlist__233189CB03175B66");

                entity.ToTable("Wishlist");

                entity.Property(e => e.WishlistId).HasColumnName("WishlistID");
                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.HasOne(d => d.Customer).WithMany(p => p.Wishlists)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Wishlist__Custom__02FC7413");
            });

            modelBuilder.Entity<WishlistItem>(entity =>
            {
                entity.HasKey(e => e.WishlistItemId).HasName("PK__Wishlist__171E21813C0A4C5A");

                entity.Property(e => e.WishlistItemId).HasColumnName("WishlistItemID");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.WishlistId).HasColumnName("WishlistID");

                entity.HasOne(d => d.Product).WithMany(p => p.WishlistItems)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__WishlistI__Produ__06CD04F7");

                entity.HasOne(d => d.Wishlist).WithMany(p => p.WishlistItems)
                    .HasForeignKey(d => d.WishlistId)
                    .HasConstraintName("FK__WishlistI__Wishl__05D8E0BE");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.EventID).HasName("PK__Event__1E20497A");

                entity.ToTable("Event");

                entity.Property(e => e.EventID).HasColumnName("EventID");
                entity.Property(e => e.EventName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(int.MaxValue);

                entity.HasMany(e => e.EventItems)
                      .WithOne(ei => ei.Event)
                      .HasForeignKey(ei => ei.EventID)
                      .HasConstraintName("FK__EventItem__EventID__02FC7413");
            });

            modelBuilder.Entity<EventItem>(entity =>
            {
                entity.HasKey(e => e.EventItemID).HasName("PK__EventItem__1BFD2C07");

                entity.ToTable("EventItem");

                entity.Property(e => e.EventItemID).HasColumnName("EventItemID");
                entity.Property(e => e.EventID).IsRequired();
                entity.Property(e => e.ProductID).IsRequired();
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Discount).IsRequired();

                entity.HasOne(ei => ei.Event)
                      .WithMany(e => e.EventItems)
                      .HasForeignKey(ei => ei.EventID)
                      .HasConstraintName("FK__EventItem__EventID__02FC7413");

                entity.HasOne(ei => ei.Product)
                      .WithMany(p => p.EventItems)
                      .HasForeignKey(ei => ei.ProductID)
                      .HasConstraintName("FK__EventItem__ProductID__02FC7413");
            });

            modelBuilder.Entity<PriceDetail>(entity =>
            {
                entity.HasKey(e => e.PriceDetailID).HasName("PK__PriceDetail__1A14E395");

                entity.ToTable("PriceDetail");

                entity.Property(e => e.PriceDetailID).HasColumnName("PriceDetailID");
                entity.Property(e => e.ProductID).IsRequired();
                entity.Property(e => e.DiamondPrice).IsRequired();
                entity.Property(e => e.JewelryPrice).IsRequired();
                entity.Property(e => e.ProcessingPrice).IsRequired();
                entity.Property(e => e.Profit).IsRequired();

                entity.HasOne(pd => pd.Product)
                      .WithMany(p => p.PriceDetails)
                      .HasForeignKey(pd => pd.ProductID)
                      .HasConstraintName("FK__PriceDeta__ProductID__06CD04F7");
            });

            modelBuilder.Entity<CustomerPoints>(entity =>
            {
                entity.HasKey(e => e.CustomerPointID).HasName("PK__CustomerPoints");

                entity.ToTable("CustomerPoints");

                entity.Property(e => e.CustomerPointID).HasColumnName("CustomerPointID");
                entity.Property(e => e.CustomerID).IsRequired();
                entity.Property(e => e.Points).IsRequired();
                entity.Property(e => e.LastUpdated).IsRequired();

                entity.HasOne(cp => cp.Customer)
                      .WithMany(c => c.CustomerPoints)
                      .HasForeignKey(cp => cp.CustomerID)
                      .HasConstraintName("FK__CustomerPoints__CustomerID");
            });

            modelBuilder.Entity<OrderLog>(entity =>
            {
                entity.HasKey(e => e.LogID).HasName("PK__OrderLog__A5D58A608123E0B0");

                entity.ToTable("OrderLogs");

                entity.Property(e => e.LogID).HasColumnName("LogID");
                entity.Property(e => e.OrderID).IsRequired();
                entity.Property(e => e.Phase1).HasDefaultValue(false);
                entity.Property(e => e.Phase2).HasDefaultValue(false);
                entity.Property(e => e.Phase3).HasDefaultValue(false);
                entity.Property(e => e.Phase4).HasDefaultValue(false);

                entity.HasOne(ol => ol.Order)
                      .WithMany(o => o.OrderLogs)
                      .HasForeignKey(ol => ol.OrderID)
                      .HasConstraintName("FK__OrderLogs__OrderID__02FC7413");
            });

            modelBuilder.Entity<DiamondPriceTable>(entity =>
            {
                entity.HasKey(e => new { e.Carat, e.Color, e.Clarity, e.Cut }).HasName("PK_DiamondPriceTable");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<RingPriceTable>(entity =>
            {
                entity.HasKey(e => new { e.Material, e.Size, e.CaratWeight }).HasName("PK_RingPriceTable");
                entity.Property(e => e.Material).HasMaxLength(50);
                entity.Property(e => e.Size).HasColumnType("decimal(3, 1)");
                entity.Property(e => e.CaratWeight).HasColumnType("decimal(4, 2)");
                entity.Property(e => e.BasePrice).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<NecklacePriceTable>(entity =>
            {
                entity.HasKey(e => new { e.Material, e.Length, e.CaratWeight }).HasName("PK_NecklacePriceTable");
                entity.Property(e => e.Material).HasMaxLength(50);
                entity.Property(e => e.Length).HasColumnType("int");
                entity.Property(e => e.CaratWeight).HasColumnType("decimal(3, 2)");
                entity.Property(e => e.BasePrice).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<RingMold>(entity =>
            {
                entity.HasKey(e => e.RingMoldId);
                entity.Property(e => e.Material).HasMaxLength(50);
                entity.Property(e => e.Size).HasMaxLength(50);
                entity.Property(e => e.CaratWeight).HasColumnType("decimal(4, 2)");
                entity.Property(e => e.Gender).HasMaxLength(6);
                entity.Property(e => e.RingType).HasMaxLength(10);
                entity.Property(e => e.BasePrice).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<NecklaceMold>(entity =>
            {
                entity.HasKey(e => e.NecklaceMoldId);
                entity.Property(e => e.Material).HasMaxLength(50);
                entity.Property(e => e.Size).HasMaxLength(50);
                entity.Property(e => e.CaratWeight).HasColumnType("decimal(3, 2)");
                entity.Property(e => e.BasePrice).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Warranty>(entity =>
            {
                entity.HasKey(e => e.WarrantyId);
                entity.Property(e => e.StoreRepresentativeSignature).IsRequired();

                entity.HasOne(e => e.Order)
                      .WithMany(o => o.Warranties) // Make sure to update Order entity to include this relationship
                      .HasForeignKey(e => e.OrderId);
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

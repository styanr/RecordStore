using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RecordStore.Api.Entities;

namespace RecordStore.Api.Context;

public partial class RecordStoreContext : DbContext
{
    public RecordStoreContext()
    {
    }

    public RecordStoreContext(DbContextOptions<RecordStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<Format> Formats { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryEvent> InventoryEvents { get; set; }

    public virtual DbSet<OrderLine> OrderLines { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }

    public virtual DbSet<Record> Records { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ShopOrder> ShopOrders { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<ShoppingCartProduct> ShoppingCartProducts { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine);
        NpgsqlConnection.GlobalTypeMapper.MapEnum<OrderStatus>("order_status");
        NpgsqlConnection.GlobalTypeMapper.MapEnum<EventType>("event_type");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("event_type", ["Sale", "Purchase", "Cancel"])
            .HasPostgresEnum("order_status", ["Pending", "Paid", "Processing", "Shipped", "Delivered", "Canceled"])
            .HasPostgresExtension("moddatetime");

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("address_pkey");

            entity.ToTable("address");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Apartment)
                .HasMaxLength(10)
                .HasColumnName("apartment");
            entity.Property(e => e.Building)
                .HasMaxLength(10)
                .HasColumnName("building");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Region)
                .HasMaxLength(50)
                .HasColumnName("region");
            entity.Property(e => e.Street)
                .HasMaxLength(150)
                .HasColumnName("street");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("address_user_id_fkey");
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("app_user");

            entity.HasIndex(e => e.Email, "app_user_email_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.AppUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("app_user_role_id_fkey");
        });

        modelBuilder.Entity<Artist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("artist_pkey");

            entity.ToTable("artist");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasMany(d => d.Records).WithMany(p => p.Artists)
                .UsingEntity<Dictionary<string, object>>(
                    "ArtistRecord",
                    r => r.HasOne<Record>().WithMany()
                        .HasForeignKey("RecordId")
                        .HasConstraintName("artist_record_record_id_fkey"),
                    l => l.HasOne<Artist>().WithMany()
                        .HasForeignKey("ArtistId")
                        .HasConstraintName("artist_record_artist_id_fkey"),
                    j =>
                    {
                        j.HasKey("ArtistId", "RecordId").HasName("artist_record_pkey");
                        j.ToTable("artist_record");
                        j.IndexerProperty<int>("ArtistId").HasColumnName("artist_id");
                        j.IndexerProperty<int>("RecordId").HasColumnName("record_id");
                    });
        });

        modelBuilder.Entity<Format>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("format_pkey");

            entity.ToTable("format");

            entity.HasIndex(e => e.FormatName, "format_format_name_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.FormatName)
                .HasMaxLength(50)
                .HasColumnName("format_name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("genre_pkey");

            entity.ToTable("genre");

            entity.HasIndex(e => e.Name, "genre_name_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasMany(d => d.Records).WithMany(p => p.Genres)
                .UsingEntity<Dictionary<string, object>>(
                    "GenreRecord",
                    r => r.HasOne<Record>().WithMany()
                        .HasForeignKey("RecordId")
                        .HasConstraintName("genre_record_record_id_fkey"),
                    l => l.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .HasConstraintName("genre_record_genre_id_fkey"),
                    j =>
                    {
                        j.HasKey("GenreId", "RecordId").HasName("genre_record_pkey");
                        j.ToTable("genre_record");
                        j.IndexerProperty<int>("GenreId").HasColumnName("genre_id");
                        j.IndexerProperty<int>("RecordId").HasColumnName("record_id");
                    });
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("inventory_pkey");

            entity.ToTable("inventory");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.RestockLevel).HasColumnName("restock_level");

            entity.HasOne(d => d.Product).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventory_product_id_fkey");
        });

        modelBuilder.Entity<InventoryEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("inventory_event_pkey");

            entity.ToTable("inventory_event");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.QuantityChange).HasColumnName("quantity_change");
            entity.Property(e => e.EventType).HasColumnName("event_type");

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryEvents)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventory_event_product_id_fkey");
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.HasKey(e => new { e.ShopOrderId, e.ProductId }).HasName("order_line_pkey");

            entity.ToTable("order_line");

            entity.Property(e => e.ShopOrderId).HasColumnName("shop_order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Price)
                .HasPrecision(12, 2)
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("order_line_product_id_fkey");

            entity.HasOne(d => d.ShopOrder).WithMany(p => p.OrderLines)
                .HasForeignKey(d => d.ShopOrderId)
                .HasConstraintName("order_line_shop_order_id_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_pkey");

            entity.ToTable("product");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FormatId).HasColumnName("format_id");
            entity.Property(e => e.Price)
                .HasPrecision(12, 2)
                .HasColumnName("price");
            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Format).WithMany(p => p.Products)
                .HasForeignKey(d => d.FormatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("product_format_id_fkey");

            entity.HasOne(d => d.Record).WithMany(p => p.Products)
                .HasForeignKey(d => d.RecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("product_record_id_fkey");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("purchase_order_pkey");

            entity.ToTable("purchase_order");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.Total)
                .HasPrecision(12, 2)
                .HasColumnName("total");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("purchase_order_supplier_id_fkey");
        });

        modelBuilder.Entity<PurchaseOrderLine>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("purchase_order_product_pkey");

            entity.ToTable("purchase_order_line");

            entity.HasIndex(e => new { e.ProductId, e.Id }, "purchase_order_line_product_id_id_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.PurchaseOrderId).HasColumnName("purchase_order_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseOrderLines)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("purchase_order_product_product_id_fkey");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseOrderLines)
                .HasForeignKey(d => d.PurchaseOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("purchase_order_product_purchase_order_id_fkey");
        });

        modelBuilder.Entity<Record>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("record_pkey");

            entity.ToTable("record");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ReleaseDate).HasColumnName("release_date");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("review_pkey");

            entity.ToTable("review");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("review_product_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("review_user_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<ShopOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shop_order_pkey");

            entity.ToTable("shop_order");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Apartment)
                .HasMaxLength(10)
                .HasColumnName("apartment");
            entity.Property(e => e.Building)
                .HasMaxLength(10)
                .HasColumnName("building");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Region)
                .HasMaxLength(50)
                .HasColumnName("region");
            entity.Property(e => e.Street)
                .HasMaxLength(150)
                .HasColumnName("street");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.User).WithMany(p => p.ShopOrders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("shop_order_user_id_fkey");
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shopping_cart_pkey");

            entity.ToTable("shopping_cart");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("shopping_cart_user_id_fkey");
        });

        modelBuilder.Entity<ShoppingCartProduct>(entity =>
        {
            entity.HasKey(e => new { e.ShoppingCartId, e.ProductId }).HasName("shopping_cart_product_pkey");

            entity.ToTable("shopping_cart_product");

            entity.Property(e => e.ShoppingCartId).HasColumnName("shopping_cart_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Product).WithMany(p => p.ShoppingCartProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("shopping_cart_product_product_id_fkey");

            entity.HasOne(d => d.ShoppingCart).WithMany(p => p.ShoppingCartProducts)
                .HasForeignKey(d => d.ShoppingCartId)
                .HasConstraintName("shopping_cart_product_shopping_cart_id_fkey");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("supplier_pkey");

            entity.ToTable("supplier");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(1024)
                .HasColumnName("address");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

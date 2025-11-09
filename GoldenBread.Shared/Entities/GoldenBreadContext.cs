using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Shared.Entities;

public partial class GoldenBreadContext : DbContext
{
    public GoldenBreadContext()
    {
    }

    public GoldenBreadContext(DbContextOptions<GoldenBreadContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeTask> EmployeeTasks { get; set; }

    public virtual DbSet<Favourite> Favourites { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<IngredientBatch> IngredientBatches { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderTariff> OrderTariffs { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductBatch> ProductBatches { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql
        (
            "Host=localhost;Database=golden_bread;Username=postgres;Password=root",
            o =>
            {
                o.MapEnum<AccountType>("account_type");
                o.MapEnum<IngredientBatchStatus>("ingredient_batch_status");
                o.MapEnum<IngredientUnit>("ingredient_unit");
                o.MapEnum<OrderStatus>("order_status");
                o.MapEnum<UserRole>("user_role");
                o.MapEnum<VerificationStatus>("verification_status");
            }
        ).UseSnakeCaseNamingConvention();*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("cart_items_new_pkey");

            entity.ToTable("cart_items");

            entity.HasIndex(e => e.BatchId, "fk_cart_items_product_batch_id_idx");

            entity.HasIndex(e => e.UserId, "fk_cart_items_user_id_idx");

            entity.Property(e => e.CartItemId)
                .HasDefaultValueSql("nextval('cart_items_new_cart_item_id_seq'::regclass)")
                .HasColumnName("cart_item_id");
            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Batch).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("fk_cart_items_product_batch_id");

            entity.HasOne(d => d.User).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cart_items_user_id");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("employees_pkey");

            entity.ToTable("employees");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.Deleted)
                .HasDefaultValue((short)0)
                .HasColumnName("deleted");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
        });

        modelBuilder.Entity<EmployeeTask>(entity =>
        {
            entity.HasKey(e => e.EmployeeTaskId).HasName("employee_tasks_new_pkey");

            entity.ToTable("employee_tasks");

            entity.HasIndex(e => e.EmployeeId, "fk_employee_tasks_employee_id_idx");

            entity.HasIndex(e => e.OrderItemId, "fk_employee_tasks_order_item_id_idx");

            entity.Property(e => e.EmployeeTaskId)
                .HasDefaultValueSql("nextval('employee_tasks_new_employee_task_id_seq'::regclass)")
                .HasColumnName("employee_task_id");
            entity.Property(e => e.AssignedQuantity).HasColumnName("assigned_quantity");
            entity.Property(e => e.CompletedQuantity)
                .HasDefaultValue(0)
                .HasColumnName("completed_quantity");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.StartTime).HasColumnName("start_time");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeTasks)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_employee_tasks_employee_id");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.EmployeeTasks)
                .HasForeignKey(d => d.OrderItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_employee_tasks_order_item_id");
        });

        modelBuilder.Entity<Favourite>(entity =>
        {
            entity.HasKey(e => e.FavouriteId).HasName("favourites_pkey");

            entity.ToTable("favourites");

            entity.HasIndex(e => e.ProductId, "fk_favourites_product_id_idx");

            entity.HasIndex(e => e.UserId, "fk_favourites_user_id_idx");

            entity.Property(e => e.FavouriteId).HasColumnName("favourite_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Favourites)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_favourites_product_id");

            entity.HasOne(d => d.User).WithMany(p => p.Favourites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_favourites_user_id");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("ingredients_pkey");

            entity.ToTable("ingredients");

            entity.HasIndex(e => e.SupplierId, "fk_ingredients_supplier_id_idx");

            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.Deleted)
                .HasDefaultValue((short)0)
                .HasColumnName("deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ShelfLifeMonths).HasColumnName("shelf_life_months");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.Weight)
                .HasPrecision(4, 3)
                .HasColumnName("weight");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Ingredients)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ingredients_supplier_id");
        });

        modelBuilder.Entity<IngredientBatch>(entity =>
        {
            entity.HasKey(e => e.IngredientBatchId).HasName("ingredient_batches_pkey");

            entity.ToTable("ingredient_batches");

            entity.HasIndex(e => e.IngredientId, "fk_ingredient_batches_ingredient_id_idx");

            entity.Property(e => e.IngredientBatchId).HasColumnName("ingredient_batch_id");
            entity.Property(e => e.DeliveryDate).HasColumnName("delivery_date");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.PurchasedQuantity).HasColumnName("purchased_quantity");
            entity.Property(e => e.RemainingQuantity)
                .HasPrecision(4, 3)
                .HasColumnName("remaining_quantity");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.IngredientBatches)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ingredient_batches_ingredient_id");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.HasIndex(e => e.TariffId, "fk_orders_tariff_id_idx");

            entity.HasIndex(e => e.UserId, "fk_orders_user_id_idx");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.TariffId).HasColumnName("tariff_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Tariff).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TariffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_tariff_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orders_user_id");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("order_items_new_pkey");

            entity.ToTable("order_items");

            entity.HasIndex(e => e.OrderId, "fk_order_items_order_id_idx");

            entity.HasIndex(e => e.BatchId, "fk_order_items_product_batch_id_idx");

            entity.Property(e => e.OrderItemId)
                .HasDefaultValueSql("nextval('order_items_new_order_item_id_seq'::regclass)")
                .HasColumnName("order_item_id");
            entity.Property(e => e.BatchId).HasColumnName("batch_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TotalUnits).HasColumnName("total_units");

            entity.HasOne(d => d.Batch).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("fk_order_items_product_batch_id");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_order_items_order_id");
        });

        modelBuilder.Entity<OrderTariff>(entity =>
        {
            entity.HasKey(e => e.OrderTariffId).HasName("order_tariffs_pkey");

            entity.ToTable("order_tariffs");

            entity.Property(e => e.OrderTariffId).HasColumnName("order_tariff_id");
            entity.Property(e => e.Deleted)
                .HasDefaultValue((short)0)
                .HasColumnName("deleted");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FreeEmployeesPercent)
                .HasPrecision(4, 2)
                .HasColumnName("free_employees_percent");
            entity.Property(e => e.MarkupPercent)
                .HasPrecision(4, 2)
                .HasColumnName("markup_percent");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("products_pkey");

            entity.ToTable("products");

            entity.HasIndex(e => e.CategoryId, "fk_products_category_id_idx");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CostPrice)
                .HasPrecision(4, 2)
                .HasColumnName("cost_price");
            entity.Property(e => e.Deleted)
                .HasDefaultValue((short)0)
                .HasColumnName("deleted");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MarkupPercent).HasColumnName("markup_percent");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ProductionTime).HasColumnName("production_time");
            entity.Property(e => e.SalePrice)
                .HasPrecision(4, 2)
                .HasColumnName("sale_price");
            entity.Property(e => e.Weight)
                .HasPrecision(5, 3)
                .HasColumnName("weight");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_products_category_id");
        });

        modelBuilder.Entity<ProductBatch>(entity =>
        {
            entity.HasKey(e => e.ProductBatchId).HasName("product_batches_new_pkey");

            entity.ToTable("product_batches");

            entity.HasIndex(e => e.ProductId, "fk_product_batches_product_id_idx");

            entity.Property(e => e.ProductBatchId)
                .HasDefaultValueSql("nextval('product_batches_new_product_batch_id_seq'::regclass)")
                .HasColumnName("product_batch_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Units).HasColumnName("units");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductBatches)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_batches_product_id");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.ProductCategoryId).HasName("production_categories_pkey");

            entity.ToTable("product_categories");

            entity.Property(e => e.ProductCategoryId)
                .HasDefaultValueSql("nextval('production_categories_production_category_id_seq'::regclass)")
                .HasColumnName("product_category_id");
            entity.Property(e => e.Color)
                .HasMaxLength(6)
                .HasColumnName("color");
            entity.Property(e => e.Deleted)
                .HasDefaultValue((short)0)
                .HasColumnName("deleted");
            entity.Property(e => e.Icon).HasColumnName("icon");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ProductImageId).HasName("product_images_pkey");

            entity.ToTable("product_images");

            entity.HasIndex(e => e.ProductId, "fk_product_images_product_id_idx");

            entity.Property(e => e.ProductImageId).HasColumnName("product_image_id");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product_images_product_id");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("recipes_pkey");

            entity.ToTable("recipes");

            entity.HasIndex(e => e.IngredientId, "fk_recipe_ingredient_id_idx");

            entity.HasIndex(e => e.ProductId, "fk_recipe_product_id_idx");

            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasPrecision(4, 3)
                .HasColumnName("quantity");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_recipe_ingredient_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_recipe_product_id");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("suppliers_pkey");

            entity.ToTable("suppliers");

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Deleted)
                .HasDefaultValue((short)0)
                .HasColumnName("deleted");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(11)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.CompanyAddress).HasColumnName("company_address");
            entity.Property(e => e.CompanyInn)
                .HasMaxLength(12)
                .HasColumnName("company_inn");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(150)
                .HasColumnName("company_name");
            entity.Property(e => e.CompanyOgrn)
                .HasMaxLength(13)
                .HasColumnName("company_ogrn");
            entity.Property(e => e.CompanyPhone)
                .HasMaxLength(11)
                .HasColumnName("company_phone");
            entity.Property(e => e.Dismissed)
                .HasDefaultValue((short)0)
                .HasColumnName("dismissed");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using Microsoft.EntityFrameworkCore;
using prismodSale.Src.Infraestructure.Persistence.Models;

namespace prismodSale.Src.Infraestructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TaxConfigurationModel> TaxConfigurations { get; set; } = null!;
    public DbSet<PaymentMethodModel> PaymentMethods { get; set; } = null!;
    public DbSet<WaiterModel> Waiters { get; set; } = null!;
    public DbSet<KdsTeamModel> KdsTeams { get; set; } = null!;
    public DbSet<KdsTeamCategoryModel> KdsTeamCategories { get; set; } = null!;
    public DbSet<TicketModel> Tickets { get; set; } = null!;
    public DbSet<TicketItemModel> TicketItems { get; set; } = null!;
    public DbSet<SaleModel> Sales { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("sales");

        modelBuilder.Entity<TaxConfigurationModel>(e =>
        {
            e.ToTable("tax_configurations");
            e.HasKey(x => x.Id);
            e.HasAlternateKey(x => x.CompanyCen);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.CompanyCen).HasColumnName("company_cen");
            e.Property(x => x.GlobalTaxPercentage).HasColumnName("global_tax_percentage");
        });

        modelBuilder.Entity<PaymentMethodModel>(e =>
        {
            e.ToTable("payment_methods");
            e.HasKey(x => x.PaymentMethodCode);
            e.Property(x => x.PaymentMethodCode).HasColumnName("payment_method_code");
            e.Property(x => x.Name).HasColumnName("name");
            e.Property(x => x.IsActive).HasColumnName("is_active");
        });

        modelBuilder.Entity<WaiterModel>(e =>
        {
            e.ToTable("waiters");
            e.HasKey(x => x.Id);
            e.HasAlternateKey(x => x.WaiterCen);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.WaiterCen).HasColumnName("waiter_cen");
            e.Property(x => x.CompanyCen).HasColumnName("company_cen");
            e.Property(x => x.Name).HasColumnName("name");
        });

        modelBuilder.Entity<KdsTeamModel>(e =>
        {
            e.ToTable("kds_teams");
            e.HasKey(x => x.Id);
            e.HasAlternateKey(x => x.TeamCen);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.TeamCen).HasColumnName("team_cen");
            e.Property(x => x.CompanyCen).HasColumnName("company_cen");
            e.Property(x => x.Name).HasColumnName("name");
        });

        modelBuilder.Entity<KdsTeamCategoryModel>(e =>
        {
            e.ToTable("kds_team_categories");
            e.HasKey(x => new { x.TeamCen, x.CategoryCen });
            e.Property(x => x.TeamCen).HasColumnName("team_cen");
            e.Property(x => x.CategoryCen).HasColumnName("category_cen");

            e.HasOne(x => x.Team)
                .WithMany(t => t.Categories)
                .HasForeignKey(x => x.TeamCen)
                .HasPrincipalKey(t => t.TeamCen);
        });

        modelBuilder.Entity<TicketModel>(e =>
        {
            e.ToTable("tickets");
            e.HasKey(x => x.Id);
            e.HasAlternateKey(x => x.TicketCen);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.TicketCen).HasColumnName("ticket_cen");
            e.Property(x => x.CompanyCen).HasColumnName("company_cen");
            e.Property(x => x.WaiterCen).HasColumnName("waiter_cen");
            e.Property(x => x.DailyNumber).HasColumnName("daily_number");
            e.Property(x => x.Status).HasColumnName("status");
            e.Property(x => x.Subtotal).HasColumnName("subtotal");
            e.Property(x => x.TaxAmount).HasColumnName("tax_amount");
            e.Property(x => x.Total).HasColumnName("total");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<TicketItemModel>(e =>
        {
            e.ToTable("ticket_items");
            e.HasKey(x => x.Id);
            e.HasAlternateKey(x => x.TicketItemCen);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.TicketItemCen).HasColumnName("ticket_item_cen");
            e.Property(x => x.TicketCen).HasColumnName("ticket_cen");
            e.Property(x => x.ProductCen).HasColumnName("product_cen");
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.UnitPrice).HasColumnName("unit_price");
            e.Property(x => x.Note).HasColumnName("note");
            e.Property(x => x.KdsStatus).HasColumnName("kds_status");
            e.Property(x => x.SentAt).HasColumnName("sent_at");
            e.Property(x => x.ResendCount).HasColumnName("resend_count");

            e.HasOne(x => x.Ticket)
                .WithMany(t => t.Items)
                .HasForeignKey(x => x.TicketCen)
                .HasPrincipalKey(t => t.TicketCen);
        });

        modelBuilder.Entity<SaleModel>(e =>
        {
            e.ToTable("sales");
            e.HasKey(x => x.Id);
            e.HasAlternateKey(x => x.SaleCen);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.SaleCen).HasColumnName("sale_cen");
            e.Property(x => x.TicketCen).HasColumnName("ticket_cen");
            e.Property(x => x.PaymentMethodCode).HasColumnName("payment_method_code");
            e.Property(x => x.InventoryDocumentCen).HasColumnName("inventory_document_cen");
            e.Property(x => x.Total).HasColumnName("total");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });
    }
}

using DocumentFormat.OpenXml.Wordprocessing;
using HUECL.alpha._6_0.Areas.Identity.Data;
using HUECL.alpha._6_0.Models.Projects;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HUECL.alpha._6_0.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext>
            options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDelivery> SaleDeliveries { get; set; }
        public DbSet<SaleDeliveryItem> SaleDeliveryItems { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<SaleInvoice> SaleInvoices { get; set; }
        public DbSet<SaleInvoicePayment> SaleInvoicePayments { get; set; }

        public DbSet<ProjectDocument> ProjectDocuments { get; set; }
        public DbSet<ProjectDocumentType> ProjectDocumentTypes { get; set; }
        public DbSet<Interaction> Interactions { get; set; }
        public DbSet<InteractionType> InteractionTypes { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectStatus> ProjectStatuses { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<ProjectSector> ProjectSectors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure that all FK related to AspNetUsers have ON DELETE NO ACTION

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.OwnedProjects)  // Explicit navigation to prevent duplicate FK
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProjectUser>()
                .HasOne(pu => pu.User)
                .WithMany(u => u.ProjectUsers)  // Explicit navigation
                .HasForeignKey(pu => pu.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Interaction>()
                .HasOne(i => i.CreatedByUser)
                .WithMany(u => u.CreatedInteractions)  // Explicit navigation
                .HasForeignKey(i => i.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reminder>()
                .HasOne(r => r.AssignedUser)
                .WithMany(u => u.AssignedReminders)  // Explicit navigation
                .HasForeignKey(r => r.AssignedUserId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}

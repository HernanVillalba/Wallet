using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Wallet.Data.Models
{
    public partial class WALLETContext : DbContext
    {
        public WALLETContext()
        {
        }

        public WALLETContext(DbContextOptions<WALLETContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accounts> Accounts { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<EmailTemplates> EmailTemplates { get; set; }
        public virtual DbSet<FixedTermDeposits> FixedTermDeposits { get; set; }
        public virtual DbSet<Rates> Rates { get; set; }
        public virtual DbSet<RefundRequest> RefundRequest { get; set; }
        public virtual DbSet<TransactionLog> TransactionLog { get; set; }
        public virtual DbSet<Transactions> Transactions { get; set; }
        public virtual DbSet<Transfers> Transfers { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accounts>(entity =>
            {
                entity.Property(e => e.Currency).HasMaxLength(3);

                entity.Property(e => e.UserId).HasColumnName("User_Id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Accounts__User_I__286302EC");
            });

            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasIndex(e => e.Type)
                    .HasName("UQ__Categori__F9B8A48BDA3C6C66")
                    .IsUnique();

                entity.Property(e => e.Editable).HasDefaultValueSql("((1))");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<EmailTemplates>(entity =>
            {
                entity.Property(e => e.Body)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.Title).HasMaxLength(46);
            });

            modelBuilder.Entity<FixedTermDeposits>(entity =>
            {
                entity.Property(e => e.AccountId).HasColumnName("Account_Id");

                entity.Property(e => e.ClosingDate)
                    .HasColumnName("Closing_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("Creation_Date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.FixedTermDeposits)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FixedTerm__Accou__38996AB5");
            });

            modelBuilder.Entity<Rates>(entity =>
            {
                entity.Property(e => e.BuyingPrice).HasColumnName("Buying_price");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SellingPrice).HasColumnName("Selling_price");
            });

            modelBuilder.Entity<RefundRequest>(entity =>
            {
                entity.Property(e => e.SourceAccountId).HasColumnName("Source_Account_Id");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValueSql("('Pending')");

                entity.Property(e => e.TargetAccountId).HasColumnName("Target_Account_Id");

                entity.Property(e => e.TransactionId).HasColumnName("Transaction_Id");

                entity.HasOne(d => d.SourceAccount)
                    .WithMany(p => p.RefundRequestSourceAccount)
                    .HasForeignKey(d => d.SourceAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RefundReq__Sourc__44FF419A");

                entity.HasOne(d => d.TargetAccount)
                    .WithMany(p => p.RefundRequestTargetAccount)
                    .HasForeignKey(d => d.TargetAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RefundReq__Targe__45F365D3");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.RefundRequest)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RefundReq__Trans__4222D4EF");
            });

            modelBuilder.Entity<TransactionLog>(entity =>
            {
                entity.Property(e => e.ModificationDate)
                    .HasColumnName("Modification_Date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NewValue)
                    .IsRequired()
                    .HasColumnName("New_Value")
                    .HasMaxLength(100);

                entity.Property(e => e.TransactionId).HasColumnName("Transaction_Id");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.TransactionLog)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Trans__3B75D760");
            });

            modelBuilder.Entity<Transactions>(entity =>
            {
                entity.Property(e => e.AccountId).HasColumnName("Account_Id");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("Category_Id")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Concept)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Accou__31EC6D26");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Categ__33D4B598");
            });

            modelBuilder.Entity<Transfers>(entity =>
            {
                entity.Property(e => e.DestinationTransactionId).HasColumnName("Destination_Transaction_Id");

                entity.Property(e => e.OriginTransactionId).HasColumnName("Origin_Transaction_Id");

                entity.HasOne(d => d.DestinationTransaction)
                    .WithMany(p => p.TransfersDestinationTransaction)
                    .HasForeignKey(d => d.DestinationTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transfers__Desti__49C3F6B7");

                entity.HasOne(d => d.OriginTransaction)
                    .WithMany(p => p.TransfersOriginTransaction)
                    .HasForeignKey(d => d.OriginTransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transfers__Origi__48CFD27E");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("UQ__Users__A9D1053435B8A63E")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("First_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("Last_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Password).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

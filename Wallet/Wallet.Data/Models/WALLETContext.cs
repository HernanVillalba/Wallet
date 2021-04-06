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
        public virtual DbSet<FixedTermDeposits> FixedTermDeposits { get; set; }
        public virtual DbSet<Rates> Rates { get; set; }
        public virtual DbSet<RefundRequest> RefundRequest { get; set; }
        public virtual DbSet<TransactionLog> TransactionLog { get; set; }
        public virtual DbSet<Transactions> Transactions { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UserContact> UserContact { get; set; }

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
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
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
                    .HasConstraintName("FK__FixedTerm__Accou__36B12243");
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
                entity.Property(e => e.SourceUserId).HasColumnName("Source_User_Id");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValueSql("('Pending')");

                entity.Property(e => e.TargetUsetId).HasColumnName("Target_Uset_Id");

                entity.Property(e => e.TransactionId).HasColumnName("Transaction_Id");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.SourceUser)
                    .WithMany(p => p.RefundRequestSourceUser)
                    .HasForeignKey(d => d.SourceUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RefundReq__Sourc__4316F928");

                entity.HasOne(d => d.TargetUset)
                    .WithMany(p => p.RefundRequestTargetUset)
                    .HasForeignKey(d => d.TargetUsetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RefundReq__Targe__440B1D61");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.RefundRequest)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RefundReq__Trans__403A8C7D");
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
                    .HasConstraintName("FK__Transacti__Trans__398D8EEE");
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

                entity.Property(e => e.Editable).HasDefaultValueSql("((1))");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Accou__300424B4");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transacti__Categ__31EC6D26");

            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("UQ__Users__A9D1053454151707")
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

            modelBuilder.Entity<UserContact>().HasNoKey();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HBK.Storage.Adapter.Interfaces;
using HBK.Storage.Adapter.Models;
using HBK.Storage.Adapter.StorageCredentials;
using HBK.Storage.Adapter.ValueConversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace HBK.Storage.Adapter.Storages
{
    /// <summary>
    /// HBK Stroage 資料庫實體
    /// </summary>
    public partial class HBKStorageContext : DbContext
    {
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        public HBKStorageContext()
        {
        }
        /// <summary>
        /// 建立一個新的執行個體
        /// </summary>
        /// <param name="options">資料庫選項</param>
        public HBKStorageContext(DbContextOptions<HBKStorageContext> options)
            : base(options)
        {
        }
        /// <summary>
        /// 取得或設定驗證用的資料集
        /// </summary>
        public virtual DbSet<AuthorizeKey> AuthorizeKey { get; set; }
        /// <summary>
        /// 取得或設定檔案位於儲存個體上的操作紀錄資料集
        /// </summary>
        public virtual DbSet<FileEntityStroageOperation> FileEntityStroageOperation { get; set; }
        /// <summary>
        /// 取得或設定檔案存取權杖資料集
        /// </summary>
        public virtual DbSet<FileAccessToken> FileAccessToken { get; set; }
        /// <summary>
        /// 取得或設定檔案實體資料集
        /// </summary>
        public virtual DbSet<FileEntity> FileEntity { get; set; }
        /// <summary>
        /// 取得或設定檔案位於儲存個體上的橋接資訊資料集
        /// </summary>
        public virtual DbSet<FileEntityStorage> FileEntityStorage { get; set; }
        /// <summary>
        /// 取得或設定儲存個體資料集
        /// </summary>
        public virtual DbSet<Storage> Storage { get; set; }
        /// <summary>
        /// 取得或設定儲存個體群組資料集
        /// </summary>
        public virtual DbSet<StorageGroup> StorageGroup { get; set; }
        /// <summary>
        /// 取得或設定儲存服務資料集
        /// </summary>
        public virtual DbSet<StorageProvider> StorageProvider { get; set; }

        /// <summary>
        /// 設定資料庫實體
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=HBKStorage;Integrated Security=True");
            }
        }

        /// <summary>
        /// 建立資料庫實體
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");

            modelBuilder.Entity<AuthorizeKey>(entity =>
            {
                entity.HasKey(e => e.AuthorizeKeyId)
                    .IsClustered(false);

                entity.HasIndex(e => e.AuthorizeKeyId)
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.AuthorizeKeyId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AuthorizeKeyNo).ValueGeneratedOnAdd();

                // SoftDelete
                entity.HasQueryFilter(model => model.DeleteDateTime == null);
            });

            modelBuilder.Entity<AuthorizeKeyScope>(entity =>
            {
                entity.Property(e => e.AuthorizeKeyScopeNo).ValueGeneratedNever();

                entity.HasOne(d => d.AuthorizeKey)
                    .WithMany(p => p.AuthorizeKeyScope)
                    .HasForeignKey(d => d.AuthorizeKeyId)
                    .HasConstraintName("FK_AuthorizeKeyScope_AuthorizeKey");

                entity.HasOne(d => d.StorageProvider)
                    .WithMany(p => p.AuthorizeKeyScope)
                    .HasForeignKey(d => d.StorageProviderId)
                    .HasConstraintName("FK_AuthorizeKeyScope_StorageProvider");
            });

            modelBuilder.Entity<FileEntityStroageOperation>(entity => 
            {
                entity.HasKey(e => e.FileEntityStroageOperationNo)
                    .IsClustered();

                entity.Property(e => e.FileEntityStroageOperationNo).ValueGeneratedOnAdd();

                entity.Property(e => e.FileEntityStroageId);

                entity.HasOne(d => d.FileEntityStroage)
                    .WithMany(p => p.FileEntityStroageOperation)
                    .HasForeignKey(d => d.FileEntityStroageId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_FileEntityStroageOperation_FileEntityStroage");

                entity.HasOne(d => d.Storage)
                   .WithMany(p => p.FileEntityStroageOperation)
                   .HasForeignKey(d => d.SyncTargetStorageId)
                   .HasConstraintName("FK_FileEntityStroageOperation_Storage");
            });

            modelBuilder.Entity<FileAccessToken>(entity =>
            {
                entity.HasKey(e => e.FileAccessTokenId)
                    .IsClustered(false);

                entity.HasIndex(e => e.FileAccessTokenNo, "IX_FileAccessToken")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.FileAccessTokenId)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.FileAccessTokenNo).ValueGeneratedOnAdd();

                entity.Property(e => e.FileEntityId);

                entity.Property(e => e.StorageGroupId);

                entity.Property(e => e.StorageProviderId);

                entity.Property(e => e.Token)
                    .IsUnicode(false);

                entity.HasOne(d => d.FileEntity)
                    .WithMany(p => p.FileAccessToken)
                    .HasForeignKey(d => d.FileEntityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FileAccessToken_FileEntity");

                entity.HasOne(d => d.StorageGroup)
                    .WithMany(p => p.FileAccessToken)
                    .HasForeignKey(d => d.StorageGroupId)
                    .HasConstraintName("FK_FileAccessToken_StorageGroup");

                entity.HasOne(d => d.StorageProvider)
                    .WithMany(p => p.FileAccessToken)
                    .HasForeignKey(d => d.StorageProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FileAccessToken_StorageProvider");
            });

            modelBuilder.Entity<FileEntityTag>(entity => 
            {
                entity.HasKey(e => e.FileEntityTagNo)
                    .IsClustered();

                entity.Property(e => e.FileEntityTagNo).ValueGeneratedOnAdd();

                entity.HasOne(d => d.FileEntity)
                    .WithMany(p => p.FileEntityTag)
                    .HasForeignKey(d => d.FileEntityId)
                    .HasConstraintName("FK_FileEntityTag_FileEntity");
            });

            modelBuilder.Entity<FileEntity>(entity =>
            {
                entity.HasKey(e => e.FileEntityId)
                    .IsClustered(false);

                entity.HasIndex(e => e.FileEntityNo, "IX_FileEntity")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.FileEntityId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.FileEntityNo).ValueGeneratedOnAdd();

                entity.HasOne(d => d.ParentFileEntity)
                    .WithMany(p => p.ChildFileEntitiy)
                    .HasForeignKey(d => d.ParentFileEntityID)
                    .HasConstraintName("FK_ChildFileEntity_ParentFileEntity");

                // SoftDelete
                entity.HasQueryFilter(model => model.DeleteDateTime == null);
            });

            modelBuilder.Entity<FileEntityStorage>(entity =>
            {
                entity.HasKey(e => e.FileEntityStorageId)
                    .IsClustered(false);

                entity.HasIndex(e => e.FileEntityStorageNo, "IX_FileEntityStroage")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.FileEntityStorageId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.FileEntityStorageNo).ValueGeneratedOnAdd();

                entity.HasOne(d => d.FileEntity)
                    .WithMany(p => p.FileEntityStroage)
                    .HasForeignKey(d => d.FileEntityId)
                    .HasConstraintName("FK_FileEntityStroage_FileEntity");

                entity.HasOne(d => d.Storage)
                    .WithMany(p => p.FileEntityStroage)
                    .HasForeignKey(d => d.StorageId)
                    .HasConstraintName("FK_FileEntityStroage_Storage");

                // SoftDelete
                entity.HasQueryFilter(model => model.DeleteDateTime == null);
            });

            modelBuilder.Entity<Storage>(entity =>
            {
                entity.HasKey(e => e.StorageId)
                    .IsClustered(false);

                entity.HasIndex(e => e.StorageNo, "IX_Storage")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.StorageId).ValueGeneratedNever();

                entity.Property(e => e.Credentials)
                    .HasConversion(new StorageCredentialsConverter())
                    .Metadata.SetValueComparer(new StorageCredentialsValueComparer());

                entity.HasOne(d => d.StorageGroup)
                    .WithMany(p => p.Storage)
                    .HasForeignKey(d => d.StorageGroupId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Storage_StorageGroup");

                entity.Property(e => e.StorageId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.StorageNo).ValueGeneratedOnAdd();

                // SoftDelete
                entity.HasQueryFilter(model => model.DeleteDateTime == null);
            });

            modelBuilder.Entity<StorageGroup>(entity =>
            {
                entity.HasKey(e => e.StorageGroupId)
                    .IsClustered(false);

                entity.HasIndex(e => e.StorageGroupNo, "IX_StorageGroup")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.StorageGroupId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.StorageGroupNo).ValueGeneratedOnAdd();

                entity.Property(e => e.SyncPolicy)
                    .HasConversion(new JsonParseConverter<SyncPolicy>());

                entity.HasOne(d => d.StorageProvider)
                    .WithMany(p => p.StorageGroup)
                    .HasForeignKey(d => d.StorageProviderId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_StorageGroup_StorageProvider");

                // SoftDelete
                entity.HasQueryFilter(model => model.DeleteDateTime == null);
            });

            modelBuilder.Entity<StorageProvider>(entity =>
            {
                entity.HasKey(e => e.StorageProviderId)
                    .IsClustered(false);

                entity.HasIndex(e => e.StorageProviderNo, "IX_StorageProvider")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.StorageProviderId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.StorageProviderNo).ValueGeneratedOnAdd();

                // SoftDelete
                entity.HasQueryFilter(model => model.DeleteDateTime == null);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        /// <summary>
        /// 儲存變更
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <returns></returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplySaveChangesPolicies();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// 以非同步方式儲存變更
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            this.ApplySaveChangesPolicies();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// 套用儲存變更政策
        /// </summary>
        protected virtual void ApplySaveChangesPolicies()
        {
            base.ChangeTracker.DetectChanges();

            // Deleted
            var deletedEntries = base.ChangeTracker.Entries()
                .Where(e => e.Entity is ISoftDeleteModel && e.State == EntityState.Deleted);

            foreach (var entityEntry in deletedEntries)
            {
                entityEntry.State = EntityState.Unchanged;
                ((ISoftDeleteModel)entityEntry.Entity).DeleteDateTime = DateTimeOffset.UtcNow;
            }

            // Added
            var addedEntries = base.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added);

            foreach (var entityEntry in addedEntries)
            {
                if (entityEntry.Entity is ICreatedDateModel createdDateModel)
                {
                    createdDateModel.CreateDateTime = DateTimeOffset.UtcNow;
                }
            }

            // Updated
            var modifiedEntries = base.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entityEntry in modifiedEntries)
            {
                if (entityEntry.Entity is ITimeStampModel timeStampModel)
                {
                    timeStampModel.UpdateDateTime = DateTimeOffset.UtcNow;
                }
            }
        }
    }
}
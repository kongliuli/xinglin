using Microsoft.EntityFrameworkCore;
using Xinglin.Core.Models;
using Xinglin.Core.Models.Authorization;

namespace Xinglin.Infrastructure.Data
{
    /// <summary>
    /// 云端数据库上下文
    /// 用于管理医院信息、机器码、激活码、权限和模板等数据
    /// </summary>
    public class CloudDbContext : DbContext
    {
        /// <summary>
        /// 医院表
        /// </summary>
        public DbSet<Hospital> Hospitals { get; set; }

        /// <summary>
        /// 机器码表
        /// </summary>
        public DbSet<MachineCode> MachineCodes { get; set; }

        /// <summary>
        /// 激活码表
        /// </summary>
        public DbSet<ActivationCode> ActivationCodes { get; set; }

        /// <summary>
        /// 权限表
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// 报告模板表
        /// </summary>
        public DbSet<ReportTemplate> ReportTemplates { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">数据库上下文选项</param>
        public CloudDbContext(DbContextOptions<CloudDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// 配置模型
        /// </summary>
        /// <param name="modelBuilder">模型构建器</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置医院表
            modelBuilder.Entity<Hospital>()
                .HasKey(h => h.Id);

            modelBuilder.Entity<Hospital>()
                .Property(h => h.Id)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Hospital>()
                .Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Hospital>()
                .Property(h => h.Code)
                .HasMaxLength(50);

            modelBuilder.Entity<Hospital>()
                .Property(h => h.Status)
                .HasMaxLength(20);

            // 配置机器码表
            modelBuilder.Entity<MachineCode>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<MachineCode>()
                .Property(m => m.Id)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<MachineCode>()
                .Property(m => m.Code)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<MachineCode>()
                .Property(m => m.HospitalId)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<MachineCode>()
                .Property(m => m.Status)
                .HasMaxLength(20);

            // 配置激活码表
            modelBuilder.Entity<ActivationCode>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<ActivationCode>()
                .Property(a => a.Id)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<ActivationCode>()
                .Property(a => a.Code)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<ActivationCode>()
                .Property(a => a.HospitalId)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<ActivationCode>()
                .Property(a => a.LicenseType)
                .HasMaxLength(20);

            modelBuilder.Entity<ActivationCode>()
                .Property(a => a.Status)
                .HasMaxLength(20);

            // 配置权限表
            modelBuilder.Entity<Permission>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Permission>()
                .Property(p => p.Id)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Permission>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Permission>()
                .Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(50);

            // 配置报告模板表
            modelBuilder.Entity<ReportTemplate>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<ReportTemplate>()
                .Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<ReportTemplate>()
                .Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<ReportTemplate>()
                .Property(t => t.Version)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<ReportTemplate>()
                .Property(t => t.Type)
                .HasMaxLength(50);

            modelBuilder.Entity<ReportTemplate>()
                .Property(t => t.HospitalId)
                .HasMaxLength(50);
        }
    }
}
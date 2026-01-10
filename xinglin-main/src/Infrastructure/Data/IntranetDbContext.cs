using Microsoft.EntityFrameworkCore;
using Xinglin.Core.Models;
using Xinglin.Core.Models.Authorization;

namespace Xinglin.Infrastructure.Data
{
    /// <summary>
    /// 内网数据库上下文
    /// 用于管理报告归档、查询日志等数据
    /// </summary>
    public class IntranetDbContext : DbContext
    {
        /// <summary>
        /// 报告表
        /// </summary>
        public DbSet<Report> Reports { get; set; }

        /// <summary>
        /// 报告查询日志表
        /// </summary>
        public DbSet<ReportQueryLog> ReportQueryLogs { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">数据库上下文选项</param>
        public IntranetDbContext(DbContextOptions<IntranetDbContext> options)
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

            // 配置报告表
            modelBuilder.Entity<Report>()
                .HasKey(r => r.ReportNumber);

            modelBuilder.Entity<Report>()
                .Property(r => r.ReportNumber)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Report>()
                .Property(r => r.Status)
                .HasMaxLength(20);

            modelBuilder.Entity<Report>()
                .HasMany(r => r.ReportItems)
                .WithOne()
                .HasForeignKey(ri => ri.ReportNumber);

            // 配置报告查询日志表
            modelBuilder.Entity<ReportQueryLog>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<ReportQueryLog>()
                .Property(l => l.Id)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<ReportQueryLog>()
                .Property(l => l.ReportNumber)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<ReportQueryLog>()
                .Property(l => l.QueryIdentifier)
                .HasMaxLength(100);

            modelBuilder.Entity<ReportQueryLog>()
                .Property(l => l.QueryMethod)
                .HasMaxLength(50);

            modelBuilder.Entity<ReportQueryLog>()
                .Property(l => l.QueryResult)
                .HasMaxLength(50);

            modelBuilder.Entity<ReportQueryLog>()
                .Property(l => l.IpAddress)
                .HasMaxLength(50);

            modelBuilder.Entity<ReportQueryLog>()
                .Property(l => l.BrowserInfo)
                .HasMaxLength(200);
        }
    }
}
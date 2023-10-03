using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRabbitMQ.Repository.Context
{
    public class OutboxMessageDbContext : DbContext
    {
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public OutboxMessageDbContext(DbContextOptions<OutboxMessageDbContext> options)
       : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.ConnectionName).HasColumnType("VARCHAR(10)").IsRequired();
                entity.Property(e => e.MessageData).HasColumnType("TEXT").IsRequired();
                entity.Property(e => e.IsProcessed).HasColumnType("BOOL");
                entity.Property(e => e.CreatedAt).HasColumnType("DATETIME").HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();
                entity.Property(e => e.ModifiedAt).HasColumnType("DATETIME");
                entity.Property(e => e.ExchangeName).HasColumnType("VARCHAR(255)").IsRequired();
                entity.Property(e => e.RoutingKey).HasColumnType("VARCHAR(255)").IsRequired();
            });
        }
    }
}

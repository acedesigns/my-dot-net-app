/* =======================================================
 *
 * Created by anele on 27/08/2025.
 *
 * @anele_ace
 *
 * =======================================================
 */

using MyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MyApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Title).HasMaxLength(100).IsRequired();
                entity.Property(p => p.Content).HasColumnType("TEXT").IsRequired();
                entity.Property(p => p.Likes).IsRequired(false);
                entity.Property(p => p.CreatedAt);
            });
        }
    }
}
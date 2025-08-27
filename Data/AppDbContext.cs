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
        public DbSet<User> Users { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ----- Post -----
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Title).HasMaxLength(100).IsRequired();
                entity.Property(p => p.Content).HasColumnType("TEXT").IsRequired();
                entity.Property(p => p.CreatedAt);

                // Post has one author
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Posts)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ----- User -----
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(c => c.CreatedAt);
            });

            // ----- Comment -----
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Content).IsRequired();
                entity.Property(c => c.CreatedAt);

                entity.HasOne(c => c.Post)
                      .WithMany(p => p.Comments)
                      .HasForeignKey(c => c.PostId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ----- Like -----
            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasKey(l => l.Id);

                // Each user can like a post only once
                entity.HasIndex(l => new { l.UserId, l.PostId }).IsUnique();

                entity.HasOne(l => l.Post)
                      .WithMany(p => p.Likes)
                      .HasForeignKey(l => l.PostId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.User)
                      .WithMany(u => u.Likes)
                      .HasForeignKey(l => l.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
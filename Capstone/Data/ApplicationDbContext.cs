using Capstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace Capstone.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public DbSet<Template> Templates { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TemplateTag> TemplateTags { get; set; }
        public DbSet<TemplateUser> TempateUsers { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<Answer> Answers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<ApplicationUser>()
                  .Property<NpgsqlTsVector>("SearchVector")
                  .HasColumnType("tsvector") 
                  .HasComputedColumnSql("to_tsvector('english', coalesce(\"UserName\", '') || ' ' || coalesce(\"Email\", '') || ' ' || coalesce(\"FirstName\", '') || ' ' || coalesce(\"LastName\", ''))", stored: true);
            
            modelBuilder.Entity<ApplicationUser>()
                  .HasIndex("SearchVector")
                  .HasMethod("GIN");
            
            modelBuilder.Entity<Topic>(entity =>
            {
                entity.HasKey(t => t.TopicId);
                
                entity.Property(t => t.TopicId)
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
                
                entity.HasIndex(t => t.TopicName)
                      .IsUnique();
                
            });
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(t => t.TagId);
                entity.Property(t => t.TagId)
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
                entity.HasIndex(t => t.TagName)
                      .HasMethod("GIN")
                      .IsTsVectorExpressionIndex("english");
            });
            
            modelBuilder.Entity<Template>(entity =>
            {
                entity.HasKey(t => t.TemplateId);
                
                entity.Property(t => t.TemplateId)
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
                
                entity.HasOne(t => t.User)
                      .WithMany(u => u.Templates)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(q => q.QuestionId);
                
                entity.Property(q => q.QuestionId)
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
                
                entity.HasOne(q => q.Template)
                      .WithMany(t => t.Questions)
                      .HasForeignKey(q => q.TemplateId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.CommentId);
                
                entity.Property(c => c.CommentId)
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
                
                entity.HasOne(c => c.Template)
                      .WithMany(t => t.Comments)
                      .HasForeignKey(c => c.TemplateId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasKey(l => l.LikeId);
                
                entity.Property(l => l.LikeId)
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
                
                entity.HasOne(l => l.Template)
                      .WithMany(t => t.Likes)
                      .HasForeignKey(l => l.TemplateId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(l => l.User)
                      .WithMany(u => u.Likes)
                      .HasForeignKey(l => l.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasIndex(l => new { l.TemplateId, l.UserId })
                      .IsUnique();
                
            });
            
            modelBuilder.Entity<TemplateTag>(entity =>
            {
                entity.HasKey(tt => new { tt.TemplateId, tt.TagId });
                
                entity.HasOne(tt => tt.Template)
                      .WithMany(t => t.TemplateTags)
                      .HasForeignKey(tt => tt.TemplateId);
                
                entity.HasOne(tt => tt.Tag)
                      .WithMany(tg => tg.TemplateTags)
                      .HasForeignKey(tt => tt.TagId);
            });

            modelBuilder.Entity<TemplateUser>(entity =>
            {
                  entity.HasKey(tu => new { tu.UserId, tu.TemplateId });

                  entity.HasOne(tu => tu.User)
                        .WithMany(u => u.TemplateUsers)
                        .HasForeignKey(tu => tu.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

                  entity.HasOne(tu => tu.Template)
                        .WithMany(t => t.TemplateUsers)
                        .HasForeignKey(tu => tu.TemplateId)
                        .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<Form>(entity =>
            {
                entity.HasKey(f => f.FormId);

                entity.Property(f => f.FormId)
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
                
                entity.HasOne(f => f.Template)
                      .WithMany(t => t.Forms)
                      .HasForeignKey(f => f.TemplateId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(f => f.User)
                      .WithMany(u => u.Forms)
                      .HasForeignKey(f => f.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
                
            });
            
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(a => a.AnswerId);
                
                entity.Property(a => a.AnswerId)
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
                
                entity.HasOne(a => a.Form)
                      .WithMany(f => f.Answers)
                      .HasForeignKey(a => a.FormId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(a => a.Question)
                      .WithMany(q => q.Answers)
                      .HasForeignKey(a => a.QuestionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
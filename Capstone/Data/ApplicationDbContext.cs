using Capstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<TemplateUser> TemplateUsers { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<OptionAnswer> OptionAnswers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.TemplateId, l.UserId })
                .IsUnique();
            
            modelBuilder.Entity<TemplateTag>()
                .HasKey(tt => new { tt.TemplateId, tt.TagId });
            
            modelBuilder.Entity<Template>(entity =>
            {
                entity.HasOne(t => t.User)
                    .WithMany(u => u.Templates)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(c => c.Template)
                    .WithMany(t => t.Comments)
                    .HasForeignKey(c => c.TemplateId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<QuestionOption>(entity =>
            {
                entity.HasOne(qo => qo.Question)
                    .WithMany(q => q.QuestionOptions)
                    .HasForeignKey(qo => qo.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasOne(a => a.Question)
                    .WithMany(q => q.Answers)
                    .HasForeignKey(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Template)
                    .WithMany(t => t.Answers)
                    .HasForeignKey(a => a.TemplateId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OptionAnswer>(entity =>
            { 
                entity.HasOne(oa => oa.Answer)
                    .WithMany(a => a.OptionAnswers)
                    .HasForeignKey(oa => oa.AnswerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oa => oa.QuestionOption)
                    .WithMany(qo => qo.OptionAnswers)
                    .HasForeignKey(oa => oa.QuestionOptionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasOne<Template>(t => t.Template)
                    .WithMany(t => t.Questions)
                    .HasForeignKey(t => t.TemplateId)
                    .OnDelete(DeleteBehavior.Cascade);
                
            });
        }
    }
}
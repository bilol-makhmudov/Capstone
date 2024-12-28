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
            
            modelBuilder.Entity<ApplicationUser>()
                  .Property<NpgsqlTsVector>("SearchVector")
                  .HasColumnType("tsvector") 
                  .HasComputedColumnSql("to_tsvector('english', coalesce(\"UserName\", '') || ' ' || coalesce(\"Email\", '') || ' ' || coalesce(\"FirstName\", '') || ' ' || coalesce(\"LastName\", ''))", stored: true);
            
            modelBuilder.Entity<ApplicationUser>()
                  .HasIndex("SearchVector")
                  .HasMethod("GIN");
            
            modelBuilder.Entity<Topic>(entity =>
            {
                entity.HasKey(t => t.Id);
                
                entity.HasIndex(t => t.TopicName)
                      .IsUnique();
                
            });
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(t => t.Id);
                
                entity.HasIndex(t => t.TagName)
                      .HasMethod("GIN")
                      .IsTsVectorExpressionIndex("english");
            });
            
            modelBuilder.Entity<Template>(entity =>
            {
                entity.HasKey(t => t.Id);
                
                entity.HasOne(t => t.User)
                      .WithMany(u => u.Templates)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(q => q.Id);
                
                entity.HasOne(q => q.Template)
                      .WithMany(t => t.Questions)
                      .HasForeignKey(q => q.TemplateId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<QuestionOption>(entity =>
            {
                  entity.HasKey(q => q.Id);
                  
                  entity.HasOne(q => q.Question)
                        .WithMany(q => q.QuestionOptions)
                        .HasForeignKey(q => q.QuestionId)
                        .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);
                
                entity.HasOne(c => c.Template)
                      .WithMany(t => t.Comments)
                      .HasForeignKey(c => c.TemplateId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            modelBuilder.Entity<Like>()
                  .HasIndex(l => new { l.TemplateId, l.UserId })
                  .IsUnique();
            
            modelBuilder.Entity<Like>(entity =>
            {
                  
                  entity.HasIndex(l => new { l.TemplateId, l.UserId })
                        .IsUnique(); 
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
            
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(a => a.Id);
                
                entity.HasOne(a => a.Template)
                      .WithMany(f => f.Answers)
                      .HasForeignKey(a => a.TemplateId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(a => a.Question)
                      .WithMany(q => q.Answers)
                      .HasForeignKey(a => a.QuestionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OptionAnswer>(entity =>
            {
                  entity.HasKey(a => a.Id);
                  entity.HasOne(a => a.Answer)
                        .WithMany(f => f.OptionAnswers)
                        .HasForeignKey(a => a.AnswerId)
                        .OnDelete(DeleteBehavior.Cascade);
                
                  entity.HasOne(a => a.QuestionOption)
                        .WithMany(q => q.OptionAnswers)
                        .HasForeignKey(a => a.QuestionOptionId)
                        .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
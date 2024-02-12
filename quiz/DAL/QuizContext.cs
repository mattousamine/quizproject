using Microsoft.EntityFrameworkCore;
using quiz.Models;

namespace quiz.DAL
{
    public class QuizContext : DbContext
    {
        public QuizContext(DbContextOptions<QuizContext> options) : base(options)
        {
        }

        public DbSet<UserQuiz> UserQuizzes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<QuizSession> QuizSessions { get; set; }
        public DbSet<QuizIssue> QuizIssues { get; set; }
        public DbSet<CategoryQuiz> CategoryQuizzes { get; set; }
        public DbSet<QuizTracker> QuizTrackers { get; set; }
        public DbSet<UserIssue> UserIssues { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure primary keys
            modelBuilder.Entity<UserQuiz>().HasKey(uq => uq.UserQuizId);
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<Quiz>().HasKey(q => q.QuizId);
            modelBuilder.Entity<Question>().HasKey(q => q.QuestionId);
            modelBuilder.Entity<Option>().HasKey(o => o.OptionId);
            modelBuilder.Entity<QuizSession>().HasKey(qs => qs.QuizSessionId);
            modelBuilder.Entity<QuizIssue>().HasKey(qi => qi.QuizIssueId);
            modelBuilder.Entity<CategoryQuiz>().HasKey(cq => cq.CategoryQuizId);
            modelBuilder.Entity<QuizTracker>().HasKey(qt => qt.QuizTrackerId);
            modelBuilder.Entity<UserIssue>().HasKey(ui => ui.UserIssueId);
            modelBuilder.Entity<QuizQuestion>().HasKey(qq => qq.QuizQuestionId);
            modelBuilder.Entity<QuestionOption>().HasKey(qo => qo.QuestionOptionId);

            // Configure relationships
            modelBuilder.Entity<CategoryQuiz>()
                .HasOne(cq => cq.Category)
                .WithMany()
                .HasForeignKey(cq => cq.CategoryId);

            modelBuilder.Entity<CategoryQuiz>()
                .HasOne(cq => cq.Quiz)
                .WithMany()
                .HasForeignKey(cq => cq.QuizId);

            modelBuilder.Entity<QuizTracker>()
                .HasOne(qt => qt.UserQuiz)
                .WithMany()
                .HasForeignKey(qt => qt.UserQuizId);

            modelBuilder.Entity<QuizTracker>()
                .HasOne(qt => qt.QuizSession)
                .WithMany()
                .HasForeignKey(qt => qt.QuizSessionId);

            modelBuilder.Entity<QuizTracker>()
                .HasOne(qt => qt.Quiz)
                .WithMany()
                .HasForeignKey(qt => qt.QuizId);

            modelBuilder.Entity<UserIssue>()
                .HasOne(ui => ui.UserQuiz)
                .WithMany()
                .HasForeignKey(ui => ui.UserQuizId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserIssue>()
                .HasOne(ui => ui.QuizIssue)
                .WithMany()
                .HasForeignKey(ui => ui.QuizIssueId);

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(qq => qq.Quiz)
                .WithMany()
                .HasForeignKey(qq => qq.QuizId);

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(qq => qq.Question)
                .WithMany()
                .HasForeignKey(qq => qq.QuestionId);

            modelBuilder.Entity<QuestionOption>()
                .HasOne(qo => qo.Question)
                .WithMany()
                .HasForeignKey(qo => qo.QuestionId);

            modelBuilder.Entity<QuestionOption>()
                .HasOne(qo => qo.Option)
                .WithMany()
                .HasForeignKey(qo => qo.OptionId);
        }
    } 
}

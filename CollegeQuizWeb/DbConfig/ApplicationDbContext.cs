using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CollegeQuizWeb.Config;
using CollegeQuizWeb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CollegeQuizWeb.DbConfig;

public class ApplicationDbContext : DbContext
{
    ///////////////////// mappers //////////////////////////////////////////////////////////////////////////////////////
    
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<OtaTokenEntity> OtaTokens { get; set; }
    public DbSet<AnswerEntity> Answers { get; set; }
    public DbSet<QuestionEntity> Questions { get; set; }
    public DbSet<QuizEntity> Quizes { get; set; }
    public DbSet<QuizLobbyEntity> QuizLobbies { get; set; }
    public DbSet<CouponEntity> Coupons { get; set; }
    public DbSet<QuizSessionParticEntity> QuizSessionPartics { get; set; }
    public DbSet<SubscriptionTypesEntity> SubsciptionTypes { get; set; }
    public DbSet<ClientAddressEntity> ClientsAddresses { get; set; }
    public DbSet<SubscriptionPaymentHistoryEntity> SubscriptionsPaymentsHistory { get; set; }
    public DbSet<SharedQuizesEntity> SharedQuizes { get; set; }
    public DbSet<UsersQuestionsAnswersEntity> UsersQuestionsAnswers { get; set; }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseMySql(
                ConfigLoader.DbConnectionString,
                new MySqlServerVersion(ConfigLoader.DbVersion))
            .UseLoggerFactory(LoggerFactory.Create(factory => factory
                .AddConsole()
                .AddFilter(level => level >= LogLevel.Information)
            ))
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // before model creating, ex. relational mapping
        modelBuilder.Entity<SharedQuizesEntity>().HasKey(e => new { e.QuizId, e.UserId });
        modelBuilder.Entity<SharedQuizesEntity>()
            .HasOne(e => e.QuizEntity)
            .WithMany(e => e.SharedQuizesEntities)
            .HasForeignKey(e => e.QuizId);
        modelBuilder.Entity<SharedQuizesEntity>()
            .HasOne(e => e.UserEntity)
            .WithMany(e => e.SharedQuizesEntities)
            .HasForeignKey(e => e.UserId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        DateTime d = DateTime.Now;
        
        IEnumerable<EntityEntry> entitiesWithPrimaryKey = ChangeTracker.Entries()
            .Where(x => x.Entity is AbstractAuditableEntity
                        && (x.State == EntityState.Added || x.State == EntityState.Modified));
        
        foreach (var entityEntry in entitiesWithPrimaryKey) {
            DateTime formatedDateTime = new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Millisecond);
            if (entityEntry.State == EntityState.Added) {
                ((AbstractAuditableEntity)entityEntry.Entity).CreatedAt = formatedDateTime;
            }
            ((AbstractAuditableEntity) entityEntry.Entity).UpdatedAt = formatedDateTime;
        }
        return await base.SaveChangesAsync(cancellationToken);
    }

    public static void AddDatabaseConfiguration(IServiceCollection service, IConfiguration config)
    {
        service.AddDbContext<ApplicationDbContext>(options => 
            options.UseMySql(
                ConfigLoader.DbConnectionString,
                new MySqlServerVersion(ConfigLoader.DbVersion),
                opt =>
                {
                    opt.EnableStringComparisonTranslations();
                    opt.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(5), null);
                }
        ));
    }
}
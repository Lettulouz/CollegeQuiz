﻿// <auto-generated />
using System;
using CollegeQuizWeb.DbConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CollegeQuizWeb.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("CollegeQuizWeb.Entities.AnswerEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsGood")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_good");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("name");

                    b.Property<long>("QuestionId")
                        .HasColumnType("bigint")
                        .HasColumnName("question_id");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("answers");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.ClientAddressEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("Address1")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("address1");

                    b.Property<string>("Address2")
                        .HasColumnType("longtext")
                        .HasColumnName("address2");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("country");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext")
                        .HasColumnName("phone_number");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("state");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("client_address");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.CouponEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("varchar(80)")
                        .HasColumnName("customer_name");

                    b.Property<DateTime>("ExpiringAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("expiring_at");

                    b.Property<int>("ExtensionTime")
                        .HasColumnType("int")
                        .HasColumnName("extension_time");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_used");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("token");

                    b.Property<int>("TypeOfSubscription")
                        .HasColumnType("int")
                        .HasColumnName("type_of_subscription");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.ToTable("coupons");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.OtaTokenEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("ExpiredAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("expired_at");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_used");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasColumnName("token");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ota_tokens");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.QuestionEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<int>("Index")
                        .HasColumnType("int")
                        .HasColumnName("index");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("name");

                    b.Property<long>("QuizId")
                        .HasColumnType("bigint")
                        .HasColumnName("quiz_id");

                    b.Property<int>("TimeMin")
                        .HasColumnType("int")
                        .HasColumnName("time_min");

                    b.Property<int>("TimeSec")
                        .HasColumnType("int")
                        .HasColumnName("time_sec");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("QuizId");

                    b.ToTable("questions");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.QuizEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_public");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("name");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("quizes");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.QuizLobbyEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("varchar(5)")
                        .HasColumnName("code");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<int>("CurrentQuestion")
                        .HasColumnType("int")
                        .HasColumnName("current_question");

                    b.Property<string>("HostConnId")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("host_conn_id");

                    b.Property<string>("InGameScreen")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("in_game_screen");

                    b.Property<bool>("IsEstabilished")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_estabilished");

                    b.Property<long>("QuizId")
                        .HasColumnType("bigint")
                        .HasColumnName("quiz_id");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserHostId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_host_id");

                    b.HasKey("Id");

                    b.HasIndex("QuizId");

                    b.HasIndex("UserHostId");

                    b.ToTable("quiz_lobby");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.QuizSessionParticEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<string>("ConnectionId")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("connection_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<int>("CurrentStreak")
                        .HasColumnType("int")
                        .HasColumnName("current_streak");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_active");

                    b.Property<long>("ParticipantId")
                        .HasColumnType("bigint")
                        .HasColumnName("participant_id");

                    b.Property<long>("QuizLobbyId")
                        .HasColumnType("bigint")
                        .HasColumnName("quiz_lobby_id");

                    b.Property<long>("Score")
                        .HasColumnType("bigint")
                        .HasColumnName("score");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.HasIndex("QuizLobbyId");

                    b.ToTable("quiz_session_participants");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.SharedQuizesEntity", b =>
                {
                    b.Property<long>("QuizId")
                        .HasColumnType("bigint")
                        .HasColumnName("quiz_id");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("QuizId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("shared_quizes");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.ShareTokensEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<long>("QuizId")
                        .HasColumnType("bigint")
                        .HasColumnName("quiz_id");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("varchar(12)")
                        .HasColumnName("token");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("QuizId");

                    b.ToTable("quiz_tokens");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.SubscriptionPaymentHistoryEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("PayuId")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("payu_order_id");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(65,30)")
                        .HasColumnName("price");

                    b.Property<string>("Subscription")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("subscription");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("subscription_payment_history");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.SubscriptionTypesEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<double?>("CurrentDiscount")
                        .HasColumnType("double")
                        .HasColumnName("current_discount");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(65,30)")
                        .HasColumnName("price");

                    b.Property<int>("SiteId")
                        .HasColumnType("int")
                        .HasColumnName("site_id");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.ToTable("subscription_types");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.UserEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<int>("AccountStatus")
                        .HasColumnType("int")
                        .HasColumnName("account_status");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("CurrentStatusExpirationDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("current_status_expiration_date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("first_name");

                    b.Property<bool>("IsAccountActivated")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_account_activated");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_admin");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("last_name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("password");

                    b.Property<bool>("RulesAccept")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("rules_accept");

                    b.Property<int>("TeamID")
                        .HasColumnType("int")
                        .HasColumnName("team_id");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("username");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.UsersQuestionsAnswersEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    b.Property<int>("Answer")
                        .HasColumnType("int")
                        .HasColumnName("answer");

                    b.Property<long>("ConnectionId")
                        .HasColumnType("bigint")
                        .HasColumnName("id_of_connections");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<int>("Question")
                        .HasColumnType("int")
                        .HasColumnName("question");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("ConnectionId");

                    b.ToTable("users_questions_answers");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.AnswerEntity", b =>
                {
                    b.HasOne("CollegeQuizWeb.Entities.QuestionEntity", "QuestionEntity")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuestionEntity");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.ClientAddressEntity", b =>
                {
                    b.HasOne("CollegeQuizWeb.Entities.UserEntity", "UserEntity")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserEntity");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.OtaTokenEntity", b =>
                {
                    b.HasOne("CollegeQuizWeb.Entities.UserEntity", "UserEntity")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserEntity");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.QuestionEntity", b =>
                {
                    b.HasOne("CollegeQuizWeb.Entities.QuizEntity", "QuizEntity")
                        .WithMany()
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuizEntity");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.QuizEntity", b =>
                {
                    b.HasOne("CollegeQuizWeb.Entities.UserEntity", "UserEntity")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserEntity");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.QuizLobbyEntity", b =>
                {
                    b.HasOne("CollegeQuizWeb.Entities.QuizEntity", "QuizEntity")
                        .WithMany()
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CollegeQuizWeb.Entities.UserEntity", "UserEntity")
                        .WithMany()
                        .HasForeignKey("UserHostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuizEntity");

                    b.Navigation("UserEntity");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.QuizSessionParticEntity", b =>
                {
                    b.HasOne("CollegeQuizWeb.Entities.UserEntity", "UserEntity")
                        .WithMany()
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CollegeQuizWeb.Entities.QuizLobbyEntity", "QuizLobbyEntity")
                        .WithMany()
                        .HasForeignKey("QuizLobbyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuizLobbyEntity");

                    b.Navigation("UserEntity");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.SharedQuizesEntity", b =>
                {
                    b.HasOne("CollegeQuizWeb.Entities.QuizEntity", "QuizEntity")
                        .WithMany("SharedQuizesEntities")
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CollegeQuizWeb.Entities.UserEntity", "UserEntity")
                        .WithMany("SharedQuizesEntities")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuizEntity");

                    b.Navigation("UserEntity");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.ShareTokensEntity", b =>
                {
                    b.HasOne("CollegeQuizWeb.Entities.QuizEntity", "QuizEntity")
                        .WithMany()
                        .HasForeignKey("QuizId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuizEntity");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.SubscriptionPaymentHistoryEntity", b =>
                {
                    b.HasOne("CollegeQuizWeb.Entities.UserEntity", "UserEntity")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserEntity");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.UsersQuestionsAnswersEntity", b =>
                {
                    b.HasOne("CollegeQuizWeb.Entities.QuizSessionParticEntity", "QuizSessionParticEntity")
                        .WithMany()
                        .HasForeignKey("ConnectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuizSessionParticEntity");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.QuizEntity", b =>
                {
                    b.Navigation("SharedQuizesEntities");
                });

            modelBuilder.Entity("CollegeQuizWeb.Entities.UserEntity", b =>
                {
                    b.Navigation("SharedQuizesEntities");
                });
#pragma warning restore 612, 618
        }
    }
}

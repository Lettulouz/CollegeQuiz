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

                    b.Property<string>("HostConnId")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("host_conn_id");

                    b.Property<bool>("IsEstabilished")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_estabilished");

                    b.Property<bool>("IsExpired")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_expired");

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

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_active");

                    b.Property<long>("ParticipantId")
                        .HasColumnType("bigint")
                        .HasColumnName("participant_id");

                    b.Property<long>("QuizLobbyId")
                        .HasColumnType("bigint")
                        .HasColumnName("quiz_lobby_id");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.HasIndex("QuizLobbyId");

                    b.ToTable("quiz_session_participants");
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

            modelBuilder.Entity("CollegeQuizWeb.Entities.AnswerEntity", b =>
                {
                    b.HasOne("CollegeQuizWeb.Entities.QuestionEntity", "QuestionEntity")
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuestionEntity");
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
#pragma warning restore 612, 618
        }
    }
}

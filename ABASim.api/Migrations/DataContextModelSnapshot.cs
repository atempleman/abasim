﻿// <auto-generated />
using System;
using ABASim.api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ABASim.api.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1");

            modelBuilder.Entity("ABASim.api.Models.ContactForm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Contact")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ContactForms");
                });

            modelBuilder.Entity("ABASim.api.Models.DepthChart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Depth")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Position")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("DepthCharts");
                });

            modelBuilder.Entity("ABASim.api.Models.DraftPick", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Pick")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Round")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("DraftPicks");
                });

            modelBuilder.Entity("ABASim.api.Models.DraftRanking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rank")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("DraftRankings");
                });

            modelBuilder.Entity("ABASim.api.Models.DraftTracker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DateTimeOfLastPick")
                        .HasColumnType("TEXT");

                    b.Property<int>("Pick")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Round")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("DraftTrackers");
                });

            modelBuilder.Entity("ABASim.api.Models.GameBoxScore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Assists")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BlockedAttempts")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Blocks")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DRebs")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FieldGoalsAttempted")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FieldGoalsMade")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Fouls")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FreeThrowsAttempted")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FreeThrowsMade")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Minutes")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ORebs")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlusMinus")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Points")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rebounds")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Steals")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ThreeFieldGoalsAttempted")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ThreeFieldGoalsMade")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Turnovers")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("GameBoxScores");
                });

            modelBuilder.Entity("ABASim.api.Models.GameResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AwayScore")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HomeScore")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WinningTeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("GameResults");
                });

            modelBuilder.Entity("ABASim.api.Models.InitialDraft", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Pick")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Round")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("InitialDrafts");
                });

            modelBuilder.Entity("ABASim.api.Models.League", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("StateId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Leagues");
                });

            modelBuilder.Entity("ABASim.api.Models.LeagueState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("State")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("LeagueStates");
                });

            modelBuilder.Entity("ABASim.api.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CPosition")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .HasColumnType("TEXT");

                    b.Property<int>("PFPosition")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PGPosition")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SFPosition")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SGPosition")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Surname")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("ABASim.api.Models.PlayerContract", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GuranteedFive")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GuranteedFour")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GuranteedOne")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GuranteedThree")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GuranteedTwo")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerOption")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamOption")
                        .HasColumnType("INTEGER");

                    b.Property<int>("YearFive")
                        .HasColumnType("INTEGER");

                    b.Property<int>("YearFour")
                        .HasColumnType("INTEGER");

                    b.Property<int>("YearOne")
                        .HasColumnType("INTEGER");

                    b.Property<int>("YearThree")
                        .HasColumnType("INTEGER");

                    b.Property<int>("YearTwo")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("PlayerContracts");
                });

            modelBuilder.Entity("ABASim.api.Models.PlayerGrading", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BlockGrade")
                        .HasColumnType("TEXT");

                    b.Property<string>("DRebGrade")
                        .HasColumnType("TEXT");

                    b.Property<string>("FTGrade")
                        .HasColumnType("TEXT");

                    b.Property<string>("HandlingGrade")
                        .HasColumnType("TEXT");

                    b.Property<string>("IntangiblesGrade")
                        .HasColumnType("TEXT");

                    b.Property<string>("ORebGrade")
                        .HasColumnType("TEXT");

                    b.Property<string>("PassingGrade")
                        .HasColumnType("TEXT");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("StaminaGrade")
                        .HasColumnType("TEXT");

                    b.Property<string>("StealGrade")
                        .HasColumnType("TEXT");

                    b.Property<string>("ThreeGrade")
                        .HasColumnType("TEXT");

                    b.Property<string>("TwoGrade")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PlayerGradings");
                });

            modelBuilder.Entity("ABASim.api.Models.PlayerRating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AssitRating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BlockRating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DRPMRating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DRebRating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FTRating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ORPMRating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ORebRating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PassAssistRating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StaminaRating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StealRating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ThreeRating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TwoRating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UsageRating")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("PlayerRatings");
                });

            modelBuilder.Entity("ABASim.api.Models.PlayerTeam", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("PlayerTeams");
                });

            modelBuilder.Entity("ABASim.api.Models.PlayerTendancy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("FouledTendancy")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PassTendancy")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ThreePointTendancy")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TurnoverTendancy")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TwoPointTendancy")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("PlayerTendancies");
                });

            modelBuilder.Entity("ABASim.api.Models.Roster", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Rosters");
                });

            modelBuilder.Entity("ABASim.api.Models.Schedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AwayTeamId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HomeTeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("ABASim.api.Models.Standing", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ConfLosses")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ConfWins")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GamesPlayed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HomeLosses")
                        .HasColumnType("INTEGER");

                    b.Property<int>("HomeWins")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Losses")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoadLosses")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoadWins")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TeamId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Wins")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Standings");
                });

            modelBuilder.Entity("ABASim.api.Models.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Division")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Mascot")
                        .HasColumnType("TEXT");

                    b.Property<string>("ShortCode")
                        .HasColumnType("TEXT");

                    b.Property<string>("Teamname")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("ABASim.api.Models.TeamDraftPick", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentTeam")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OriginalTeam")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Round")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("TeamDraftPicks");
                });

            modelBuilder.Entity("ABASim.api.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<int>("IsAdmin")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("BLOB");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}

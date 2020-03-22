using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ABASim.api.Migrations
{
    public partial class WholeDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactForms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Contact = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactForms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepthCharts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamId = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    Depth = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepthCharts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DraftPicks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Round = table.Column<int>(nullable: false),
                    Pick = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftPicks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameBoxScores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false),
                    Minutes = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    Rebounds = table.Column<int>(nullable: false),
                    Assists = table.Column<int>(nullable: false),
                    Steals = table.Column<int>(nullable: false),
                    Blocks = table.Column<int>(nullable: false),
                    BlockedAttempts = table.Column<int>(nullable: false),
                    FieldGoalsMade = table.Column<int>(nullable: false),
                    FieldGoalsAttempted = table.Column<int>(nullable: false),
                    ThreeFieldGoalsMade = table.Column<int>(nullable: false),
                    ThreeFieldGoalsAttempted = table.Column<int>(nullable: false),
                    FreeThrowsMade = table.Column<int>(nullable: false),
                    FreeThrowsAttempted = table.Column<int>(nullable: false),
                    ORebs = table.Column<int>(nullable: false),
                    DRebs = table.Column<int>(nullable: false),
                    Turnovers = table.Column<int>(nullable: false),
                    Fouls = table.Column<int>(nullable: false),
                    PlusMinus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameBoxScores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(nullable: false),
                    AwayScore = table.Column<int>(nullable: false),
                    HomeScore = table.Column<int>(nullable: false),
                    WinningTeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InitialDrafts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Round = table.Column<int>(nullable: false),
                    Pick = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialDrafts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeagueStates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerContracts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    YearOne = table.Column<int>(nullable: false),
                    GuranteedOne = table.Column<int>(nullable: false),
                    YearTwo = table.Column<int>(nullable: false),
                    GuranteedTwo = table.Column<int>(nullable: false),
                    YearThree = table.Column<int>(nullable: false),
                    GuranteedThree = table.Column<int>(nullable: false),
                    YearFour = table.Column<int>(nullable: false),
                    GuranteedFour = table.Column<int>(nullable: false),
                    YearFive = table.Column<int>(nullable: false),
                    GuranteedFive = table.Column<int>(nullable: false),
                    TeamOption = table.Column<int>(nullable: false),
                    PlayerOption = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerContracts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerGradings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(nullable: false),
                    TwoGrade = table.Column<string>(nullable: true),
                    ThreeGrade = table.Column<string>(nullable: true),
                    FTGrade = table.Column<string>(nullable: true),
                    ORebGrade = table.Column<string>(nullable: true),
                    DRebGrade = table.Column<string>(nullable: true),
                    HandlingGrade = table.Column<string>(nullable: true),
                    StealGrade = table.Column<string>(nullable: true),
                    BlockGrade = table.Column<string>(nullable: true),
                    StaminaGrade = table.Column<string>(nullable: true),
                    PassingGrade = table.Column<string>(nullable: true),
                    IntangiblesGrade = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGradings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerRatings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(nullable: false),
                    TwoRating = table.Column<int>(nullable: false),
                    ThreeRating = table.Column<int>(nullable: false),
                    FTRating = table.Column<int>(nullable: false),
                    ORebRating = table.Column<int>(nullable: false),
                    DRebRating = table.Column<int>(nullable: false),
                    AssitRating = table.Column<int>(nullable: false),
                    PassAssistRating = table.Column<int>(nullable: false),
                    StealRating = table.Column<int>(nullable: false),
                    BlockRating = table.Column<int>(nullable: false),
                    UsageRating = table.Column<int>(nullable: false),
                    StaminaRating = table.Column<int>(nullable: false),
                    ORPMRating = table.Column<int>(nullable: false),
                    DRPMRating = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRatings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    PGPosition = table.Column<int>(nullable: false),
                    SGPosition = table.Column<int>(nullable: false),
                    SFPosition = table.Column<int>(nullable: false),
                    PFPosition = table.Column<int>(nullable: false),
                    CPosition = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTendancies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(nullable: false),
                    TwoPointTendancy = table.Column<int>(nullable: false),
                    ThreePointTendancy = table.Column<int>(nullable: false),
                    PassTendancy = table.Column<int>(nullable: false),
                    FouledTendancy = table.Column<int>(nullable: false),
                    TurnoverTendancy = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTendancies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rosters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamId = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rosters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AwayTeamId = table.Column<int>(nullable: false),
                    HomeTeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Standings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TeamId = table.Column<int>(nullable: false),
                    GamesPlayed = table.Column<int>(nullable: false),
                    Wins = table.Column<int>(nullable: false),
                    Losses = table.Column<int>(nullable: false),
                    HomeWins = table.Column<int>(nullable: false),
                    HomeLosses = table.Column<int>(nullable: false),
                    RoadWins = table.Column<int>(nullable: false),
                    RoadLosses = table.Column<int>(nullable: false),
                    ConfWins = table.Column<int>(nullable: false),
                    ConfLosses = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Standings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamDraftPicks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<int>(nullable: false),
                    Round = table.Column<int>(nullable: false),
                    OriginalTeam = table.Column<int>(nullable: false),
                    CurrentTeam = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamDraftPicks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(nullable: false),
                    Teamname = table.Column<string>(nullable: true),
                    ShortCode = table.Column<string>(nullable: true),
                    Mascot = table.Column<string>(nullable: true),
                    Division = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    IsAdmin = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactForms");

            migrationBuilder.DropTable(
                name: "DepthCharts");

            migrationBuilder.DropTable(
                name: "DraftPicks");

            migrationBuilder.DropTable(
                name: "GameBoxScores");

            migrationBuilder.DropTable(
                name: "GameResults");

            migrationBuilder.DropTable(
                name: "InitialDrafts");

            migrationBuilder.DropTable(
                name: "Leagues");

            migrationBuilder.DropTable(
                name: "LeagueStates");

            migrationBuilder.DropTable(
                name: "PlayerContracts");

            migrationBuilder.DropTable(
                name: "PlayerGradings");

            migrationBuilder.DropTable(
                name: "PlayerRatings");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "PlayerTendancies");

            migrationBuilder.DropTable(
                name: "Rosters");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Standings");

            migrationBuilder.DropTable(
                name: "TeamDraftPicks");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

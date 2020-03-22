using Microsoft.EntityFrameworkCore.Migrations;

namespace ABASim.api.Migrations
{
    public partial class DraftTracker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DraftTrackers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Round = table.Column<int>(nullable: false),
                    Pick = table.Column<int>(nullable: false),
                    DateTimeOfLastPick = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftTrackers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DraftTrackers");
        }
    }
}

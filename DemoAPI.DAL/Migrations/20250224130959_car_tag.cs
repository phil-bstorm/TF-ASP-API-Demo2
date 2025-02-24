using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoAPI.DAL.Migrations
{
    /// <inheritdoc />
    public partial class car_tag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarTag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarCarTag",
                columns: table => new
                {
                    CarsId = table.Column<int>(type: "int", nullable: false),
                    TagsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarCarTag", x => new { x.CarsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_CarCarTag_CarTag_TagsId",
                        column: x => x.TagsId,
                        principalTable: "CarTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarCarTag_Cars_CarsId",
                        column: x => x.CarsId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarCarTag_TagsId",
                table: "CarCarTag",
                column: "TagsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarCarTag");

            migrationBuilder.DropTable(
                name: "CarTag");
        }
    }
}

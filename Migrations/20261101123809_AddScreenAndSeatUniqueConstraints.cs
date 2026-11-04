using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTicketBooking.Migrations
{
    /// <inheritdoc />
    public partial class AddScreenAndSeatUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seats_ScreenId",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Screens_TheatreId",
                table: "Screens");

            migrationBuilder.AlterColumn<string>(
                name: "RowName",
                table: "Seats",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ScreenName",
                table: "Screens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_ScreenId_RowName_SeatNumber",
                table: "Seats",
                columns: new[] { "ScreenId", "RowName", "SeatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Screens_TheatreId_ScreenName",
                table: "Screens",
                columns: new[] { "TheatreId", "ScreenName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seats_ScreenId_RowName_SeatNumber",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Screens_TheatreId_ScreenName",
                table: "Screens");

            migrationBuilder.AlterColumn<string>(
                name: "RowName",
                table: "Seats",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ScreenName",
                table: "Screens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_ScreenId",
                table: "Seats",
                column: "ScreenId");

            migrationBuilder.CreateIndex(
                name: "IX_Screens_TheatreId",
                table: "Screens",
                column: "TheatreId");
        }
    }
}

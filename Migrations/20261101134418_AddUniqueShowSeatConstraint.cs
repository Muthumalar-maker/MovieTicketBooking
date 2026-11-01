using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTicketBooking.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueShowSeatConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShowSeats_ShowId",
                table: "ShowSeats");

            migrationBuilder.CreateIndex(
                name: "IX_ShowSeats_ShowId_SeatId",
                table: "ShowSeats",
                columns: new[] { "ShowId", "SeatId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShowSeats_ShowId_SeatId",
                table: "ShowSeats");

            migrationBuilder.CreateIndex(
                name: "IX_ShowSeats_ShowId",
                table: "ShowSeats",
                column: "ShowId");
        }
    }
}

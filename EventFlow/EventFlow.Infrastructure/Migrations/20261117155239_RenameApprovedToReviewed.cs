using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameApprovedToReviewed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizerRequests_AspNetUsers_ApprovedById",
                table: "OrganizerRequests");

            migrationBuilder.RenameColumn(
                name: "ApprovedById",
                table: "OrganizerRequests",
                newName: "ReviewedById");

            migrationBuilder.RenameColumn(
                name: "ApprovedAt",
                table: "OrganizerRequests",
                newName: "ReviewedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizerRequests_ApprovedById",
                table: "OrganizerRequests",
                newName: "IX_OrganizerRequests_ReviewedById");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizerRequests_AspNetUsers_ReviewedById",
                table: "OrganizerRequests",
                column: "ReviewedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizerRequests_AspNetUsers_ReviewedById",
                table: "OrganizerRequests");

            migrationBuilder.RenameColumn(
                name: "ReviewedById",
                table: "OrganizerRequests",
                newName: "ApprovedById");

            migrationBuilder.RenameColumn(
                name: "ReviewedAt",
                table: "OrganizerRequests",
                newName: "ApprovedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizerRequests_ReviewedById",
                table: "OrganizerRequests",
                newName: "IX_OrganizerRequests_ApprovedById");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizerRequests_AspNetUsers_ApprovedById",
                table: "OrganizerRequests",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCryptoPortfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Fee",
                table: "Transactions",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fee",
                table: "Transactions");
        }
    }
}

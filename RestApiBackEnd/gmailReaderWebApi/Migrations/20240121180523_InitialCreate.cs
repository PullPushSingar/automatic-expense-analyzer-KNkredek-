using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gmailReaderWebApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    OperationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperationTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountAmountAfterOperation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.OperationId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Operations");
        }
    }
}

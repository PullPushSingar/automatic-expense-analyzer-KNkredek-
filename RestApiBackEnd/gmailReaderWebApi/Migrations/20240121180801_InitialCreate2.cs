using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gmailReaderWebApi.Migrations
{
    public partial class InitialCreate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Operations");
        }

     
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bestella.Migrations;

public partial class AddParsedDate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateOnly>(
            name: "ParsedDate",
            table: "Orders",
            type: "date",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ParsedDate",
            table: "Orders");
    }
}
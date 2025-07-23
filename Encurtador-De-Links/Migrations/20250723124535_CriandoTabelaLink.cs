using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Encurtador_De_Links.Migrations;

public partial class CriandoTabelaLink : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Links",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                DataHoraCriacao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                Original = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Encurtado = table.Column<string>(type: "longtext", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Inativo = table.Column<bool>(type: "tinyint(1)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Links", x => x.Id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Links");
    }
}

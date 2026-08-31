using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioApi.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionadoAsignacionComunal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "AsignacionesComunales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AsignacionComunalVersiones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AsignacionComunalId = table.Column<int>(type: "int", nullable: false),
                    NumeroVersion = table.Column<int>(type: "int", nullable: false),
                    FechaGuardado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatosJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionComunalVersiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsignacionComunalVersiones_AsignacionesComunales_AsignacionComunalId",
                        column: x => x.AsignacionComunalId,
                        principalTable: "AsignacionesComunales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionComunalVersiones_AsignacionComunalId",
                table: "AsignacionComunalVersiones",
                column: "AsignacionComunalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignacionComunalVersiones");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "AsignacionesComunales");
        }
    }
}

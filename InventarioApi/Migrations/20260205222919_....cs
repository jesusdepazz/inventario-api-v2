using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioApi.Migrations
{
    /// <inheritdoc />
    public partial class _ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        { 
            migrationBuilder.CreateTable(
                name: "Traslados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    No = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UbicacionDesde = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UbicacionHasta = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traslados", x => x.Id);
                });


            migrationBuilder.CreateTable(
                name: "TrasladoEmpleadoEntregas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrasladoId = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Puesto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrasladoEmpleadoEntregas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrasladoEmpleadoEntregas_Traslados_TrasladoId",
                        column: x => x.TrasladoId,
                        principalTable: "Traslados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrasladoEmpleadoRecibes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrasladoId = table.Column<int>(type: "int", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Puesto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrasladoEmpleadoRecibes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrasladoEmpleadoRecibes_Traslados_TrasladoId",
                        column: x => x.TrasladoId,
                        principalTable: "Traslados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrasladoEquipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrasladoId = table.Column<int>(type: "int", nullable: false),
                    Equipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescripcionEquipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Serie = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrasladoEquipos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrasladoEquipos_Traslados_TrasladoId",
                        column: x => x.TrasladoId,
                        principalTable: "Traslados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrasladoEmpleadoEntregas_TrasladoId",
                table: "TrasladoEmpleadoEntregas",
                column: "TrasladoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrasladoEmpleadoRecibes_TrasladoId",
                table: "TrasladoEmpleadoRecibes",
                column: "TrasladoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrasladoEquipos_TrasladoId",
                table: "TrasladoEquipos",
                column: "TrasladoId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrasladoEmpleadoEntregas");

            migrationBuilder.DropTable(
                name: "TrasladoEmpleadoRecibes");

            migrationBuilder.DropTable(
                name: "TrasladoEquipos");

            migrationBuilder.DropTable(
                name: "Traslados");
        }
    }
}

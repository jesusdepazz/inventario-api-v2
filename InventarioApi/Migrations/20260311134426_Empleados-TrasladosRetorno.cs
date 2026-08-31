using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioApi.Migrations
{
    /// <inheritdoc />
    public partial class EmpleadosTrasladosRetorno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrasladoRetornoEmpleados_TrasladoRetornoId",
                table: "TrasladoRetornoEmpleados");

            migrationBuilder.AlterColumn<string>(
                name: "TipoHoja",
                table: "HojasResponsabilidad",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrasladoRetornoEmpleados_TrasladoRetornoId",
                table: "TrasladoRetornoEmpleados",
                column: "TrasladoRetornoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrasladoRetornoEmpleados_TrasladoRetornoId",
                table: "TrasladoRetornoEmpleados");

            migrationBuilder.AlterColumn<string>(
                name: "TipoHoja",
                table: "HojasResponsabilidad",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TrasladoRetornoEmpleados_TrasladoRetornoId",
                table: "TrasladoRetornoEmpleados",
                column: "TrasladoRetornoId",
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioApi.Migrations
{
    /// <inheritdoc />
    public partial class HojaResponsabilidadMovil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Accesorios",
                table: "HojasResponsabilidad",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "TipoHoja",
                table: "HojasResponsabilidad",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "HojaEquipos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Imei",
                table: "HojaEquipos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroAsignado",
                table: "HojaEquipos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EquipoTipo",
                table: "Equipos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Imei",
                table: "Equipos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroAsignado",
                table: "Equipos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoHoja",
                table: "HojasResponsabilidad");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "HojaEquipos");

            migrationBuilder.DropColumn(
                name: "Imei",
                table: "HojaEquipos");

            migrationBuilder.DropColumn(
                name: "NumeroAsignado",
                table: "HojaEquipos");

            migrationBuilder.DropColumn(
                name: "EquipoTipo",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "Imei",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "NumeroAsignado",
                table: "Equipos");

            migrationBuilder.AlterColumn<string>(
                name: "Accesorios",
                table: "HojasResponsabilidad",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}

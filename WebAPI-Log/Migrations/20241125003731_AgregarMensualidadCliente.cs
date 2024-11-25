using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI_Log.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMensualidadCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstaPagada",
                table: "Cliente",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaPago",
                table: "Cliente",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVencimiento",
                table: "Cliente",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstaPagada",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "FechaPago",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "FechaVencimiento",
                table: "Cliente");
        }
    }
}

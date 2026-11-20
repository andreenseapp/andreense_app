using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModuloMonolitico.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoImoveisFavoritos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "imoveis_favoritos",
                columns: table => new
                {
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    imovel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imoveis_favoritos", x => new { x.usuario_id, x.imovel_id });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "imoveis_favoritos");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace proyectonew.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aerolineas",
                columns: table => new
                {
                    AerolineaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aerolineas", x => x.AerolineaId);
                });

            migrationBuilder.CreateTable(
                name: "Aeropuertos",
                columns: table => new
                {
                    AeropuertoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aeropuertos", x => x.AeropuertoId);
                });

            migrationBuilder.CreateTable(
                name: "EstadosVuelo",
                columns: table => new
                {
                    EstadoVueloId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosVuelo", x => x.EstadoVueloId);
                });

            migrationBuilder.CreateTable(
                name: "Vuelos",
                columns: table => new
                {
                    VueloId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroVuelo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AerolineaId = table.Column<int>(type: "int", nullable: false),
                    AeropuertoOrigenId = table.Column<int>(type: "int", nullable: false),
                    AeropuertoDestinoId = table.Column<int>(type: "int", nullable: false),
                    HorarioProgramado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoVueloId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vuelos", x => x.VueloId);
                    table.ForeignKey(
                        name: "FK_Vuelos_Aerolineas_AerolineaId",
                        column: x => x.AerolineaId,
                        principalTable: "Aerolineas",
                        principalColumn: "AerolineaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vuelos_Aeropuertos_AeropuertoDestinoId",
                        column: x => x.AeropuertoDestinoId,
                        principalTable: "Aeropuertos",
                        principalColumn: "AeropuertoId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Vuelos_Aeropuertos_AeropuertoOrigenId",
                        column: x => x.AeropuertoOrigenId,
                        principalTable: "Aeropuertos",
                        principalColumn: "AeropuertoId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Vuelos_EstadosVuelo_EstadoVueloId",
                        column: x => x.EstadoVueloId,
                        principalTable: "EstadosVuelo",
                        principalColumn: "EstadoVueloId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vuelos_AerolineaId",
                table: "Vuelos",
                column: "AerolineaId");

            migrationBuilder.CreateIndex(
                name: "IX_Vuelos_AeropuertoDestinoId",
                table: "Vuelos",
                column: "AeropuertoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Vuelos_AeropuertoOrigenId",
                table: "Vuelos",
                column: "AeropuertoOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_Vuelos_EstadoVueloId",
                table: "Vuelos",
                column: "EstadoVueloId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vuelos");

            migrationBuilder.DropTable(
                name: "Aerolineas");

            migrationBuilder.DropTable(
                name: "Aeropuertos");

            migrationBuilder.DropTable(
                name: "EstadosVuelo");
        }
    }
}
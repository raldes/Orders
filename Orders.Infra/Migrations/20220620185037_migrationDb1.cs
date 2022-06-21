using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orders.Infra.Migrations
{
    public partial class migrationDb1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "IntegrationEventLog",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTypeName = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    TimesSent = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    TransactionId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEventLog", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Items = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ruid = table.Column<Guid>(type: "uuid", nullable: false),
                    created_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    modified_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationEventLog");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "public");
        }
    }
}

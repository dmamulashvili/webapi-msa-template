using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSA.Template.Infrastructure.Migrations
{
    public partial class ChangeDefaultSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessage_SequenceNumber_OutboxId",
                table: "OutboxMessage");

            migrationBuilder.EnsureSchema(
                name: "api");

            migrationBuilder.RenameTable(
                name: "OrderLine",
                schema: "Application",
                newName: "OrderLine",
                newSchema: "api");

            migrationBuilder.RenameTable(
                name: "Order",
                schema: "Application",
                newName: "Order",
                newSchema: "api");

            migrationBuilder.RenameTable(
                name: "ClientRequest",
                schema: "Application",
                newName: "ClientRequest",
                newSchema: "api");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                table: "OutboxMessage");

            migrationBuilder.EnsureSchema(
                name: "Application");

            migrationBuilder.RenameTable(
                name: "OrderLine",
                schema: "api",
                newName: "OrderLine",
                newSchema: "Application");

            migrationBuilder.RenameTable(
                name: "Order",
                schema: "api",
                newName: "Order",
                newSchema: "Application");

            migrationBuilder.RenameTable(
                name: "ClientRequest",
                schema: "api",
                newName: "ClientRequest",
                newSchema: "Application");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_SequenceNumber_OutboxId",
                table: "OutboxMessage",
                columns: new[] { "SequenceNumber", "OutboxId" },
                unique: true);
        }
    }
}

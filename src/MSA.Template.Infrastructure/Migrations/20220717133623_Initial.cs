using System;
using MSA.Template.Core.OrderAggregate;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MSA.Template.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "Application");

            migrationBuilder.CreateTable(
                name: "ClientRequest",
                schema: "Application",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        Name = table.Column<string>(type: "text", nullable: false),
                        Time = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientRequest", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Order",
                schema: "Application",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        OrderStatus = table.Column<int>(type: "integer", nullable: false),
                        OrderDate = table.Column<DateTimeOffset>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        Address = table.Column<Address>(type: "jsonb", nullable: false),
                        CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                        ModifiedBy = table.Column<string>(type: "text", nullable: false),
                        DateModified = table.Column<DateTimeOffset>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        CreatedBy = table.Column<string>(type: "text", nullable: false),
                        DateCreated = table.Column<DateTimeOffset>(
                            type: "timestamp with time zone",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                columns: table =>
                    new
                    {
                        SequenceNumber = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        EnqueueTime = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        SentTime = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        Headers = table.Column<string>(type: "text", nullable: true),
                        Properties = table.Column<string>(type: "text", nullable: true),
                        InboxMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                        InboxConsumerId = table.Column<Guid>(type: "uuid", nullable: true),
                        OutboxId = table.Column<Guid>(type: "uuid", nullable: true),
                        MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                        ContentType = table.Column<string>(
                            type: "character varying(256)",
                            maxLength: 256,
                            nullable: false
                        ),
                        Body = table.Column<string>(type: "text", nullable: false),
                        ConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                        CorrelationId = table.Column<Guid>(type: "uuid", nullable: true),
                        InitiatorId = table.Column<Guid>(type: "uuid", nullable: true),
                        RequestId = table.Column<Guid>(type: "uuid", nullable: true),
                        SourceAddress = table.Column<string>(
                            type: "character varying(256)",
                            maxLength: 256,
                            nullable: true
                        ),
                        DestinationAddress = table.Column<string>(
                            type: "character varying(256)",
                            maxLength: 256,
                            nullable: true
                        ),
                        ResponseAddress = table.Column<string>(
                            type: "character varying(256)",
                            maxLength: 256,
                            nullable: true
                        ),
                        FaultAddress = table.Column<string>(
                            type: "character varying(256)",
                            maxLength: 256,
                            nullable: true
                        ),
                        ExpirationTime = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.SequenceNumber);
                }
            );

            migrationBuilder.CreateTable(
                name: "OutboxState",
                columns: table =>
                    new
                    {
                        OutboxId = table.Column<Guid>(type: "uuid", nullable: false),
                        Created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        Delivered = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxState", x => x.OutboxId);
                }
            );

            migrationBuilder.CreateTable(
                name: "OrderLine",
                schema: "Application",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(type: "integer", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        ItemId = table.Column<int>(type: "integer", nullable: false),
                        ItemPrice = table.Column<decimal>(type: "numeric", nullable: false),
                        Quantity = table.Column<int>(type: "integer", nullable: false),
                        OrderId = table.Column<Guid>(type: "uuid", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderLine_Order_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "Application",
                        principalTable: "Order",
                        principalColumn: "Id"
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_OrderLine_ItemId",
                schema: "Application",
                table: "OrderLine",
                column: "ItemId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OrderLine_OrderId",
                schema: "Application",
                table: "OrderLine",
                column: "OrderId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_EnqueueTime",
                table: "OutboxMessage",
                column: "EnqueueTime"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_ExpirationTime",
                table: "OutboxMessage",
                column: "ExpirationTime"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_SequenceNumber_OutboxId",
                table: "OutboxMessage",
                columns: new[] { "SequenceNumber", "OutboxId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_Created",
                table: "OutboxState",
                column: "Created"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ClientRequest", schema: "Application");

            migrationBuilder.DropTable(name: "OrderLine", schema: "Application");

            migrationBuilder.DropTable(name: "OutboxMessage");

            migrationBuilder.DropTable(name: "OutboxState");

            migrationBuilder.DropTable(name: "Order", schema: "Application");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Khidmah_Inventory.Infrastructure.Migrations
{
    public partial class AddAutomationRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutomationRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Trigger = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConditionJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table => table.PrimaryKey("PK_AutomationRules", x => x.Id));

            migrationBuilder.CreateTable(
                name: "AutomationRuleHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AutomationRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Trigger = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TriggerContextJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionExecuted = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutomationRuleHistories", x => x.Id);
                    table.ForeignKey(name: "FK_AutomationRuleHistories_AutomationRules_AutomationRuleId", column: x => x.AutomationRuleId, principalTable: "AutomationRules", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(name: "IX_AutomationRules_CompanyId_Trigger_IsActive", table: "AutomationRules", columns: new[] { "CompanyId", "Trigger", "IsActive" });
            migrationBuilder.CreateIndex(name: "IX_AutomationRuleHistories_AutomationRuleId", table: "AutomationRuleHistories", column: "AutomationRuleId");
            migrationBuilder.CreateIndex(name: "IX_AutomationRuleHistories_CompanyId_AutomationRuleId_CreatedAt", table: "AutomationRuleHistories", columns: new[] { "CompanyId", "AutomationRuleId", "CreatedAt" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AutomationRuleHistories");
            migrationBuilder.DropTable(name: "AutomationRules");
        }
    }
}

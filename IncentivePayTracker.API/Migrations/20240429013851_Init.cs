using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IncentivePayTracker.API.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsExempted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Infractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infractions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTimeIns",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    TimeIn = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTimeIns", x => new { x.EmployeeId, x.Month, x.Year });
                    table.ForeignKey(
                        name: "FK_EmployeeTimeIns_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    YearHired = table.Column<int>(type: "int", nullable: false),
                    MonthHired = table.Column<int>(type: "int", nullable: false),
                    YearTerminated = table.Column<int>(type: "int", nullable: true),
                    MonthTerminated = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmploymentDates_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeInfractions",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    InfractionId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeInfractions", x => new { x.EmployeeId, x.InfractionId, x.Month, x.Year });
                    table.ForeignKey(
                        name: "FK_EmployeeInfractions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeInfractions_Infractions_InfractionId",
                        column: x => x.InfractionId,
                        principalTable: "Infractions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Infractions",
                columns: new[] { "Id", "Amount", "Description" },
                values: new object[,]
                {
                    { 1, 200.0, "If you are missing a login and/or logout in Sprout during a work day." },
                    { 2, 200.0, "If you forget to log your hours in JIRA (or equivalent methods in your project) and send the report by 4PM the following work day.\r\n\r\nIf you forget to log your hours before you go on a planned leave." },
                    { 3, 200.0, "If your login time in Sprout is past your latest allowed login time." },
                    { 4, 500.0, "If you take a leave of absence (vacation leave) without respecting the proper advance notice period. \r\n\r\nIf you take a leave of absence with no notification at all (AWOL)" },
                    { 5, 300.0, "If you take an undertime, SL, or Compensation, but did not file it or clearly declare whether it's for deduction or compensation, that Marifil has to chase you for it.\r\n\r\n(Check https://tinyurl.com/p862fcx3 for proper filing of these absences)." },
                    { 6, 500.0, "If you were sick for 2 days without a medical certificate, or submit it late that Jobi has to chase you for it. \r\n\r\nIf you file your SL late (over 1 working day after returning from SL)." },
                    { 7, 1000.0, "If you are unavailable during project activities without giving timely notification." },
                    { 8, 200.0, "If you miss submitting an evaluation feedback (including self-evaluation) before a deadline." },
                    { 9, 500.0, "If you miss submitting an evaluation feedback (including self-evaluation) before a second or an extended deadline." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInfractions_InfractionId",
                table: "EmployeeInfractions",
                column: "InfractionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentDates_EmployeeId",
                table: "EmploymentDates",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeInfractions");

            migrationBuilder.DropTable(
                name: "EmployeeTimeIns");

            migrationBuilder.DropTable(
                name: "EmploymentDates");

            migrationBuilder.DropTable(
                name: "Infractions");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}

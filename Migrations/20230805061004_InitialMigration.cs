using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RCCProgressAccountingOfEquipmentOfSpacecraft.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AccessTypes",
                columns: table => new
                {
                    AccessType_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccessTypeName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessTypes", x => x.AccessType_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "EquipmentTypes",
                columns: table => new
                {
                    EquipmentType_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EquipmentTypeName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentTypes", x => x.EquipmentType_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Manufacturer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ManufacturerShortName = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ManufacturerFullName = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Manufacturer_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Post_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PostName = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Post_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "UnitInfo",
                columns: table => new
                {
                    UnitInfo_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Photo = table.Column<byte[]>(type: "longblob", nullable: true),
                    EquipmentName = table.Column<string>(type: "varchar(175)", maxLength: 175, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EquipmentCodename = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EquipmentPassport = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EquipmentType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitInfo", x => x.UnitInfo_id);
                    table.ForeignKey(
                        name: "fk_UnitInfo_EquipmentTypes1",
                        column: x => x.EquipmentType,
                        principalTable: "EquipmentTypes",
                        principalColumn: "EquipmentType_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "SupplyContracts",
                columns: table => new
                {
                    SupplyContract_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SupplyContractCodename = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ManufacturerShortName = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyContracts", x => x.SupplyContract_id);
                    table.ForeignKey(
                        name: "fk_SupplyContracts_Manufacturers1",
                        column: x => x.ManufacturerShortName,
                        principalTable: "Manufacturers",
                        principalColumn: "Manufacturer_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "ResponsibleEmployees",
                columns: table => new
                {
                    Employee_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmployeeLogin = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeePassword = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeFirstName = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeSecondName = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeLastName = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeInitials = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccessType = table.Column<int>(type: "int", nullable: false),
                    Post = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Employee_id);
                    table.ForeignKey(
                        name: "fk_ResponsibleEmployees_AccessTypes1",
                        column: x => x.AccessType,
                        principalTable: "AccessTypes",
                        principalColumn: "AccessType_id");
                    table.ForeignKey(
                        name: "fk_ResponsibleEmployees_Posts1",
                        column: x => x.Post,
                        principalTable: "Posts",
                        principalColumn: "Post_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "StoredUnits",
                columns: table => new
                {
                    SUnit_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Purpose = table.Column<string>(type: "varchar(325)", maxLength: 325, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateOfWriteOff = table.Column<DateTime>(type: "datetime", nullable: true),
                    Note = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WhoAdded = table.Column<int>(type: "int", nullable: false),
                    SupplyContract = table.Column<int>(type: "int", nullable: false),
                    UnitInfo = table.Column<int>(type: "int", nullable: false),
                    SignOfDeleting = table.Column<string>(type: "char(1)", fixedLength: true, maxLength: 1, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SUnit_id);
                    table.ForeignKey(
                        name: "fk_StoredUnits_ResponsibleEmployees1",
                        column: x => x.WhoAdded,
                        principalTable: "ResponsibleEmployees",
                        principalColumn: "Employee_id");
                    table.ForeignKey(
                        name: "fk_StoredUnits_SupplyContracts1",
                        column: x => x.SupplyContract,
                        principalTable: "SupplyContracts",
                        principalColumn: "SupplyContract_id");
                    table.ForeignKey(
                        name: "fk_StoredUnits_UnitInfo1",
                        column: x => x.UnitInfo,
                        principalTable: "UnitInfo",
                        principalColumn: "UnitInfo_id");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "fk_ResponsibleEmployees_AccessTypes1_idx",
                table: "ResponsibleEmployees",
                column: "AccessType");

            migrationBuilder.CreateIndex(
                name: "fk_ResponsibleEmployees_Posts1_idx",
                table: "ResponsibleEmployees",
                column: "Post");

            migrationBuilder.CreateIndex(
                name: "fk_StoredUnits_ResponsibleEmployees1_idx",
                table: "StoredUnits",
                column: "WhoAdded");

            migrationBuilder.CreateIndex(
                name: "fk_StoredUnits_SupplyContracts1_idx1",
                table: "StoredUnits",
                column: "SupplyContract");

            migrationBuilder.CreateIndex(
                name: "fk_StoredUnits_UnitInfo1_idx1",
                table: "StoredUnits",
                column: "UnitInfo");

            migrationBuilder.CreateIndex(
                name: "fk_SupplyContracts_Manufacturers1_idx",
                table: "SupplyContracts",
                column: "ManufacturerShortName");

            migrationBuilder.CreateIndex(
                name: "fk_UnitInfo_EquipmentTypes1_idx",
                table: "UnitInfo",
                column: "EquipmentType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoredUnits");

            migrationBuilder.DropTable(
                name: "ResponsibleEmployees");

            migrationBuilder.DropTable(
                name: "SupplyContracts");

            migrationBuilder.DropTable(
                name: "UnitInfo");

            migrationBuilder.DropTable(
                name: "AccessTypes");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "EquipmentTypes");
        }
    }
}

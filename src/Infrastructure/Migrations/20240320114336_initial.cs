using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Installations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstallationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Terms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TocCapacity = table.Column<double>(type: "float", nullable: false),
                    WaterCapacity = table.Column<double>(type: "float", nullable: false),
                    NitrogenCapacity = table.Column<double>(type: "float", nullable: false),
                    Toc = table.Column<double>(type: "float", nullable: false),
                    Water = table.Column<double>(type: "float", nullable: false),
                    Nitrogen = table.Column<double>(type: "float", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipsToId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Installations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Installations_Installations_ShipsToId",
                        column: x => x.ShipsToId,
                        principalTable: "Installations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Chemicals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TocWeight = table.Column<double>(type: "float", nullable: false),
                    NitrogenWeight = table.Column<double>(type: "float", nullable: false),
                    BiocideWeight = table.Column<double>(type: "float", nullable: false),
                    Density = table.Column<double>(type: "float", nullable: false),
                    HazardClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeasureUnitDefault = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FollowOilPhaseDefault = table.Column<bool>(type: "bit", nullable: false),
                    FollowWaterPhaseDefault = table.Column<bool>(type: "bit", nullable: false),
                    Tentative = table.Column<bool>(type: "bit", nullable: false),
                    Proposed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProposedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProposedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProposedByEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disabled = table.Column<bool>(type: "bit", nullable: false),
                    ProposedByInstallationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chemicals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chemicals_Installations_ProposedByInstallationId",
                        column: x => x.ProposedByInstallationId,
                        principalTable: "Installations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InstallationPlants",
                columns: table => new
                {
                    InstallationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallationPlants", x => new { x.PlantId, x.InstallationId });
                    table.ForeignKey(
                        name: "FK_InstallationPlants_Installations_InstallationId",
                        column: x => x.InstallationId,
                        principalTable: "Installations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstallationPlants_Installations_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Installations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RinsingOffshorePercent = table.Column<double>(type: "float", nullable: false),
                    PlannedExecutionFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedExecutionTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WaterAmount = table.Column<double>(type: "float", nullable: false),
                    WaterAmountPerHour = table.Column<double>(type: "float", nullable: false),
                    VolumePersentageOffspec = table.Column<bool>(type: "bit", nullable: false),
                    Well = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContainsChemicals = table.Column<bool>(type: "bit", nullable: false),
                    ContainsStableOilEmulsion = table.Column<bool>(type: "bit", nullable: false),
                    ContainsHighParticleAmount = table.Column<bool>(type: "bit", nullable: false),
                    ContainsBiocides = table.Column<bool>(type: "bit", nullable: false),
                    VolumeHasBeenMinimized = table.Column<bool>(type: "bit", nullable: false),
                    VolumeHasBeenMinimizedComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalProcedure = table.Column<bool>(type: "bit", nullable: true),
                    OnlyWayToGetRidOf = table.Column<bool>(type: "bit", nullable: true),
                    OnlyWayToGetRidOfComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvailableForDailyContact = table.Column<bool>(type: "bit", nullable: true),
                    HeightenedLra = table.Column<bool>(type: "bit", nullable: false),
                    Pb210 = table.Column<double>(type: "float", nullable: true),
                    Ra226 = table.Column<double>(type: "float", nullable: true),
                    Ra228 = table.Column<double>(type: "float", nullable: true),
                    TakePrecaution = table.Column<bool>(type: "bit", nullable: false),
                    Precautions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WaterHasBeenAnalyzed = table.Column<bool>(type: "bit", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasBeenOpened = table.Column<bool>(type: "bit", nullable: false),
                    EvalCapacityOk = table.Column<bool>(type: "bit", nullable: true),
                    EvalCapacityOkUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvalContaminationRisk = table.Column<bool>(type: "bit", nullable: true),
                    EvalContaminationRiskUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvalAmountOk = table.Column<bool>(type: "bit", nullable: true),
                    EvalAmountOkUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvalBiocidesOk = table.Column<bool>(type: "bit", nullable: true),
                    EvalBiocidesOkUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvalEnvImpact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvalComments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_Installations_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Installations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Installations_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Installations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MimeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LogEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstallationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogEntry_Installations_InstallationId",
                        column: x => x.InstallationId,
                        principalTable: "Installations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LogEntry_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentChemicals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChemicalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeasureUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    CalculatedWeight = table.Column<double>(type: "float", nullable: false),
                    CalculatedToc = table.Column<double>(type: "float", nullable: false),
                    CalculatedNitrogen = table.Column<double>(type: "float", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculatedBiocides = table.Column<double>(type: "float", nullable: false),
                    CalculatedBiocidesUnrinsed = table.Column<double>(type: "float", nullable: false),
                    CalculatedNitrogenUnrinsed = table.Column<double>(type: "float", nullable: false),
                    CalculatedTocUnrinsed = table.Column<double>(type: "float", nullable: false),
                    CalculatedWeightUnrinsed = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentChemicals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentChemicals_Chemicals_ChemicalId",
                        column: x => x.ChemicalId,
                        principalTable: "Chemicals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipmentChemicals_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Shipped = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Water = table.Column<double>(type: "float", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentParts_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldChange",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromField = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToField = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldChange_LogEntry_LogId",
                        column: x => x.LogId,
                        principalTable: "LogEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ShipmentId",
                table: "Attachments",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Chemicals_ProposedByInstallationId",
                table: "Chemicals",
                column: "ProposedByInstallationId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ShipmentId",
                table: "Comments",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldChange_LogId",
                table: "FieldChange",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_InstallationPlants_InstallationId",
                table: "InstallationPlants",
                column: "InstallationId");

            migrationBuilder.CreateIndex(
                name: "IX_Installations_ShipsToId",
                table: "Installations",
                column: "ShipsToId");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntry_InstallationId",
                table: "LogEntry",
                column: "InstallationId");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntry_ShipmentId",
                table: "LogEntry",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentChemicals_ChemicalId",
                table: "ShipmentChemicals",
                column: "ChemicalId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentChemicals_ShipmentId",
                table: "ShipmentChemicals",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentParts_ShipmentId",
                table: "ShipmentParts",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ReceiverId",
                table: "Shipments",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_SenderId",
                table: "Shipments",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "FieldChange");

            migrationBuilder.DropTable(
                name: "InstallationPlants");

            migrationBuilder.DropTable(
                name: "ShipmentChemicals");

            migrationBuilder.DropTable(
                name: "ShipmentParts");

            migrationBuilder.DropTable(
                name: "LogEntry");

            migrationBuilder.DropTable(
                name: "Chemicals");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "Installations");
        }
    }
}

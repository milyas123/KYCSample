using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KYC_AzaKaw_Core.Migrations
{
    public partial class FirstTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "MrzInfos",
                columns: table => new
                {
                    MrzId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerId = table.Column<int>(nullable: false),
                    MrzType = table.Column<string>(nullable: true),
                    Line1 = table.Column<string>(nullable: true),
                    Line2 = table.Column<string>(nullable: true),
                    Checksum = table.Column<string>(nullable: true),
                    ChecksumVerified = table.Column<string>(nullable: true),
                    DocumentType = table.Column<string>(nullable: true),
                    DocumentSubtype = table.Column<string>(nullable: true),
                    IssuingCountry = table.Column<string>(nullable: true),
                    DocumentNumber = table.Column<string>(nullable: true),
                    DocumentNumberVerified = table.Column<string>(nullable: true),
                    DocumentNumberCheck = table.Column<string>(nullable: true),
                    Nationality = table.Column<string>(nullable: true),
                    BirthDate = table.Column<string>(nullable: true),
                    BirthDateVerified = table.Column<string>(nullable: true),
                    BirthDateCheck = table.Column<string>(nullable: true),
                    Sex = table.Column<string>(nullable: true),
                    ExpiryDate = table.Column<string>(nullable: true),
                    ExpiryDateVerified = table.Column<string>(nullable: true),
                    ExpiryDateCheck = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    FileNameUnique = table.Column<string>(nullable: true),
                    isKYCVerified = table.Column<bool>(nullable: false),
                    AdditionalInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MrzInfos", x => x.MrzId);
                    table.ForeignKey(
                        name: "FK_MrzInfos_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MrzInfos_CustomerId",
                table: "MrzInfos",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MrzInfos");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}

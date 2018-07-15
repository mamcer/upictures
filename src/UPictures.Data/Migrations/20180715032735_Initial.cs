using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UPictures.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Picture",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Extension = table.Column<string>(maxLength: 50, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Path = table.Column<string>(nullable: true),
                    DateTaken = table.Column<DateTime>(nullable: false),
                    CameraMaker = table.Column<string>(maxLength: 255, nullable: true),
                    CameraModel = table.Column<string>(maxLength: 255, nullable: true),
                    FileSize = table.Column<double>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Hash = table.Column<string>(maxLength: 32, nullable: true),
                    DirectoryName = table.Column<string>(maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Picture", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Picture");
        }
    }
}

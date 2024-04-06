using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecordStore.Api.Entities.Migrations
{
    /// <inheritdoc />
    public partial class RemoveImageUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_url",
                table: "record");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "artist");

            migrationBuilder.AlterColumn<string>(
                name: "track_order",
                table: "track_product",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "track_order",
                table: "track_product",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "record",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "artist",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);
        }
    }
}

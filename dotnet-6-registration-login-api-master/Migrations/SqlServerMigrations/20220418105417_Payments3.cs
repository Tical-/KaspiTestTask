using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations.SqlServerMigrations
{
    public partial class Payments3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Payments",
                type: "char(12)",
                maxLength: 12,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "char(12)",
                oldMaxLength: 12,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPayed",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ToUserId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ToUserId",
                table: "Payments",
                column: "ToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_ToUserId",
                table: "Payments",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_ToUserId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ToUserId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsPayed",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ToUserId",
                table: "Payments");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Payments",
                type: "char(12)",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(12)",
                oldMaxLength: 12);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

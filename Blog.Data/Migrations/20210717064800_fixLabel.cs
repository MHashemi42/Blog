using Microsoft.EntityFrameworkCore.Migrations;

namespace Blog.Data.Migrations
{
    public partial class fixLabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabelPost_Label_LabelsId",
                table: "LabelPost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Label",
                table: "Label");

            migrationBuilder.RenameTable(
                name: "Label",
                newName: "Labels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Labels",
                table: "Labels",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LabelPost_Labels_LabelsId",
                table: "LabelPost",
                column: "LabelsId",
                principalTable: "Labels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabelPost_Labels_LabelsId",
                table: "LabelPost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Labels",
                table: "Labels");

            migrationBuilder.RenameTable(
                name: "Labels",
                newName: "Label");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Label",
                table: "Label",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LabelPost_Label_LabelsId",
                table: "LabelPost",
                column: "LabelsId",
                principalTable: "Label",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

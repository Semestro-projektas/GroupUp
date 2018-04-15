using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace groupon.Migrations
{
    public partial class v5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Connections",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Connections_ApplicationUserId",
                table: "Connections",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_AspNetUsers_ApplicationUserId",
                table: "Connections",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_AspNetUsers_ApplicationUserId",
                table: "Connections");

            migrationBuilder.DropIndex(
                name: "IX_Connections_ApplicationUserId",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Connections");
        }
    }
}

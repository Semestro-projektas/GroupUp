using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace groupon.Migrations
{
    public partial class v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Hot",
                table: "Groups",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Companies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hot",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Companies");
        }
    }
}

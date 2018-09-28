using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace BCMyProject.Migrations
{
    public partial class index123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlock",
                table: "Photos");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Photos",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Photos");

            migrationBuilder.AddColumn<bool>(
                name: "IsBlock",
                table: "Photos",
                nullable: false,
                defaultValue: false);
        }
    }
}

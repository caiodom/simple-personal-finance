using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SimplePersonalFinance.Infrastructure.Data.Context;

#nullable disable

namespace SimplePersonalFinance.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260906002500_UseDateForUserBirthday")]
public partial class UseDateForUserBirthday : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            ALTER TABLE "Users"
            ALTER COLUMN "BirthdayDate" TYPE date
            USING ("BirthdayDate" AT TIME ZONE 'UTC')::date;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            ALTER TABLE "Users"
            ALTER COLUMN "BirthdayDate" TYPE timestamp with time zone
            USING ("BirthdayDate"::timestamp without time zone AT TIME ZONE 'UTC');
            """);
    }
}

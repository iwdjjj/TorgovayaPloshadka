using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TorgovayaPloshadka.Data.Migrations
{
    /// <inheritdoc />
    public partial class Otchet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE Otchet
	        --@id int
            AS
            BEGIN
	            -- SET NOCOUNT ON added to prevent extra result sets from
	            -- interfering with SELECT statements.
	            SET NOCOUNT ON;

                -- Insert statements for procedure here
	            SELECT N.[ProductId] id,[ProductName] nm, COUNT(P.OrderId) kol FROM [dbo].[Products] N
		            JOIN [dbo].[Orders] P ON N.ProductId=P.ProductId
			            --WHERE OrderId=@id
	            --GROUP BY [ProductName]
                GROUP BY N.[ProductId],[ProductName]
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE Otchet");
        }
    }
}

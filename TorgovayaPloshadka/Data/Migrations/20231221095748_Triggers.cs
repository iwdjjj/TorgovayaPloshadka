using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TorgovayaPloshadka.Data.Migrations
{
    /// <inheritdoc />
    public partial class Triggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE TRIGGER [dbo].[Check_Category]
               ON  [dbo].[Categories]
               AFTER INSERT
            AS 
            BEGIN
	            -- SET NOCOUNT ON added to prevent extra result sets from
	            -- interfering with SELECT statements.
	            IF EXISTS (SELECT 1 FROM inserted WHERE CategoryName IS NULL)
                    BEGIN
                        RAISERROR('Название события не может быть пустым', 50, 2)
                        ROLLBACK TRANSACTION
                        RETURN
                    END

            END");

            migrationBuilder.Sql(@"CREATE TRIGGER [dbo].[AddDelCount]
            ON  [dbo].[Products]
            AFTER INSERT, DELETE
            AS 
            BEGIN
        	    -- SET NOCOUNT ON added to prevent extra result sets from
        	    -- interfering with SELECT statements.
        	    SET NOCOUNT ON;


        	    DECLARE @CategoryId int;
                DECLARE @Count int;       

                SELECT TOP 1 @CategoryId=[CategoryId] FROM INSERTED;
                if (@CategoryId IS NULL) BEGIN
                    SELECT TOP 1 @CategoryId=[CategoryId] FROM DELETED;
                END
                
                SET @Count = (SELECT COUNT(*) FROM [dbo].[Products] WHERE [CategoryId]=@CategoryId)
                UPDATE [dbo].[Categories] SET [Products_Count]=@Count WHERE [CategoryId]=@CategoryId
               
            END");


            migrationBuilder.Sql(@"CREATE TRIGGER [dbo].[UpdCount]
            ON  [dbo].[Products]
            AFTER UPDATE
            AS 
            BEGIN
        	    -- SET NOCOUNT ON added to prevent extra result sets from
        	    -- interfering with SELECT statements.
        	    SET NOCOUNT ON;


        	    DECLARE @CategoryId int;
                DECLARE @CategoryId2 int;
                DECLARE @Count int;       

                SELECT TOP 1 @CategoryId=[CategoryId] FROM INSERTED;
                SELECT TOP 1 @CategoryId2=[CategoryId] FROM DELETED;
                
                SET @Count = (SELECT COUNT(*) FROM [dbo].[Products] WHERE [CategoryId]=@CategoryId)
                UPDATE [dbo].[Categories] SET [Products_Count]=@Count WHERE [CategoryId]=@CategoryId

                SET @Count = (SELECT COUNT(*) FROM [dbo].[Products] WHERE [CategoryId]=@CategoryId2)
                UPDATE [dbo].[Categories] SET [Products_Count]=@Count WHERE [CategoryId]=@CategoryId2 
               
            END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER [dbo].[Check_Category]");

            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS [dbo].[AddDelCount]");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS [dbo].[UpdCount]");
        }
    }
}

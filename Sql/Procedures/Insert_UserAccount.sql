SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_UserAccount]
    @Username NVARCHAR(48),
    @Email NVARCHAR(254),
    @Phone NVARCHAR(20),
    @PasswordHash VARBINARY(128),
    @PasswordSalt VARBINARY(128)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO User_Accounts (Username, Email, Phone, PasswordHash, PasswordSalt)
    VALUES (@Username, @Email, @Phone, @PasswordHash, @PasswordSalt);
    SELECT * FROM User_Accounts WHERE UserId = SCOPE_IDENTITY();
END;
GO

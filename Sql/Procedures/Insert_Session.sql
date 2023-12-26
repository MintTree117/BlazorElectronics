SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_Session]
    @UserId INT,
    @IpAddress NVARCHAR(64) = NULL,
    @Fingerprint NVARCHAR(64) = NULL,
    @SessionHash VARBINARY(128),
    @SessionSalt VARBINARY(128)
AS
BEGIN
    INSERT INTO Sessions (UserId, IpAddress, DeviceFingerprint, TokenHash, TokenSalt)
    VALUES (@UserId, @IpAddress, @Fingerprint, @SessionHash, @SessionSalt);
    SELECT * FROM Sessions WHERE SessionId = SCOPE_IDENTITY();
END;
GO

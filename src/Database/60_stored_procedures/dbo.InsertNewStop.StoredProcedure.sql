/****** Object:  StoredProcedure [dbo].[InsertNewStop]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
DROP PROCEDURE [dbo].[InsertNewStop]
GO

/****** Object:  StoredProcedure [dbo].[InsertNewStop]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Tom Tang
-- Create date: 2017-09-15
-- Description:
-- Revision:
-- =============================================
CREATE PROCEDURE [dbo].[InsertNewStop] 
@pStopName NVARCHAR(50),
@pLongitude FLOAT,
@pLatitude FLOAT,
@pCreatorLongitude FLOAT,
@pCreatorLatitude FLOAT,
@pOwner NVARCHAR(50),
@pCulture NVARCHAR(15),
@oStopID INT OUTPUT
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
		INSERT INTO Stops(StopName, Longitude, Latitude, CreatorLongitude, CreatorLatitude, [Owner], Culture) 
		VALUES(@pStopName, @pLongitude, @pLatitude, @pCreatorLongitude, @pCreatorLatitude, @pOwner, @pCulture)
		SET @oStopID = SCOPE_IDENTITY();
		INSERT INTO StopRating(StopID) VALUES(@oStopID)
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK TRAN
		DECLARE @vErrorMsg VARCHAR(500);
		SET @vErrorMsg = CONVERT(VARCHAR(10), ERROR_NUMBER()) + ':' + ERROR_MESSAGE();
		RAISERROR(@vErrorMsg, 18, 1)
	END CATCH
END
GO

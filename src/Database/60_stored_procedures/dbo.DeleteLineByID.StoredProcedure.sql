/****** Object:  StoredProcedure [dbo].[DeleteLineByID]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
DROP PROCEDURE [dbo].[DeleteLineByID]
GO

/****** Object:  StoredProcedure [dbo].[DeleteLineByID]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
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
CREATE PROCEDURE [dbo].[DeleteLineByID] 
@pLineID INT
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
		DELETE FROM StopRating WHERE StopID IN (SELECT StopID FROM Line2Stop(NOLOCK) WHERE LineID = @pLineID)
		DELETE FROM Line2Stop WHERE LineID = @pLineID
		DELETE FROM Stops WHERE StopID NOT IN (SELECT StopID FROM Line2Stop(NOLOCK))
		DELETE FROM Lines WHERE LineID = @pLineID
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

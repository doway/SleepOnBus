/****** Object:  StoredProcedure [dbo].[DeleteStopByID]    Script Date: 2017/5/15 �W�� 11:08:15 ******/
DROP PROCEDURE [dbo].[DeleteStopByID]
GO

/****** Object:  StoredProcedure [dbo].[DeleteStopByID]    Script Date: 2017/5/15 �W�� 11:08:15 ******/
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
CREATE PROCEDURE [dbo].[DeleteStopByID] 
@pStopID INT
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
		DELETE FROM StopRating WHERE StopID = @pStopID
		DELETE FROM Line2Stop WHERE StopID = @pStopID
		DELETE FROM Stops WHERE StopID = @pStopID
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

/****** Object:  StoredProcedure [dbo].[GetStopIDByOwnerAndStopID]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
DROP PROCEDURE [dbo].[GetStopIDByOwnerAndStopID]
GO

/****** Object:  StoredProcedure [dbo].[GetStopIDByOwnerAndStopID]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
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
CREATE PROCEDURE [dbo].[GetStopIDByOwnerAndStopID]
@pStopID INT,
@pOwner NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT StopID 
	FROM Stops(NOLOCK)
	WHERE StopID = @pStopID AND [Owner] = @pOwner
END
GO

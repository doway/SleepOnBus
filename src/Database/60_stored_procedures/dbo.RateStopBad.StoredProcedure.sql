/****** Object:  StoredProcedure [dbo].[RateStopBad]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
DROP PROCEDURE [dbo].[RateStopBad]
GO

/****** Object:  StoredProcedure [dbo].[RateStopBad]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
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
CREATE PROCEDURE [dbo].[RateStopBad] 
@pStopID INT
AS
BEGIN
	UPDATE StopRating 
	SET RatingBad += 1 
	WHERE StopID = @pStopID
END
GO

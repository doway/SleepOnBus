/****** Object:  StoredProcedure [dbo].[GetStopsByLineID]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
DROP PROCEDURE [dbo].[GetStopsByLineID]
GO

/****** Object:  StoredProcedure [dbo].[GetStopsByLineID]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
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
CREATE PROCEDURE [dbo].[GetStopsByLineID] 
@pLineID INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		s.*, 
		sr.RatingGood, 
		sr.RatingBad 
	FROM 
		Stops s(NOLOCK) 
		INNER JOIN Line2Stop l2s(NOLOCK) ON s.StopID = l2s.StopID
		INNER JOIN StopRating sr(NOLOCK) ON s.StopID = sr.StopID 
	WHERE l2s.LineID = @pLineID
	ORDER BY StopName
END
GO

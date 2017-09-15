/****** Object:  StoredProcedure [dbo].[GetLinesList]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
DROP PROCEDURE [dbo].[GetLinesList]
GO

/****** Object:  StoredProcedure [dbo].[GetLinesList]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
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
CREATE PROCEDURE [dbo].[GetLinesList] 
@pOwner NVARCHAR(50),
@pLatitude FLOAT,
@pLongitude FLOAT,
@pRadius FLOAT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT DISTINCT l.* 
	FROM 
		Lines l(NOLOCK) 
		INNER JOIN Line2Stop l2s(NOLOCK) ON l.LineID = l2s.LineID
		INNER JOIN (
			SELECT StopID 
			FROM Stops(NOLOCK) 
			WHERE 
				([Owner] = @pOwner) OR (ABS(Longitude - @pLongitude) < @pRadius
					AND ABS(Latitude - @pLatitude) < @pRadius)) tmp ON l2s.StopID = tmp.StopID 
	ORDER BY LineName
END
GO

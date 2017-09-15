/****** Object:  StoredProcedure [dbo].[GetLinesListAll]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
DROP PROCEDURE [dbo].[GetLinesListAll]
GO

/****** Object:  StoredProcedure [dbo].[GetLinesListAll]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
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
CREATE PROCEDURE [dbo].[GetLinesListAll] 
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		l.*, 
		(SELECT COUNT(1) FROM Line2Stop l2s(NOLOCK) WHERE l2s.LineID = l.LineID) AS StopCount 
	FROM Lines l(NOLOCK)
	ORDER BY l.LineName
END
GO

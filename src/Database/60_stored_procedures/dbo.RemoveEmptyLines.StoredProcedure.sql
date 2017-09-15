/****** Object:  StoredProcedure [dbo].[RemoveEmptyLines]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
DROP PROCEDURE [dbo].[RemoveEmptyLines]
GO

/****** Object:  StoredProcedure [dbo].[RemoveEmptyLines]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
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
CREATE PROCEDURE [dbo].[RemoveEmptyLines] 
AS
BEGIN
	DELETE FROM Lines 
	WHERE LineID NOT IN (SELECT DISTINCT LineID FROM Line2Stop(NOLOCK))
END
GO

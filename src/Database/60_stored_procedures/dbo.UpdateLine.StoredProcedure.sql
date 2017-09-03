/****** Object:  StoredProcedure [dbo].[UpdateLine]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
DROP PROCEDURE [dbo].[UpdateLine]
GO

/****** Object:  StoredProcedure [dbo].[UpdateLine]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Tom Tang
-- Create date: 2017-09-03
-- Description:
-- Revision:
-- =============================================
CREATE PROCEDURE [dbo].[UpdateLine] 
@pLineID INT,
@pLineName NVARCHAR(50)
AS
BEGIN
	UPDATE Lines SET LineName = @pLineName
	 WHERE LineID = @pLineID
END
GO

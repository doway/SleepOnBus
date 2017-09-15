/****** Object:  StoredProcedure [dbo].[UpdateStop]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
DROP PROCEDURE [dbo].[UpdateStop]
GO

/****** Object:  StoredProcedure [dbo].[UpdateStop]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
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
CREATE PROCEDURE [dbo].[UpdateStop] 
@pStopID INT,
@pStopName NVARCHAR(50)
AS
BEGIN
	UPDATE Stops SET StopName = @pStopName 
	WHERE StopID = @pStopID
END
GO

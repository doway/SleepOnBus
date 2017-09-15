/****** Object:  StoredProcedure [dbo].[InsertNewLine2StopRelation]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
DROP PROCEDURE [dbo].[InsertNewLine2StopRelation]
GO

/****** Object:  StoredProcedure [dbo].[InsertNewLine2StopRelation]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
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
CREATE PROCEDURE [dbo].[InsertNewLine2StopRelation] 
@pLineID INT,
@pStopID INT
AS
BEGIN
	INSERT INTO Line2Stop(LineID, StopID) 
	VALUES(@pLineID, @pStopID)
END
GO

/****** Object:  StoredProcedure [dbo].[InsertNewLine]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
DROP PROCEDURE [dbo].[InsertNewLine]
GO

/****** Object:  StoredProcedure [dbo].[InsertNewLine]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
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
CREATE PROCEDURE [dbo].[InsertNewLine] 
@pLineName NVARCHAR(50),
@pCulture NVARCHAR(15),
@oLineID INT OUTPUT
AS
BEGIN
	INSERT INTO Lines(LineName, Culture) 
	VALUES(@pLineName, @pCulture)

	SET @oLineID = SCOPE_IDENTITY();
END
GO

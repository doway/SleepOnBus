/****** Object:  StoredProcedure [dbo].[LogAUsageCase]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
DROP PROCEDURE [dbo].[LogAUsageCase]
GO

/****** Object:  StoredProcedure [dbo].[LogAUsageCase]    Script Date: 2017/5/15 ¤W¤È 11:08:15 ******/
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
CREATE PROCEDURE [dbo].[LogAUsageCase] 
@pUserCode NVARCHAR(50),
@pLongitude FLOAT,
@pLatitude FLOAT,
@pRadius FLOAT,
@pDevice TINYINT
AS
BEGIN
	INSERT INTO UsageStatistic(UserCode, Longitude, Latitude, Radius, Device) 
	VALUES(@pUserCode, @pLongitude, @pLatitude, @pRadius, @pDevice)
END
GO

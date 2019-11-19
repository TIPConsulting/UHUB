create proc [dbo].[User_GetRecoveryContextByID]

	@RecoveryID nvarchar(100)

as
begin


	select
		*
	from dbo.UserRecovery
	where
		RecoveryID = @RecoveryID
		AND IsEnabled = 1
		AND EffFromDate < sysdatetimeoffset()
		AND EffToDate > sysdatetimeoffset()


end
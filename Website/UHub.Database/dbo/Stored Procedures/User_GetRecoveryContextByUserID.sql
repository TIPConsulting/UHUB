create proc [dbo].[User_GetRecoveryContextByUserID]

	@UserID bigint

as
begin



	select
		*
	from dbo.UserRecovery
	where
		UserID = @UserID
		AND IsEnabled = 1
		AND EffFromDate < sysdatetimeoffset()
		AND EffToDate > sysdatetimeoffset()


end
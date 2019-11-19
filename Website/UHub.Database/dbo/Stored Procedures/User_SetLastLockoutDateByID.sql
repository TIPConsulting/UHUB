create proc dbo.[User_SetLastLockoutDateByID]
	@UserID bigint
as
begin

	update dbo.UserAuthentication
	set
		LastLockoutDate = SYSDATETIMEOFFSET()
	where
		UserID = @UserID


end
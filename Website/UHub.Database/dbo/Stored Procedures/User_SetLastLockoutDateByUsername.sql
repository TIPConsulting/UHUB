create proc dbo.[User_SetLastLockoutDateByUsername]
	@Username nvarchar(100)
as
begin

	declare @_userID bigint
	select 
		@_userID = EntID
	from dbo.Users
	where
		Username = @Username



	update dbo.UserAuthentication
	set
		LastLockoutDate = SYSDATETIMEOFFSET()
	where
		UserID = @_userID


end
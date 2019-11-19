create proc dbo.[User_SetLastLockoutDateByEmail]
	@Email nvarchar(250)
as
begin

	declare @_userID bigint
	select 
		@_userID = EntID
	from dbo.Users
	where
		Email = @Email



	update dbo.UserAuthentication
	set
		LastLockoutDate = SYSDATETIMEOFFSET()
	where
		UserID = @_userID


end
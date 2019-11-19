create proc [dbo].[User_ResetFailedPswdAttemptsByUsername]
	@Username nvarchar(250)
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
		FailedPswdAttemptCount = 0
	where
		UserID = @_userID

end
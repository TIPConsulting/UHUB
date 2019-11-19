create proc [dbo].[User_ResetFailedPswdAttemptsByEmail]
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
		FailedPswdAttemptCount = 0
	where
		UserID = @_userID

end
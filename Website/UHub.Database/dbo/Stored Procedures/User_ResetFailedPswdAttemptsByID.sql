create proc [dbo].[User_ResetFailedPswdAttemptsByID]
	@UserID bigint
as
begin


	update dbo.UserAuthentication
	set
		FailedPswdAttemptCount = 0
	where
		UserID = @UserID

end
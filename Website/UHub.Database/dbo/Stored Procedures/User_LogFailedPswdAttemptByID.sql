create proc [dbo].[User_LogFailedPswdAttemptByID]
	@UserID bigint
as
begin



	if((select FailedPswdAttemptCount from dbo.UserAuthentication where UserID = @UserID) = 0)
	begin
		update dbo.UserAuthentication
		set
			StartFailedPswdWindow = SYSDATETIMEOFFSET()
		where
			UserID = @UserID
	end


	update dbo.UserAuthentication
	set
		FailedPswdAttemptCount = FailedPswdAttemptCount + 1
	where
		UserID = @UserID

end
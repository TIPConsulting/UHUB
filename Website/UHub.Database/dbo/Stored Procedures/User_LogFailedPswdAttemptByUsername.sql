create proc [dbo].[User_LogFailedPswdAttemptByUsername]
	@Username nvarchar(100)
as
begin

	declare @_userID bigint
	select 
		@_userID = EntID
	from dbo.Users
	where
		Username = @Username


	if((select FailedPswdAttemptCount from dbo.UserAuthentication where UserID = @_userID) = 0)
	begin
		update dbo.UserAuthentication
		set
			StartFailedPswdWindow = SYSDATETIMEOFFSET()
		where
			UserID = @_userID
	end


	update dbo.UserAuthentication
	set
		FailedPswdAttemptCount = FailedPswdAttemptCount + 1
	where
		UserID = @_userID

end
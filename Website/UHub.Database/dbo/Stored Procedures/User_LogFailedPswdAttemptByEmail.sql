create proc [dbo].[User_LogFailedPswdAttemptByEmail]
	@Email nvarchar(250)
as
begin

	declare @_userID bigint
	select 
		@_userID = EntID
	from dbo.Users
	where
		Email = @Email


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
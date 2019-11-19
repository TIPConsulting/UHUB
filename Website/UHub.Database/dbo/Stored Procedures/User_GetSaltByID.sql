create proc [dbo].[User_GetSaltByID]
	@UserID bigint
as
begin

	select 
		salt
	from 
		dbo.UserAuthentication
	where
		UserID = @UserID

end

create proc [dbo].[User_GetAuthInfoByID]
	@UserID bigint
as
begin

	select
		ua.*
	from 
		dbo.UserAuthentication ua
	where
		UserID = @UserID
	
end
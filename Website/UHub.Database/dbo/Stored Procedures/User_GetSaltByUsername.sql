create proc [dbo].[User_GetSaltByUsername]
	@Username nvarchar(100)
as
begin

	select 
		auth.salt
	from 
		dbo.Users u
	inner join dbo.UserAuthentication auth
	on
		u.EntID = auth.UserID

	where
		u.Username = @Username

end
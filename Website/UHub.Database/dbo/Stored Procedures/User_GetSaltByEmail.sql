create proc [dbo].[User_GetSaltByEmail]
	@Email nvarchar(250)
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
		u.Email = @Email

end
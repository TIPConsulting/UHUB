
create proc [dbo].[User_GetAuthInfoByEmail]
	@Email nvarchar(250)
as
begin


	select
		ua.*
	from 
		dbo.UserAuthentication ua

	inner join dbo.Users u
	on
		u.EntID = ua.UserID

	where
		u.Email = @Email
	
end
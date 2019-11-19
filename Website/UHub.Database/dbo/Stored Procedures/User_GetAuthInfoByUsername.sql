
CREATE proc [dbo].[User_GetAuthInfoByUsername]
	@Username nvarchar(100),
	@Domain nvarchar(250)
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
		u.Username = @Username
		AND Domain = @Domain
	
end
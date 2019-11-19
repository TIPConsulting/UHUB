create proc [dbo].[User_UpdateVersionByUsername]

	@Username nvarchar(100),
	@Version nvarchar(20)

as
begin


	update dbo.Users
	set
		[Version] = @Version
	where
		Username = @Username


end
create proc [dbo].[User_UpdateVersionByEmail]

	@UserEmail nvarchar(250),
	@Version nvarchar(20)

as
begin


	update dbo.Users
	set
		[Version] = @Version
	where
		Email = @UserEmail


end
create proc [dbo].[User_UpdateVersionByID]

	@UserID bigint,
	@Version nvarchar(20)

as
begin


	update dbo.Users
	set
		[Version] = @Version
	where
		EntID = @UserID


end
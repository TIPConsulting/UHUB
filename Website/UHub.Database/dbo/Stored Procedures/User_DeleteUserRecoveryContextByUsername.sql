create proc [dbo].[User_DeleteUserRecoveryContextByUsername]

	@Username nvarchar(100)

as
begin

	declare @_userID bigint
	select
		@_userID = EntID
	from
		dbo.Users
	where
		Username = @Username


	delete from dbo.UserRecovery
	where
		UserID = @_userID


end
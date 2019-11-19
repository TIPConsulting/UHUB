create proc [dbo].[User_DeleteUserRecoveryContextByEmail]

	@Email nvarchar(250)

as
begin

	declare @_userID bigint
	select
		@_userID = EntID
	from
		dbo.Users
	where
		Email = @Email


	delete from dbo.UserRecovery
	where
		UserID = @_userID


end
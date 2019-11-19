create proc [dbo].[User_DeleteUserRecoveryContextByUserID]

	@UserID bigint

as
begin

	delete from dbo.UserRecovery
	where
		UserID = @UserID


end
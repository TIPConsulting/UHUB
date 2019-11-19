create proc [dbo].[User_DeleteUserRecoveryContextByID]

	@RecoveryID nvarchar(100)

as
begin


	delete from dbo.UserRecovery
	where
		RecoveryID = @RecoveryID


end
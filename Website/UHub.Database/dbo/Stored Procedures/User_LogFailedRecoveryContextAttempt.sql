create proc [dbo].[User_LogFailedRecoveryContextAttempt]

	@RecoveryID nvarchar(100)

as
begin

	declare @attemptCount tinyint
	select
		@attemptCount = AttemptCount
	from UserRecovery
	where
		RecoveryID = @RecoveryID


	if(@attemptCount = 255)
	begin

		--perform no action
		--prevents overflows
		--attempt count will be pinned at maxVal
		return;

	end


	update dbo.UserRecovery
	set
		AttemptCount = AttemptCount + 1
	where
		RecoveryID = @RecoveryID


end
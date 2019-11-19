CREATE proc [dbo].[User_CreateRecoveryContext]
	@UserID bigint,
	@RecoveryKey nvarchar(200),
	@EffToDate datetimeoffset(7),
	@IsOptional bit
as
begin



	if((select IsEnabled from dbo.vUsers where ID = @UserID) = 0)
	begin
		return;
	end


	begin try
		BEGIN TRAN

			--reject system level password reset
			if(@UserID = 0)
			begin
				;throw 51000, '400: Invalid User', 1;
			end


			--delete other recovery entries for the same user
			delete from dbo.UserRecovery
			where
				UserID = @UserID


			--add new entry
			insert into dbo.UserRecovery
			(
				UserID,
				RecoveryKey,
				EffToDate,
				IsOptional
			)
			values
			(
				@UserID,
				@RecoveryKey,
				@EffToDate,
				@IsOptional
			)


			select 
				*
			from dbo.UserRecovery
			where UserID = @UserID

		COMMIT TRAN
	end try
	begin catch
		if(@@TRANCOUNT > 0)
		begin
			ROLLBACK TRAN
		end

		;throw;
	end catch


end
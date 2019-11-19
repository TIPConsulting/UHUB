CREATE proc [dbo].[User_PurgeByID]

	
	@UserID bigint

as
begin

	if(@UserID = 0)
	begin
		return -1
	end



--User cannot be purged if there is any real account activity
--Only used in the event of a catastrophic failure during account creation

	--initial class of modified user
	declare @_userClass smallint
	declare @_userClassName nvarchar(100)


	
	begin try

		if(@UserID is NULL)
		begin
			;throw 51000, '410: Unable to delete specified user', 1
		end


		BEGIN TRAN

			--delete parentage
			delete from
				dbo.EntChildXRef
			where
				ChildEntID = @UserID


			--delete user properties
			delete from
				dbo.EntPropertyXRef
			where
				EntID = @UserID


			--delete user property history
			delete from
				dbo.EntPropertyRevisionXRef
			where
				EntID = @UserID


			--delete user confirmation tokens
			delete from
				dbo.UserConfirmation
			where
				UserID = @UserID


			--delete user psd info
			delete from
				dbo.UserAuthentication
			where
				UserID = @UserID


			--delete user
			delete from
				dbo.Users
			where
				EntID = @UserID


			--delete ent
			delete from
				dbo.Entities
			where
				ID = @UserID

			

		COMMIT TRAN
	end try
	begin catch
		if @@TRANCOUNT > 0
		begin
			ROLLBACK TRAN
		end

		;throw;

	end catch
end
CREATE proc [dbo].[User_PurgeByEmail]

	
	@Email nvarchar(250)

as
begin

	declare @_userID bigint

	select
		@_userID = EntID
	from dbo.Users
	where
		Email = @Email



	if(@_userID = 0)
	begin
		return -1
	end



--User cannot be purged if there is any real account activity
--Only used in the event of a catastrophic failure during account creation

	--initial class of modified user
	declare @_userClass smallint
	declare @_userClassName nvarchar(100)


	
	begin try

		if(@_userID is NULL)
		begin
			;throw 51000, '410: Unable to delete specified user', 1
		end


		BEGIN TRAN

			--delete parentage
			delete from
				dbo.EntChildXRef
			where
				ChildEntID = @_userID


			--delete user properties
			delete from
				dbo.EntPropertyXRef
			where
				EntID = @_userID


			--delete user property history
			delete from
				dbo.EntPropertyRevisionXRef
			where
				EntID = @_userID


			--delete user confirmation tokens
			delete from
				dbo.UserConfirmation
			where
				UserID = @_userID


			--delete user psd info
			delete from
				dbo.UserAuthentication
			where
				UserID = @_userID


			--delete user
			delete from
				dbo.Users
			where
				EntID = @_userID


			--delete ent
			delete from
				dbo.Entities
			where
				ID = @_userID

			

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
CREATE proc [dbo].[User_UpdateByID]

	@UserID bigint,
	@Name nvarchar(200),
	@PhoneNumber nvarchar(50) null,
	@Major nvarchar(200),
	@Year nvarchar(50) null,
	@GradDate nvarchar(10) null,
	@Company nvarchar(100) null,
	@JobTitle nvarchar(100) null,
	@ModifiedBy bigint null

as
begin


	begin try
		BEGIN TRAN


		--USER ENT ID => 1
		declare @_entID bigint
		declare @_entTypeID smallint = 1
		declare @_isNew bit = 0
		

		
		declare @_isEnabled bit
		declare @_isReadOnly bit
		declare @_isDeleted bit

		select 
			@_isEnabled = IsEnabled,
			@_isReadOnly = IsReadOnly,
			@_isDeleted = IsDeleted
		from
			dbo.Entities e
		where
			e.ID = @UserID


		--validate delete flag 
		if(@_isDeleted = 0)
		begin
			;throw 51000, '410: User no longer exists', 1;
		end

		--validate enabled flag
		if(@_isEnabled = 0)
		begin
			;throw 51000, '400: This user cannot be modified', 1;
		end

		--validate readonly flag
		if(@_isReadOnly = 0)
		begin
			;throw 51000, '403: This user cannot be modified', 1;
		end





		--handle empty args
		--default to system user
		if(@ModifiedBy is null)
		begin
			set @ModifiedBy = 0
		end

		declare @ModifiedDate datetimeoffset(7) = sysDateTimeOffset()


		--set props

		--Name [2]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 1,
			@PropValue = @Name,
			@ModifiedBy = @_entID,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew

		--PhoneNum [6]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 6,
			@PropValue = @PhoneNumber,
			@ModifiedBy = @_entID,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew

		--Major [7]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 7,
			@PropValue = @Major,
			@ModifiedBy = @_entID,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew

		--Year [8]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 8,
			@PropValue = @Year,
			@ModifiedBy = @_entID,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew
		
		--GradDate [10]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 10,
			@PropValue = @GradDate,
			@ModifiedBy = @_entID,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew

		--Company [30]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 30,
			@PropValue = @Company,
			@ModifiedBy = @_entID,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew

		--JobTitle [30]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 31,
			@PropValue = @JobTitle,
			@ModifiedBy = @_entID,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew



		update dbo.Entities
		set
			ModifiedBy = @ModifiedBy,
			ModifiedDate = SYSDATETIMEOFFSET()
		where
			ID = @UserID

		


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
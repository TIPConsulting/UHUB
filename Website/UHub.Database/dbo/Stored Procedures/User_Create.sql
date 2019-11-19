CREATE proc [dbo].[User_Create]

	@SchoolID bigint,
	@Email nvarchar(max),
	@Username nvarchar(max),
	@Name nvarchar(max),
	@PhoneNumber nvarchar(max) null,
	@Major nvarchar(max),
	@Year nvarchar(max) null,
	@GradDate nvarchar(max) null,
	@Company nvarchar(max) null,
	@JobTitle nvarchar(max) null,
	@IsFinished bit null,
	@Version nvarchar(max),
	@IsApproved bit,
	@IsConfirmed bit,
	@CreatedBy bigint null
as
begin


	begin try
		BEGIN TRAN

		--USER ENT ID => 1
		declare @_entTypeID smallint = 1
		declare @_entID bigint
		declare @_isNew bit = 1

		declare @_prnt_ID bigint
		declare @_prnt_IsDeleted bit
		declare @_prnt_IsEnabled bit
		declare @_prnt_IsReadOnly bit
		declare @_prnt_EntType smallint



		SELECT
			@_prnt_ID = e.ID,
			@_prnt_IsDeleted = e.IsDeleted,
			@_prnt_IsEnabled = e.IsEnabled,
			@_prnt_IsReadOnly = e.IsReadOnly,
			@_prnt_EntType = e.EntTypeID

		from dbo.Entities e
		where
			e.ID = @SchoolID


		--validate parent existance
		if(@_prnt_ID is null)
		begin
			;throw 51000, '400: Invalid parent entity', 1;
		end


		--validate parent dletetion flag
		if(@_prnt_IsDeleted = 1)
		begin
			;throw 51000, '410: Parent no longer exists', 1;
		end
		--validate parent enabled flag
		if(@_prnt_IsEnabled = 0)
		begin
			;throw 51000, '403: This user cannot be modified', 1;
		end
		--validate parent readonly flag
		if(@_prnt_IsReadOnly = 1)
		begin
			;throw 51000, '400: Parent cannot be modified', 1;
		end


		--handle empty args
		--default to system user
		if(@CreatedBy is null)
		begin
			set @CreatedBy = 0
		end

		declare @CreatedDate datetimeoffset(7) = sysDateTimeOffset()


		--insert item into ent table
		insert into dbo.Entities
		(
			EntTypeID,
			CreatedBy,
			CreatedDate
		)
		values
		( 
			@_entTypeID,
			@CreatedBy,
			@CreatedDate
		)

		select @_entID = SCOPE_IDENTITY()


		declare @domain nvarchar(250)
		set @domain = SUBSTRING(@email, CHARINDEX('@', @email), 250)


		--insert item into user table
		begin try
			insert into dbo.Users
			(
				EntID,
				Email,
				Domain,
				Username,
				[Version],
				IsApproved,
				IsConfirmed
			)
			values
			(
				@_entID,
				@Email,
				@domain,
				@Username,
				@Version,
				@IsApproved,
				@IsConfirmed
			)
		end try
		begin catch
			;throw 51000, '400: Unexpected error', 1;
		end catch


		--insert into parent/child table
		begin try
			insert into dbo.EntChildXRef
			(
				ParentEntID,
				ChildEntID,
				ParentEntType,
				ChildEntType
			)
			values
			(
				@SchoolID,
				@_entID,
				@_prnt_EntType,
				@_entTypeID
			)
		end try
		begin catch
			;throw 51000, '400: Invalid parent entity', 1;
		end catch




		--set props

		--Name [2]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 2,
			@PropValue = @Name,
			@ModifiedBy = @_entID,
			@ModifiedDate = @CreatedDate,
			@IsNewRecord = @_isNew

		--PhoneNum [6]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 6,
			@PropValue = @PhoneNumber,
			@ModifiedBy = @_entID,
			@ModifiedDate = @CreatedDate,
			@IsNewRecord = @_isNew

		--Major [7]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 7,
			@PropValue = @Major,
			@ModifiedBy = @_entID,
			@ModifiedDate = @CreatedDate,
			@IsNewRecord = @_isNew

		--Year [8]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 8,
			@PropValue = @Year,
			@ModifiedBy = @_entID,
			@ModifiedDate = @CreatedDate,
			@IsNewRecord = @_isNew
		
		--GradDate [10]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 10,
			@PropValue = @GradDate,
			@ModifiedBy = @_entID,
			@ModifiedDate = @CreatedDate,
			@IsNewRecord = @_isNew

		--Company [30]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 30,
			@PropValue = @Company,
			@ModifiedBy = @_entID,
			@ModifiedDate = @CreatedDate,
			@IsNewRecord = @_isNew

		--JobTitle [30]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 31,
			@PropValue = @JobTitle,
			@ModifiedBy = @_entID,
			@ModifiedDate = @CreatedDate,
			@IsNewRecord = @_isNew

		--IsFinished [36]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 36,
			@PropValue = @IsFinished,
			@ModifiedBy = @_entID,
			@ModifiedDate = @CreatedDate,
			@IsNewRecord = @_isNew


		exec [dbo].[User_CreateConfirmToken]
			@UserID = @_entID


		select @_entID


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
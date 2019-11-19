


CREATE proc [dbo].[School_Create]

	--CREATE PARAMETERS
	
	--DYNAMIC
	@Name nvarchar(max),
	@State nvarchar(max),
	@City nvarchar(max),
	@DomainValidator nvarchar(max),
	@Description nvarchar(max),

	--CONST ENT FIELDS
	@CreatedBy bigint null = null,
	@IsReadonly bit = 0

as
begin

	--wrap code in transaction -> any error will invalidate the entire insert
	begin try
		BEGIN TRAN

		--POST ENT ID => 2
		--use variable to manage ent type easier
		declare @_entTypeID smallint
		set @_entTypeID = 2
		declare @_entID bigint
		declare @_isNew bit = 1

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


		--get the id of the inserted object
		--SCOPE_IDENTITY()
		--use this to handle property inserts
		select @_entID = SCOPE_IDENTITY()

		--set properties
		--if any insert fails, it will throw an error and invalidate the entire entity

		--Name [2]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 2,
			@PropValue = @Name,
			@ModifiedBy = @CreatedBy,
			@ModifiedDate = @CreatedDate,
			@IsNewRecord = @_isNew

		--State [15]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 15,
			@PropValue = @State,
			@ModifiedBy = @CreatedBy,
			@ModifiedDate = @CreatedDate,
			@IsNewRecord = @_isNew

		--City [16]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 16,
			@PropValue = @City,
			@ModifiedBy = @CreatedBy,
			@ModifiedDate = @CreatedDate,
			@IsNewRecord = @_isNew

		--DomainValidator [17]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 17,
			@PropValue = @DomainValidator,
			@ModifiedBy = @CreatedBy,
			@ModifiedDate = @CreatedDate,
			@IsNewRecord = @_isNew

		--Description [11]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 11,
			@PropValue = @Description,
			@ModifiedBy = @CreatedBy,
			@ModifiedDate = @CreatedDate,
			@IsNewRecord = @_isNew

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
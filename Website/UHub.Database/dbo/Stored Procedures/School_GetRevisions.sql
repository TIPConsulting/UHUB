

create proc [dbo].[School_GetRevisions]

    @EntID bigint

as
begin

	declare @EntTypeID smallint = 2

	declare @ID bigint
	declare @IsEnabled bit
	declare @IsReadOnly bit
	declare @IsDeleted bit
	declare @deletedBy bigint
	declare @deletedDate datetimeoffset(7)
	declare @CreatedBy bigint
	declare @CreatedDate datetimeoffset(7)
	declare @ModifiedDate datetimeoffset(7)
	select
		@ID = ID,
		@IsEnabled = IsEnabled,
		@IsReadOnly = IsReadOnly,
		@IsDeleted = IsDeleted,
		@deletedBy = DeletedBy,
		@deletedDate = DeletedDate,
		@CreatedBy = CreatedBy,
		@CreatedDate = CreatedDate,
		@ModifiedDate = ModifiedDate
	from dbo.Entities
	where
		ID = @EntID;





	with DynamicSet1 as
	(
		select 
			EntID,

                --
			    case when eprx.PropID = 2
			    then PropValue
			    else NULL
			    end [Name],

                --
			    case when eprx.PropID = 15
			    then PropValue
			    else NULL
			    end [State],

                --
			    case when eprx.PropID = 16
			    then PropValue
			    else NULL
			    end [City],

                --
			    case when eprx.PropID = 17
			    then PropValue
			    else NULL
			    end [DomainValidator],

                --
			    case when eprx.PropID = 24
			    then PropValue
			    else NULL
			    end [Description],
			--
			CreatedBy,
			CreatedDate
		from
			EntPropertyRevisionXRef eprx
		where
			eprx.EntID = @EntID
			and eprx.EntTypeID = @EntTypeID
			and (CreatedDate != @ModifiedDate or @IsDeleted = 1)
	),
	DynamicSet2 as
	(
		select
			EntID,
			    min([Name])                                  as [Name],
			    min([State])                                 as [State],
			    min([City])                                  as [City],
			    min([DomainValidator])                       as [DomainValidator],
			    min([Description])                           as [Description],
			    min(CreatedBy)                               as ModifiedBy,
			    min(CreatedDate)                             as ModifiedDate,
			    ROW_NUMBER() over (order by CreatedDate)     as RowNum

		from DynamicSet1
		group by
			EntID,
			CreatedDate
	),	
	RecurseSet as
	(
		select
			prs.EntID,
			    prs.[Name],
			    prs.[State],
			    prs.[City],
			    prs.[DomainValidator],
			    prs.[Description],
			prs.ModifiedBy,
			prs.ModifiedDate,
			prs.RowNum
		from DynamicSet2 prs
		where
			RowNum = 1


		UNION ALL


		select
			rs.EntID,
			    coalesce(prs.[Name], rs.[Name])                           as [Name],
			    coalesce(prs.[State], rs.[State])                         as [State],
			    coalesce(prs.[City], rs.[City])                           as [City],
			    coalesce(prs.[DomainValidator], rs.[DomainValidator])     as [DomainValidator],
			    coalesce(prs.[Description], rs.[Description])             as [Description],
			    coalesce(prs.ModifiedBy, rs.ModifiedBy)                   as ModifiedBy,
			    coalesce(prs.ModifiedDate, rs.ModifiedDate)	              as ModifiedDate,
			    prs.RowNum                                                as RowNum
			
		from DynamicSet2 prs

		inner join RecurseSet rs
		on 
			prs.EntID = rs.EntID
			and prs.RowNum = rs.RowNum + 1

	)


	select
			[ID],
			[EntTypeID],
			[IsEnabled],
			[IsReadonly],
			[IsDeleted],
			[CreatedBy],
			[CreatedDate],
			[ModifiedBy],
			[ModifiedDate],
			[DeletedBy],
			[DeletedDate],
			[Name],
			[State],
			[City],
			[DomainValidator],
			[Description]
    from [dbo].[vSchools]
	where 
		ID = @EntID

	UNION ALL


	select
		@ID			    as ID,
		@EntTypeID		as EntTypeID,
		@IsEnabled		as IsEnabled,
		@IsReadOnly		as IsReadOnly,
		@IsDeleted		as IsDeleted,
		@CreatedBy		as CreatedBy,
		@CreatedDate	as CreatedDate,
		rs.ModifiedBy   as ModifiedBy,
		rs.ModifiedDate as ModifiedDate,
		@deletedBy		as DeletedBy,
		@deletedDate	as DeletedDate,
            rs.[Name]                as [Name],
            rs.[State]               as [State],
            rs.[City]                as [City],
            rs.[DomainValidator]     as [DomainValidator],
            rs.[Description]         as [Description]

	from RecurseSet rs



end
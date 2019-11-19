

create proc [dbo].[Post_GetRevisions]

    @EntID bigint

as
begin

	declare @EntTypeID smallint = 6

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


	declare @parentID bigint
	select
	    @parentID = ParentEntID
	from EntChildXRef
	where
		ChildEntID = @EntID;



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
			    case when eprx.PropID = 12
			    then PropValue
			    else NULL
			    end [Content],

                --
			    case when eprx.PropID = 13
			    then PropValue
			    else NULL
			    end [IsModified],

                --
			    case when eprx.PropID = 14
			    then PropValue
			    else NULL
			    end [ViewCount],

                --
			    case when eprx.PropID = 33
			    then PropValue
			    else NULL
			    end [IsLocked],

                --
			    case when eprx.PropID = 34
			    then PropValue
			    else NULL
			    end [CanComment],

                --
			    case when eprx.PropID = 35
			    then PropValue
			    else NULL
			    end [IsPublic],
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
			    min([Content])                               as [Content],
			    cast(min([IsModified]) as bit)               as [IsModified],
			    cast(min([ViewCount]) as bigint)             as [ViewCount],
			    cast(min([IsLocked]) as bit)                 as [IsLocked],
			    cast(min([CanComment]) as bit)               as [CanComment],
			    cast(min([IsPublic]) as bit)                 as [IsPublic],
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
			    prs.[Content],
			    prs.[IsModified],
			    prs.[ViewCount],
			    prs.[IsLocked],
			    prs.[CanComment],
			    prs.[IsPublic],
			prs.ModifiedBy,
			prs.ModifiedDate,
			prs.RowNum
		from DynamicSet2 prs
		where
			RowNum = 1


		UNION ALL


		select
			rs.EntID,
			    coalesce(prs.[Name], rs.[Name])                 as [Name],
			    coalesce(prs.[Content], rs.[Content])           as [Content],
			    coalesce(prs.[IsModified], rs.[IsModified])     as [IsModified],
			    coalesce(prs.[ViewCount], rs.[ViewCount])       as [ViewCount],
			    coalesce(prs.[IsLocked], rs.[IsLocked])         as [IsLocked],
			    coalesce(prs.[CanComment], rs.[CanComment])     as [CanComment],
			    coalesce(prs.[IsPublic], rs.[IsPublic])         as [IsPublic],
			    coalesce(prs.ModifiedBy, rs.ModifiedBy)         as ModifiedBy,
			    coalesce(prs.ModifiedDate, rs.ModifiedDate)	    as ModifiedDate,
			    prs.RowNum                                      as RowNum
			
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
			[ParentID],
			[Name],
			[Content],
			[IsModified],
			[ViewCount],
			[IsLocked],
			[CanComment],
			[IsPublic]
    from [dbo].[vPosts]
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
		    @parentID		as ParentID,
            rs.[Name]           as [Name],
            rs.[Content]        as [Content],
            rs.[IsModified]     as [IsModified],
            rs.[ViewCount]      as [ViewCount],
            rs.[IsLocked]       as [IsLocked],
            rs.[CanComment]     as [CanComment],
            rs.[IsPublic]       as [IsPublic]

	from RecurseSet rs



end
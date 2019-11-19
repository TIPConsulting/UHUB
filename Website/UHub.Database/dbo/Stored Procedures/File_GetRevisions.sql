

create proc [dbo].[File_GetRevisions]

    @EntID bigint

as
begin

	declare @EntTypeID smallint = 8

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
			    case when eprx.PropID = 14
			    then PropValue
			    else NULL
			    end [ViewCount],

                --
			    case when eprx.PropID = 23
			    then PropValue
			    else NULL
			    end [FilePath],

                --
			    case when eprx.PropID = 24
			    then PropValue
			    else NULL
			    end [Description],

                --
			    case when eprx.PropID = 25
			    then PropValue
			    else NULL
			    end [FileHash_SHA256],

                --
			    case when eprx.PropID = 26
			    then PropValue
			    else NULL
			    end [SourceName],

                --
			    case when eprx.PropID = 28
			    then PropValue
			    else NULL
			    end [SourceType],

                --
			    case when eprx.PropID = 29
			    then PropValue
			    else NULL
			    end [DownloadName],
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
			    cast(min([ViewCount]) as bigint)             as [ViewCount],
			    min([FilePath])                              as [FilePath],
			    min([Description])                           as [Description],
			    min([FileHash_SHA256])                       as [FileHash_SHA256],
			    min([SourceName])                            as [SourceName],
			    min([SourceType])                            as [SourceType],
			    min([DownloadName])                          as [DownloadName],
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
			    prs.[ViewCount],
			    prs.[FilePath],
			    prs.[Description],
			    prs.[FileHash_SHA256],
			    prs.[SourceName],
			    prs.[SourceType],
			    prs.[DownloadName],
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
			    coalesce(prs.[ViewCount], rs.[ViewCount])                 as [ViewCount],
			    coalesce(prs.[FilePath], rs.[FilePath])                   as [FilePath],
			    coalesce(prs.[Description], rs.[Description])             as [Description],
			    coalesce(prs.[FileHash_SHA256], rs.[FileHash_SHA256])     as [FileHash_SHA256],
			    coalesce(prs.[SourceName], rs.[SourceName])               as [SourceName],
			    coalesce(prs.[SourceType], rs.[SourceType])               as [SourceType],
			    coalesce(prs.[DownloadName], rs.[DownloadName])           as [DownloadName],
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
			[ParentID],
			[Name],
			[ViewCount],
			[FilePath],
			[Description],
			[FileHash_SHA256],
			[SourceName],
			[SourceType],
			[DownloadName]
    from [dbo].[vFiles]
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
            rs.[Name]                as [Name],
            rs.[ViewCount]           as [ViewCount],
            rs.[FilePath]            as [FilePath],
            rs.[Description]         as [Description],
            rs.[FileHash_SHA256]     as [FileHash_SHA256],
            rs.[SourceName]          as [SourceName],
            rs.[SourceType]          as [SourceType],
            rs.[DownloadName]        as [DownloadName]

	from RecurseSet rs



end
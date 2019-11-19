

create proc [dbo].[SchoolClubModerator_GetRevisions]

    @EntID bigint

as
begin

	declare @EntTypeID smallint = 5

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
			    case when eprx.PropID = 18
			    then PropValue
			    else NULL
			    end [UserID],

                --
			    case when eprx.PropID = 19
			    then PropValue
			    else NULL
			    end [IsOwner],

                --
			    case when eprx.PropID = 22
			    then PropValue
			    else NULL
			    end [IsValid],
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
			    cast(min([UserID]) as bigint)                as [UserID],
			    cast(min([IsOwner]) as bit)                  as [IsOwner],
			    cast(min([IsValid]) as bit)                  as [IsValid],
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
			    prs.[UserID],
			    prs.[IsOwner],
			    prs.[IsValid],
			prs.ModifiedBy,
			prs.ModifiedDate,
			prs.RowNum
		from DynamicSet2 prs
		where
			RowNum = 1


		UNION ALL


		select
			rs.EntID,
			    coalesce(prs.[UserID], rs.[UserID])             as [UserID],
			    coalesce(prs.[IsOwner], rs.[IsOwner])           as [IsOwner],
			    coalesce(prs.[IsValid], rs.[IsValid])           as [IsValid],
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
			[UserID],
			[IsOwner],
			[IsValid]
    from [dbo].[vSchoolClubModerators]
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
            rs.[UserID]      as [UserID],
            rs.[IsOwner]     as [IsOwner],
            rs.[IsValid]     as [IsValid]

	from RecurseSet rs



end
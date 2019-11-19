

create proc [dbo].[User_GetRevisions]

    @EntID bigint

as
begin

	declare @EntTypeID smallint = 1

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
			    case when eprx.PropID = 6
			    then PropValue
			    else NULL
			    end [PhoneNumber],

                --
			    case when eprx.PropID = 7
			    then PropValue
			    else NULL
			    end [Major],

                --
			    case when eprx.PropID = 8
			    then PropValue
			    else NULL
			    end [Year],

                --
			    case when eprx.PropID = 10
			    then PropValue
			    else NULL
			    end [ExpectedGradDate],

                --
			    case when eprx.PropID = 30
			    then PropValue
			    else NULL
			    end [Company],

                --
			    case when eprx.PropID = 31
			    then PropValue
			    else NULL
			    end [JobTitle],

                --
			    case when eprx.PropID = 36
			    then PropValue
			    else NULL
			    end [IsFinished],
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
			    min([PhoneNumber])                           as [PhoneNumber],
			    min([Major])                                 as [Major],
			    min([Year])                                  as [Year],
			    min([ExpectedGradDate])                      as [ExpectedGradDate],
			    min([Company])                               as [Company],
			    min([JobTitle])                              as [JobTitle],
			    cast(min([IsFinished]) as bit)               as [IsFinished],
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
			    prs.[PhoneNumber],
			    prs.[Major],
			    prs.[Year],
			    prs.[ExpectedGradDate],
			    prs.[Company],
			    prs.[JobTitle],
			    prs.[IsFinished],
			prs.ModifiedBy,
			prs.ModifiedDate,
			prs.RowNum
		from DynamicSet2 prs
		where
			RowNum = 1


		UNION ALL


		select
			rs.EntID,
			    coalesce(prs.[Name], rs.[Name])                             as [Name],
			    coalesce(prs.[PhoneNumber], rs.[PhoneNumber])               as [PhoneNumber],
			    coalesce(prs.[Major], rs.[Major])                           as [Major],
			    coalesce(prs.[Year], rs.[Year])                             as [Year],
			    coalesce(prs.[ExpectedGradDate], rs.[ExpectedGradDate])     as [ExpectedGradDate],
			    coalesce(prs.[Company], rs.[Company])                       as [Company],
			    coalesce(prs.[JobTitle], rs.[JobTitle])                     as [JobTitle],
			    coalesce(prs.[IsFinished], rs.[IsFinished])                 as [IsFinished],
			    coalesce(prs.ModifiedBy, rs.ModifiedBy)                     as ModifiedBy,
			    coalesce(prs.ModifiedDate, rs.ModifiedDate)	                as ModifiedDate,
			    prs.RowNum                                                  as RowNum
			
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
			[PhoneNumber],
			[Major],
			[Year],
			[ExpectedGradDate],
			[Company],
			[JobTitle],
			[IsFinished],
			[Email],
			[Domain],
			[Username],
			[IsConfirmed],
			[IsApproved],
			[Version],
			[IsAdmin]
    from [dbo].[vUsers]
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
            rs.[Name]                  as [Name],
            rs.[PhoneNumber]           as [PhoneNumber],
            rs.[Major]                 as [Major],
            rs.[Year]                  as [Year],
            rs.[ExpectedGradDate]      as [ExpectedGradDate],
            rs.[Company]               as [Company],
            rs.[JobTitle]              as [JobTitle],
            rs.[IsFinished]            as [IsFinished],
            fkJoin_0.[Email]           as [Email],
            fkJoin_0.[Domain]          as [Domain],
            fkJoin_0.[Username]        as [Username],
            fkJoin_0.[IsConfirmed]     as [IsConfirmed],
            fkJoin_0.[IsApproved]      as [IsApproved],
            fkJoin_0.[Version]         as [Version],
            fkJoin_0.[IsAdmin]         as [IsAdmin]

	from RecurseSet rs


        INNER JOIN dbo.Users fkJoin_0
	    on
		    fkJoin_0.EntID = @ID

end
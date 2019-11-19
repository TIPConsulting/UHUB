











CREATE proc [dbo].[Posts_GetBySchool]

	@SchoolID bigint

as
begin

	--get list of clubs that are children of the specified school
	select
		xr.ChildEntID
	into #tmpParentSet
	from
		dbo.EntChildXRef xr
	where
		ParentEntID = @SchoolID
		and ChildEntType = 4;		--SCHOOL CLUB TYPE ID [4]

	--include school in possible-parent set
	insert into #tmpParentSet
	values (@SchoolID);




	select
		vu.ID,
		vu.IsEnabled,
		vu.IsReadonly,
		vu.[Name],
		vu.[Content],
		vu.[IsModified],
		vu.[ViewCount],
		vu.[IsLocked],
		vu.[CanComment],
		vu.[IsPublic],
		vu.ParentID,
		vu.[IsDeleted],
		vu.[CreatedBy],
		vu.[CreatedDate],
		vu.[ModifiedBy],
		vu.[ModifiedDate],
		vu.[DeletedBy],
		vu.[DeletedDate]
	from dbo.vPosts vu
		
	--join results onto parent set
	inner join #tmpParentSet ps
	on
		ps.ChildEntID = vu.ParentID



	drop table #tmpParentSet


end






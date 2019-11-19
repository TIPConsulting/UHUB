CREATE proc [dbo].[Posts_GetCountBySchool]

	@SchoolID bigint
as
begin

	--get all posts that descend from a school
	with recurseSearch as
	(
		select
			ecx.ParentEntID,
			ecx.ParentEntType,
			ecx.ChildEntID,
			ecx.ChildEntType,
			ecx.IsDeleted
		from dbo.EntChildXRef ecx
		where
			ParentEntID = 1
			AND (
				ChildEntType = 4		--CLUB TYPE [4]
				OR ChildEntType = 6)	--POST TYPE [6]
			AND IsDeleted = 0


		union all


		select
			ecx.ParentEntID,
			ecx.ParentEntType,
			ecx.ChildEntID,
			ecx.ChildEntType,
			ecx.IsDeleted
		from dbo.EntChildXRef ecx
		inner join recurseSearch rs
		on
			rs.ChildEntID = ecx.ParentEntID
		where
			ecx.IsDeleted = 0
			AND (
				ecx.ChildEntType = 4		--CLUB TYPE [4]
				OR ecx.ChildEntType = 6)	--POST TYPE [6]
	)



	select
		count(*)
	from recurseSearch rs
	inner join dbo.Entities e
	on
		e.ID = rs.ChildEntID
		and rs.ChildEntType = 6	--POST TYPE [6]
		and e.IsDeleted = 0
		

end
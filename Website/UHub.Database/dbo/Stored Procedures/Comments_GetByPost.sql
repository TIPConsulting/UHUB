












CREATE proc [dbo].[Comments_GetByPost]

	@PostID bigint

as
begin


	with recursiveFind as
	(

		select
			ChildEntID,
			ChildEntType,
			ParentEntID,
			ParentEntType
		from dbo.EntChildXRef ecx
		where
			ecx.ChildEntType = 7
			and ecx.ParentEntID = @PostID 
			and ecx.ParentEntType = 6 --POST TYPE [6]
			and ecx.IsDeleted = 0


		UNION ALL

	
		select
			ecx.ChildEntID,
			ecx.ChildEntType,
			ecx.ParentEntID,
			ecx.ParentEntType
		from dbo.EntChildXRef ecx

		inner join recursiveFind rf
		on
			ecx.ParentEntID = rf.ChildEntID
			and ecx.ParentEntType = 7 --COMMENT TYPE [6]

		where
			ecx.ChildEntType = 7
			and ecx.IsDeleted = 0
	)

	select
		vu.ID,
		vu.IsEnabled,
		vu.IsReadonly,
		vu.[Content],
		vu.[IsModified],
		vu.[ViewCount],
		vu.ParentID,
		vu.[IsDeleted],
		vu.[CreatedBy],
		vu.[CreatedDate],
		vu.[ModifiedBy],
		vu.[ModifiedDate],
		vu.[DeletedBy],
		vu.[DeletedDate]
	from recursiveFind rf
	
	inner join dbo.vComments vu
	on
		vu.ID = rf.ChildEntID



end







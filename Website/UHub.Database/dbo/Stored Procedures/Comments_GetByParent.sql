













CREATE proc [dbo].[Comments_GetByParent]

	@ParentID bigint

as
begin


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


from dbo.vComments vu

inner JOIN dbo.EntChildXRef
ON
	ChildEntID = vu.ID
	and ChildEntType = 7
	and ParentEntID = @ParentID 
	AND
	(
		ParentEntType = 6 --POST TYPE [6]
		OR ParentEntType = 7 --COMMENT TYPE [7]
	)


end








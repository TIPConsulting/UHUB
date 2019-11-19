












create proc [dbo].[Posts_GetByParent]

	@ParentID bigint

as
begin


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

inner join dbo.EntChildXRef ecx
ON 
	ecx.ChildEntID = vu.ID
	and ecx.ChildEntType = 6		-- POST TYPE [6]
	and ecx.ParentEntID = @ParentID

end
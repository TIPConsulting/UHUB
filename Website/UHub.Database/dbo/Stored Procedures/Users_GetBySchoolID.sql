CREATE proc [dbo].[Users_GetBySchoolID]

	@SchoolID bigint

as
begin


select
	vu.ID,
	vu.IsEnabled,
	vu.IsReadonly,
	vu.Email,
	vu.Username,
	vu.IsConfirmed,
	vu.IsApproved,
	vu.[Version],
	vu.[IsAdmin],
	vu.[Name],
	vu.[PhoneNumber],
	vu.[Major],
	vu.[Year],
	vu.ExpectedGradDate,
	vu.[Company],
	vu.[JobTitle],
	vu.[IsFinished],
	vu.[ParentID],
	vu.[IsDeleted],
	vu.[CreatedBy],
	vu.[CreatedDate],
	vu.[ModifiedBy],
	vu.[ModifiedDate],
	vu.[DeletedBy],
	vu.[DeletedDate]


from dbo.vUsers vu

inner join EntChildXRef ecx
on
	ecx.ChildEntID = vu.ID
	and ParentEntType = 2		--SCHOOL TYPE [2]
	and ParentEntID = @SchoolID


end
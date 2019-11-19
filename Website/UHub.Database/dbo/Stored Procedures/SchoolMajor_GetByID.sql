







CREATE proc [dbo].[SchoolMajor_GetByID]

	@SchoolMajorID bigint

as
begin


select
	vu.ID,
	vu.IsEnabled,
	vu.IsReadonly,
	vu.[Name],
	vu.[Description],
	vu.[IsDeleted],
	vu.[CreatedBy],
	vu.[CreatedDate],
	vu.[ModifiedBy],
	vu.[ModifiedDate],
	vu.[DeletedBy],
	vu.[DeletedDate]


from dbo.vSchoolMajors vu


WHERE

	vu.ID = @SchoolMajorID

end





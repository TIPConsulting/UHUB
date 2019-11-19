








CREATE proc [dbo].[SchoolClub_GetByID]

	@SchoolClubID bigint

as
begin


select
	vu.ID,
	vu.IsEnabled,
	vu.IsReadonly,
	vu.[Name],
	vu.[Description],
	vu.[ParentID],
	vu.[IsDeleted],
	vu.[CreatedBy],
	vu.[CreatedDate],
	vu.[ModifiedBy],
	vu.[ModifiedDate],
	vu.[DeletedBy],
	vu.[DeletedDate]


from dbo.vSchoolClubs vu


WHERE

	vu.ID = @SchoolClubID

end












CREATE proc [dbo].[School_GetByID]

	@SchoolID bigint


as
begin


select
	vu.ID,
	vu.IsEnabled,
	vu.IsReadonly,
	vu.[Name],
	vu.[State],
	vu.[City],
	vu.[DomainValidator],
	vu.[Description],
	vu.[IsDeleted],
	vu.[CreatedBy],
	vu.[CreatedDate],
	vu.[ModifiedBy],
	vu.[ModifiedDate],
	vu.[DeletedBy],
	vu.[DeletedDate]


from dbo.vSchools vu

where
	vu.ID = @SchoolID 


end






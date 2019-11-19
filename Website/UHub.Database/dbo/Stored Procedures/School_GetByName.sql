



CREATE proc [dbo].[School_GetByName]

	@SchoolName nvarchar(200)


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
	vu.Name = @SchoolName 


end




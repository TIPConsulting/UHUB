

CREATE proc [dbo].[User_GetByUsername]

	@Username nvarchar(100),
	@Domain nvarchar(250)

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
	vu.[ExpectedGradDate],
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

where
	vu.Username = @Username 
	AND vu.Domain = @Domain

end


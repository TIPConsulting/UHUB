create proc User_GetConfirmTokenByID

	@RefUID nvarchar(100)

as
begin

	--there will only be one active confirmation token at a time


	select
		UserID,
		RefUID, 
		CreatedDate,
		ConfirmedDate,
		IsConfirmed,
		IsDeleted
	from
		UserConfirmation
	where
		RefUID = @RefUID
		and IsDeleted = 0

end
create proc User_GetConfirmTokenByUserID

	@UserID bigint

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
		UserID = @UserID
		and IsDeleted = 0

end
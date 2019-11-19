create proc User_CreateConfirmToken

	@UserID bigint

as
begin

	begin tran

	--delete all previous confirmation tokens
	update
		UserConfirmation
	set
		IsDeleted = 1
	where
		UserID = @UserID
		and IsConfirmed = 0


	insert into UserConfirmation
	(
		UserID
	)
	values
	(
		@UserID
	)


	IF @@TRANCOUNT > 0
	begin
		commit tran;
	end

end
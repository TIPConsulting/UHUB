
CREATE proc [dbo].[User_UpdateConfirmFlag]

	@RefUID nvarchar(100),
	@MinCreatedDate datetimeoffset(7)


as
begin



	
	declare @userID bigint
	declare @confirmationCreated datetimeoffset(7)
	declare @confirmDeleted bit

	select
		@userID = UserID,
		@confirmationCreated = CreatedDate,
		@confirmDeleted = IsDeleted
	from UserConfirmation


	--ensure RefUID is valid
	if(@userID is null)
	begin
		select cast(0 as bit)
		return;
	end


	if(@confirmDeleted = 1)
	begin
		select cast(0 as bit)
		return;
	end


	--used to track expirations
	--if the confirmation token was created before the MinKeepDate, then it is expired
	if(@confirmationCreated < @MinCreatedDate)
	begin
		select cast(0 as bit)
		return;
	end



	update dbo.UserConfirmation
	set
		IsConfirmed = 1,
		ConfirmedDate = SYSDATETIMEOFFSET()
	where
		UserID = @userID
		and RefUID = RefUID

		
	update dbo.Users
	set
		IsConfirmed = 1
	where
		EntID = @userID	


	select cast(1 as bit)

end

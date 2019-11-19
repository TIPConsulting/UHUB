CREATE proc [dbo].[User_UpdatePasswordByUsername]
	@Username nvarchar(100),
	@PswdHash nvarchar(250),
	@Salt nvarchar(100)
as
begin
	declare @_userID int
	select 
		@_userID = EntID
	from dbo.Users
	where
		Username = @Username


	declare @_pswdModDate datetimeoffset
	set @_pswdModDate = sysdatetimeoffset()



	if(@_userID = 0)
	begin
		;throw 51000, '403: This user cannot be modified', 1;
	end


	if (not exists (select * from dbo.UserAuthentication where UserID = @_userID))
	begin
		insert into dbo.UserAuthentication
		(
			UserID,
			[PswdHash],
			Salt,
			PswdCreatedDate 
		)
		values
		(
			@_userID,
			@PswdHash,
			@Salt,
			@_pswdModDate
		)

	end
	else begin
		update dbo.UserAuthentication
		set
			[PswdHash] = @PswdHash,
			Salt = @Salt,
			PswdModifiedDate = @_pswdModDate
		where
			UserID = @_userID

	end


	select @_pswdModDate

	
end


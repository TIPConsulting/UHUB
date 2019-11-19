CREATE proc [dbo].[User_UpdatePasswordByID]
	@UserID bigint,
	@PswdHash nvarchar(250),
	@Salt nvarchar(100)
as
begin


	declare @_pswdModDate datetimeoffset
	set @_pswdModDate = sysdatetimeoffset()


	if(@UserID = 0)
	begin
		;throw 51000, '403: This user cannot be modified', 1;
	end


	if (not exists (select * from dbo.UserAuthentication where UserID = @UserID))
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
			@UserID,
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
			UserID = @UserID

	end


	select @_pswdModDate

	
end


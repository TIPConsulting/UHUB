CREATE proc [dbo].[User_DeleteByID]

	@UserID bigint,
	@DeletedBy bigint null

as
begin

	
	declare @_entTypeID smallint
	declare @_isDeleted bit
	declare @_isReadOnly bit
	declare @_isAdmin bit

	--ENT USER TYPE => 1

	select 
		@_entTypeID = e.EntTypeID,
		@_isReadOnly = e.IsReadOnly,
		@_isAdmin = u.IsAdmin
	from dbo.Entities e
	inner join dbo.Users u
	on
		e.ID = u.EntID
	where
		e.ID = @UserID


	--validate user type
	if(@_entTypeID != 1)
	begin
		;throw 51000, '400: Invalid operation', 1;
	end

	--validate isDeleted type
	if(@_isDeleted = 1)
	begin
		;throw 51000, '410: User no longer exists', 1;
	end

	--validate readonly flag
	if(@_isReadOnly = 1)
	begin
		;throw 51000, '403: This user cannot be modified', 1;
	end


	--validate isAdmin flag
	if(@_isAdmin = 1)
	begin
		;throw 51000, '403: This user cannot be modified', 1;
	end



	--perform delete operation
	update dbo.Entities
	set
		IsDeleted = 1,
		DeletedDate = SYSDATETIMEOFFSET(),
		DeletedBy = coalesce(@DeletedBy, 0)

	where
		ID = @UserID



end
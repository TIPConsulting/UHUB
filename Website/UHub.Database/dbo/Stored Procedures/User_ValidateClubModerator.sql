
CREATE proc [dbo].[User_ValidateClubModerator]
	@UserID bigint,
	@ClubID bigint

as
begin


	declare @user_IsAdmin bit
	declare @user_IsDeleted bit
	declare @user_IsEnabled bit
	declare @user_IsApproved bit
	declare @user_IsConfirmed bit

	select
		@user_IsAdmin = u.IsAdmin,
		@user_IsEnabled = e.IsEnabled,
		@user_IsConfirmed = u.IsConfirmed,
		@user_IsApproved = u.IsApproved,
		@user_IsDeleted = e.IsDeleted
	from dbo.Users u
	inner join dbo.Entities e
	on
		e.ID = u.EntID
	where
		u.EntID = @UserID


	--user not found
	--user deleted
	if(
		@user_IsAdmin is null
		OR @user_IsEnabled = 0
		OR @user_IsConfirmed = 0
		OR @user_IsApproved = 0
		OR @user_IsDeleted = 1
	)
	begin
		select cast (0 as bit)
		return;
	end




	declare @schoolType smallint = 2; --SCHOOL TYPE [1]


	declare @user_school_ID bigint
	select 
		@user_school_ID = ParentEntID
	from dbo.EntChildXRef 
	where 
		ChildEntID = @UserID
		and ParentEntType = @schoolType


	declare @club_IsDeleted bit
	select
		@club_IsDeleted = e.IsDeleted
	from dbo.vSchoolClubs e
	where
		e.ID = @ClubID


	--club not found
	--club deleted
	if(@club_IsDeleted = 1)
	begin
		select cast (0 as bit)
		return;
	end

	declare @club_school_ID bigint
	select 
		@club_school_ID = ParentEntID
	from dbo.EntChildXRef 
	where 
		ChildEntID = @ClubID
		and ParentEntType = @schoolType


	if (
		@user_school_ID != @club_school_ID
	)
	begin
		select cast (0 as bit)
		return;
	end

	if ( exists (
	select
		*
	from vSchoolClubModerators vscm
	where 
		@UserID = vscm.[UserID]
		and vscm.IsOwner = 1
	))
	begin
		select cast (1 as bit)
		return;
	end

	--admins can post anywhere
	if(@user_IsAdmin = 1)
	begin
		select cast (1 as bit)
		return;
	end

	select cast (0 as bit)

end
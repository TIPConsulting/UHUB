CREATE proc [dbo].[SchoolClub_IsUserBanned]
	@ClubID bigint,
	@UserID bigint

as
begin

	declare @club_Parent bigint
	declare @user_Parent bigint

	select
		@club_Parent = ParentEntID
	from dbo.EntChildXRef ecx
	where
		ecx.ParentEntType = 2		--SCHOOL TYPE [2]
		AND ecx.ChildEntType = 4	--CLUB TYPE [4]
		AND ecx.ChildEntID = @ClubID



	
	select
		@user_Parent = ParentEntID
	from dbo.EntChildXRef ecx
	where
		ecx.ParentEntType = 2		--SCHOOL TYPE [2]
		AND ecx.ChildEntType = 1	--USER TYPE [1]
		AND ecx.ChildEntID = @UserID


	--verify existance
	if(@club_Parent is null OR @user_Parent is null)
	begin
		select cast(1 as bit)
		return;
	end

	--verify same parent school
	if(@club_Parent != @user_Parent)
	begin
		select cast(1 as bit)
		return;
	end


	--verify valid membership
	if(exists(select * from dbo.EntMemberXRef where TargetEntID = @ClubID AND ActorEntID = @UserID AND IsBanned = 1))
	begin
		select cast(1 as bit)

	end
	else begin
		select cast(0 as bit)
	end



end
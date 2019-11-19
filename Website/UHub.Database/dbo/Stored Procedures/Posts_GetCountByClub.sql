CREATE proc [dbo].[Posts_GetCountByClub]

	@ClubID bigint,
	@IncludePrivatePosts bit = 0

as
begin

	--only public posts
	if(@IncludePrivatePosts = 0)
	begin

		select
			count(*)
		from dbo.EntChildXRef ecx
		inner join EntPropertyXRef epx
		on
			epx.EntID = ecx.ChildEntID
			and epx.PropID = 35			--IS PUBLIC [35]
			and epx.PropValue = '1'
		where
			ParentEntID = @ClubID
			AND ChildEntType = 6	--POST TYPE [6]
			and ParentEntType = 4	-- SCHOOL CLUB TYPE [4]
			AND IsDeleted = 0
	end
	--all posts
	else begin

		select
			count(*)
		from dbo.EntChildXRef ecx
		where
			ParentEntID = @ClubID
			AND ChildEntType = 6	--POST TYPE [6]
			and ParentEntType = 4	-- SCHOOL CLUB TYPE [4]
			AND IsDeleted = 0

	end


end
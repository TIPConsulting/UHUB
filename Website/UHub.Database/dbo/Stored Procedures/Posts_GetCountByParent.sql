CREATE proc [dbo].[Posts_GetCountByParent]

	@ParentID bigint,
	@IncludePrivatePosts bit = 0

as
begin


	--only public posts
	if(@IncludePrivatePosts = 0)
	begin

		select
			count(*)
		from dbo.EntChildXRef ecx
		left join EntPropertyXRef epx
		on
			epx.EntID = ecx.ChildEntID
			and epx.PropID = 35			--IS PUBLIC [35]
			and (epx.PropValue is null or epx.PropValue = '1')
		where
			ParentEntID = @ParentID
			AND ChildEntType = 6	--POST TYPE [6]
			AND IsDeleted = 0
	end
	--all posts
	else begin

		select
			count(*)
		from dbo.EntChildXRef ecx
		where
			ParentEntID = @ParentID
			AND ChildEntType = 6	--POST TYPE [6]
			AND IsDeleted = 0

	end


end
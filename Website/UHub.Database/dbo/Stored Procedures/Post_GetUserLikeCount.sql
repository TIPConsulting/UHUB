CREATE proc Post_GetUserLikeCount

	@PostID bigint


as
begin


	select
		cast(count(*) as bigint)
	from dbo.EntLikeXRef
	where
		TargetEntID = @PostID
		and TargetEntTypeID = 6		--POST TYPE[6]
		and ActorEntTypeID = 1		--USER TYPE[1]

		 

end
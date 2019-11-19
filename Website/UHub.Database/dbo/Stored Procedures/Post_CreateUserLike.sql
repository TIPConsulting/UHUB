CREATE proc [dbo].[Post_CreateUserLike]

	@PostID bigint,
	@UserID bigint



as
begin

	begin try


		insert into dbo.EntLikeXRef
		(
			TargetEntID,
			ActorEntID,
			TargetEntTypeID,
			ActorEntTypeID
		)
		values
		(
			@PostID,
			@UserID,
			6,			--POST TYPE [6]
			1			--USER TYPE [1]
		)

		select cast(1 as bit)

	end try
	begin catch

		select cast(0 as bit)

	end catch
end
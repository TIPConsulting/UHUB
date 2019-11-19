CREATE proc [dbo].[Post_IncrementViewCount]

	@PostID bigint,
	@UserID bigint

as
begin

	declare @postEntType smallint = 6
	declare @userEntType smallint = 1


	begin try

		if(exists (select * from Entities where ID = @PostID and EntTypeID = 6 and IsDeleted = 0))	--POST TYPE [6]
		begin
		
			update dbo.EntPropertyXRef
			set
				PropValue = cast(PropValue as bigint) + 1
			where
				EntID = @PostID
				and PropID = 14		--VIEW COUNT [14]


			declare @viewID bigint = (select ViewID from dbo.EntViewXRef where TargetEntID = @PostID and ActorEntID = @UserID)

			if(@viewID is null)
			begin

				insert into dbo.EntViewXRef
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
					@postEntType,			--POST ENT TYPE [6]
					@userEntType			--USER ENT TYPE [1]
				)

			end
			else begin
				update dbo.EntViewXRef
				set
					ViewCount = ViewCount + 1
				where
					ViewID = @viewID
			end


			insert into dbo.EntViewDateXRef
			(
				ViewID
			)
			values
			(
				@viewID
			)



			select cast(1 as bit)
		end
		else begin

			select cast(0 as bit)

		end

	end try
	begin catch

		select cast(0 as bit)

	end catch


end
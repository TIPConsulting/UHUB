CREATE proc [dbo].[Post_Delete]

	@PostID bigint,
	@DeletedBy bigint

as
begin

	begin try
		BEGIN TRAN

			declare @deletedDate datetimeoffset(7) = sysdatetimeoffset()
	

			--mark post as deleted
			update Entities
			set
				IsDeleted = 1,
				DeletedBy = @DeletedBy,
				DeletedDate = @deletedDate
			where
				ID = @PostID
				and EntTypeID = 6;		--POST TYPE [6]




			--Migrate all live property data to historical archive
			insert into dbo.EntPropertyRevisionXRef
			(
				EntID,
				EntTypeID,
				PropID,
				PropValue,
				CreatedBy,
				CreatedDate
			)
			select
				epx.EntID,
				epx.EntTypeID,
				epx.PropID,
				epx.PropValue,
				epx.CreatedBy,
				epx.CreatedDate
			from EntPropertyXRef epx
			where
				epx.EntID = @PostID;


			--delete property data from live system
			delete from EntPropertyXRef
			where
				EntID = @PostID;



			--find all child comments
			with recursiveFind as
			(
				select
					ChildEntID,
					ChildEntType,
					ParentEntID,
					ParentEntType
				from dbo.EntChildXRef ecx
				where
					ecx.ChildEntType = 7
					and ecx.ParentEntID = @PostID 
					and ecx.ParentEntType = 6 --POST TYPE [6]
					and ecx.IsDeleted = 0


				UNION ALL

	
				select
					ecx.ChildEntID,
					ecx.ChildEntType,
					ecx.ParentEntID,
					ecx.ParentEntType
				from dbo.EntChildXRef ecx

				inner join recursiveFind rf
				on
					ecx.ParentEntID = rf.ChildEntID
					and ecx.ParentEntType = 7 --COMMENT TYPE [6]

				where
					ecx.ChildEntType = 7
					and ecx.IsDeleted = 0
			)

			SELECT ChildEntID INTO #childCommentSet
			FROM recursiveFind


			--mark child comments as deleted
			update Entities
			set
				IsDeleted = 1,
				DeletedBy = @DeletedBy,
				DeletedDate = @deletedDate
			where
				ID in (select ChildEntID from #childCommentSet)



			--mark child relationships as deleted
			update EntChildXRef
			set
				IsDeleted = 1
			where
				ChildEntID = @PostID
				or ParentEntID = @PostID
				or ChildEntID in (select ChildEntID from #childCommentSet)
				or ParentEntID in (select ChildEntID from #childCommentSet)
	


			
			--Migrate all live property data to historical archive
			insert into dbo.EntPropertyRevisionXRef
			(
				EntID,
				EntTypeID,
				PropID,
				PropValue,
				CreatedBy,
				CreatedDate
			)
			select
				epx.EntID,
				epx.EntTypeID,
				epx.PropID,
				epx.PropValue,
				epx.CreatedBy,
				epx.CreatedDate
			from EntPropertyXRef epx
			where
				epx.EntID in (select ChildEntID from #childCommentSet)



			--delete property data from live system
			delete from EntPropertyXRef
			where
				EntID in (select ChildEntID from #childCommentSet)



			COMMIT TRAN

			drop table #childCommentSet

	end try
	begin catch
		if(@@TRANCOUNT > 0)
		begin
			ROLLBACK TRAN
		end

		;throw;

	end catch

end
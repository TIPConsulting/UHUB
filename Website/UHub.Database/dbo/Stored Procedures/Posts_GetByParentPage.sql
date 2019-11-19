

create proc [dbo].[Posts_GetByParentPage]

	@ParentID bigint,
	@StartID bigint null,
	@PageNum int null,
	@ItemCount smallint

as
begin

	declare @adjStartIdx bigint
	declare @adjEndIdx bigint

	--set StartID to the id of the newest ent
	if(@StartID is null)
	begin

		select top(1)
			@StartID = ID
		from dbo.vPosts
		order by
			CreatedDate desc
	end
	else begin

		--kill if start id does not exist as a post
		if((select EntTypeID from Entities where ID = @StartID) != 6)	--POST TYPE [6]
		begin
			return;

		end
	end



	--START ID CAN BE DERIVED AT CLIENT
	--STARTID = MAX(ID) WHEN PAGE=0


	--return all items if count is -1
	if(@ItemCount = -1)
	begin
		select * from dbo.vPosts
		return;
	end


	--ensure that ItemCount is at least 1
	if(@ItemCount < 1)
	begin
		set @ItemCount = 1
	end


	--set default page number
	if(@PageNum is null or @PageNum < 0)
	begin
		set @PageNum = 0
	end

	
	
	set @adjStartIdx = 1 + @PageNum * @ItemCount;
	set @adjEndIdx = @adjStartIdx + @ItemCount;


	--get set of posts starting at the 
	with postSet as
	(

		--only select top records necessary to fulfill the request
		select top(@adjEndIdx - 1)
			vu.ID,
			vu.IsEnabled,
			vu.IsReadonly,
			vu.[Name],
			vu.[Content],
			vu.[IsModified],
			vu.[ViewCount],
			vu.[IsLocked],
			vu.[CanComment],
			vu.[IsPublic],
			vu.ParentID,
			vu.[IsDeleted],
			vu.[CreatedBy],
			vu.[CreatedDate],
			vu.[ModifiedBy],
			vu.[ModifiedDate],
			vu.[DeletedBy],
			vu.[DeletedDate]
			,ROW_NUMBER() over (order by vu.CreatedDate desc) as RowNum
		from dbo.vPosts vu

		where 
			vu.ID <= @StartID
			and vu.ParentID = @ParentID

	)


	select
		ps.ID,
		ps.IsEnabled,
		ps.IsReadonly,
		ps.[Name],
		ps.[Content],
		ps.[IsModified],
		ps.[ViewCount],
		ps.[IsLocked],
		ps.[CanComment],
		ps.[IsPublic],
		ps.ParentID,
		ps.[IsDeleted],
		ps.[CreatedBy],
		ps.[CreatedDate],
		ps.[ModifiedBy],
		ps.[ModifiedDate],
		ps.[DeletedBy],
		ps.[DeletedDate]
	from postSet ps
	where
		RowNum >= @adjStartIdx



end
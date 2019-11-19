CREATE proc [dbo].[User_ValidateCommentParent]
	@UserID bigint,
	@ParentID bigint

as
begin


	declare @user_IsAdmin bit
	declare @user_IsDeleted bit
	select
		@user_IsAdmin = u.IsAdmin,
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
		OR @user_IsDeleted = 1
	)
	begin
		select cast (0 as bit)
		return;
	end


	--admins can post anywhere
	if(@user_IsAdmin = 1)
	begin
		select cast (1 as bit)
		return;
	end



	declare @schoolID bigint = null
	declare @schoolType smallint = 2;	--SCHOOL TYPE [1]

	declare @postID bigint = null
	declare @postType smallint = 6		--POST TYPE [6]

	declare @parent_Type smallint
	declare @parent_IsReadOnly bit
	declare @parent_IsEnabled bit
	declare @parent_IsDeleted bit

	select
		@parent_Type = p.EntTypeID,
		@parent_IsReadOnly = p.IsReadOnly,
		@parent_IsEnabled = p.IsEnabled,
		@parent_IsDeleted = p.IsDeleted
	from
		dbo.Entities p
	where
		p.ID = @ParentID



	--parent not found
	--parent readonly
	--parent isEnabled
	--parent isDeleted
	if(
		@parent_Type is null
		OR @parent_IsReadOnly = 1
		OR @parent_IsEnabled = 0
		OR @parent_IsDeleted = 1
	)
	begin
		select cast (0 as bit)
		return;
	end



	--if parent is already a school, then no recursion is necessary
	if(@parent_Type = @schoolType)
	begin
		
		set @schoolID = @ParentID

	end
	--if parent is not school, then we need to recusively look for the top-level school
	else begin
		with schoolSearch as
		(
			--anchor case
			--get parent/child Xref starting at ParentID
			--ensure that no parent is in an invalid state
			select 
				ecx1.ChildEntID,
				ecx1.ParentEntID,
				ecx1.ParentEntType
			from dbo.EntChildXRef ecx1
			inner join dbo.Entities entP
			on
				entP.ID = ecx1.ParentEntID
				and entP.IsReadOnly = 0
				and entP.IsEnabled = 1
				and entP.IsDeleted = 0
			where
				ecx1.ChildEntID = @parentID
				AND ecx1.IsDeleted = 0

			--recursive lookup
			--traverse up the parent tree to find the topmost member
			--the top member is the associated school
			UNION ALL
			select 
				ecx2.ChildEntID,
				ecx2.ParentEntID,
				ecx2.ParentEntType
			from dbo.EntChildXRef ecx2
			inner join dbo.Entities entP
			on
				entP.ID = ecx2.ParentEntID
				and entP.IsReadOnly = 0
				and entP.IsEnabled = 1
				and entP.IsDeleted = 0
			inner join schoolSearch rs
			on
				rs.ParentEntID = ecx2.ChildEntID
			where
				ecx2.IsDeleted = 0

		)


		select *
		into #parentage
		from schoolSearch


		select
			@schoolID = rs.ParentEntID
		from #parentage rs
		where
			ParentEntType = @schoolType


		select
			@postID = rs.ParentEntID
		from #parentage rs
		where
			ParentEntType = @postType


		drop table #parentage

	end


	--school not found
	--or some level between insert/TLS is deleted/readonly/not enabled
	if(@schoolID is null)
	begin
		select cast (0 as bit)
		return;
	end


	declare @post_Parent bigint
	select
		@post_Parent = ParentEntID
	from dbo.EntChildXRef
	where
		ChildEntID = @postID


	declare @isUserBanned bit

	exec @isUserBanned = [dbo].[SchoolClub_IsUserBanned] 
		@ClubID = @post_Parent,
		@UserID = @UserID


	if(@isUserBanned = 1)
	begin
		select cast (0 as bit)
		return;
	end


	declare @post_IsLocked bit
	declare @post_EnableComments bit


	select
		@post_IsLocked = cast(epx.PropValue as bit)
	from dbo.EntPropertyXRef epx
	where
		epx.EntID = @postID
		AND epx.PropID = 33		--IS LOCKED [33]


	select
		@post_EnableComments = cast(epx.PropValue as bit)
	from dbo.EntPropertyXRef epx
	where
		epx.EntID = @postID
		AND epx.PropID = 34		--ENABLE COMMENTS [34]



	if(
		@post_IsLocked is null
		OR @post_IsLocked = 1
		OR @post_EnableComments is null
		OR @post_EnableComments = 0)
	begin
		select cast (0 as bit)
		return;
	end



	--ensure that the specified user is associated with the top-level school
	if(exists (select * from dbo.EntChildXRef where ParentEntID = @schoolID and ChildEntID = @UserID))
	begin
		select cast (1 as bit)
	end
	else begin
		select cast (0 as bit)
	end
	


end
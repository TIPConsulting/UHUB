CREATE proc [dbo].[Users_PurgeUnconfirmed]

	--users created before this date are subject to be purged
	--users created afgter this date will be ignored
	@MinKeepDate DateTimeOffset 

as
begin

	

	select
		u.[EntID]
	into #userTemp
	from
		dbo.Users u
	inner join dbo.Entities e
	on
		u.EntID = e.ID
	where
		e.CreatedDate < @MinKeepDate
		AND u.IsConfirmed = 0
	

	declare @userID bigint
	declare cur CURSOR LOCAL for
		select [EntID] from #userTemp
	open cur

	fetch next from cur into @userID

	while @@FETCH_STATUS = 0 BEGIN

		--execute your sproc on each row
		exec [dbo].[User_PurgeByID] @userID

		fetch next from cur into @userID
	END

	close cur
	deallocate cur
	
	drop table #userTemp
end
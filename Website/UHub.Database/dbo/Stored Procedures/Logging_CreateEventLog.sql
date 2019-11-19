CREATE proc [dbo].[Logging_CreateEventLog]


	@EventTypeID smallint,
	@EventID nvarchar(100),
	@Content nvarchar(max),
	@CreatedBy bigint null,
	@CreatedDate datetimeoffset(7) null = null

as
begin


	if(@CreatedBy is null)
	begin
		set @CreatedBy = 0
	end


	if(@CreatedDate is null)
	begin
		insert into dbo.EventLog
		(
			EventTypeID,
			EventID,
			[Content],
			CreatedBy
		)
		values
		(
			@EventTypeID,
			@EventID,
			@Content,
			@CreatedBy
		)
	end
	else begin
		insert into dbo.EventLog
		(
			EventTypeID,
			EventID,
			[Content],
			CreatedBy,
			CreatedDate
		)
		values
		(
			@EventTypeID,
			@EventID,
			@Content,
			@CreatedBy,
			@CreatedDate
		)
	end



end

create procedure [dbo].[User_CreateTokenValidator]

	@IssueDate datetimeoffset,
	@MaxExpirationDate datetimeoffset,
	@TokenID char(3),
	@IsPersistent bit,
	@TokenHash nvarchar(200),
	@RequestID nvarchar(50),
	@SessionID nvarchar(50)


as



--------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------


	insert into dbo.TokenValidators
	(
		IssueDate,
		MaxExpirationDate,
		TokenID,
		IsPersistent,
		TokenHash,
		RequestID,
		SessionID
	)
	values
	(
		@IssueDate,
		@MaxExpirationDate,
		@TokenID,
		@IsPersistent,
		@TokenHash,
		@RequestID,
		@SessionID
	)
		



--------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------



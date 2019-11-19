create proc [dbo].[Users_PurgeExpiredTokenValidators]
as
begin

	declare @now datetimeoffset(7) = SYSDATETIMEOFFSET()


	delete from
		dbo.TokenValidators
	where
		MaxExpirationDate < @now

end
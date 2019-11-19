CREATE proc [dbo].[User_GetIDByUsername]

	@Username nvarchar(100),
	@Domain nvarchar(250)

as
begin

	select
		entID
	from dbo.Users
	where
		Username = @Username
		AND Domain = @Domain


end
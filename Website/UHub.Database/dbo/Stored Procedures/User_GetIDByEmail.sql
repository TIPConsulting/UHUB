create proc User_GetIDByEmail

	@Email nvarchar(250)

as
begin

	select
		entID
	from dbo.Users
	where
		Email = @Email
	


end
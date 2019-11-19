create proc [dbo].[User_DoesExistByEmail]
	@Email nvarchar(250)


as
begin

	if exists(select * from dbo.Users where Email = @Email)
	begin
		select cast(1 as bit)
	end
	else begin
		select cast(0 as bit)
	end



end
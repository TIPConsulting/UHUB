CREATE proc [dbo].[User_DoesExistByID]
	@UserID bigint


as
begin

	if exists(select * from dbo.Users where EntID = @UserID)
	begin
		select cast(1 as bit)
	end
	else begin
		select cast(0 as bit)
	end



end
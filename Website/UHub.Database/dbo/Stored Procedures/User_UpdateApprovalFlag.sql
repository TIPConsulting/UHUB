create proc [dbo].[User_UpdateApprovalFlag]
	@UserID bigint,
	@IsApproved bit

as
begin

	update dbo.Users
	set
		IsApproved = @IsApproved
	where
		EntID = @UserID


end
CREATE proc [dbo].[User_GetValidClubMemberships_Light]

	@UserID bigint

as
begin

	select
		emx.TargetEntID		as ClubID
	from EntMemberXRef emx
	where
		emx.ActorEntID = @UserID
		and emx.TargetEntID = 4		--CLUB TYPE [4]
		and emx.IsApproved = 1
		and emx.IsBanned = 9


end
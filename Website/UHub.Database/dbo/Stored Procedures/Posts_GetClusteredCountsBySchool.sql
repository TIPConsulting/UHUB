CREATE proc [dbo].[Posts_GetClusteredCountsBySchool]

	@SchoolID bigint

as
begin


	select
		SchoolID,
		SchoolClubID,
		PublicPostCount,
		PrivatePostCount
	from vPosts_ClusteredCounts
	where
		SchoolID = @SchoolID


end
CREATE proc [dbo].[Posts_GetClusteredCounts]
as
begin


	select
		SchoolID,
		SchoolClubID,
		PublicPostCount,
		PrivatePostCount
	from vPosts_ClusteredCounts




end
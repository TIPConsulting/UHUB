












CREATE proc [dbo].[Users_GetPostCommenters]

	@PostID bigint

as
begin

with recursiveFind as
(

	select
		vu.ID,
		vu.[CreatedBy]
	from dbo.vComments vu

	inner JOIN dbo.EntChildXRef
	ON
		ChildEntID = vu.ID
		and ChildEntType = 7
		and ParentEntID = @PostID 
		and ParentEntType = 6 --POST TYPE [6]


	UNION ALL

	
	select
		vu.ID,
		vu.[CreatedBy]
	from dbo.vComments vu

	inner JOIN dbo.EntChildXRef ecx
	ON
		ChildEntID = vu.ID
		and ChildEntType = 7

	inner join recursiveFind rf
	on
		ecx.ParentEntID = rf.ID
		and ParentEntType = 7 --COMMENT TYPE [6]
),
userSet as
(
	select 
		distinct CreatedBy
	from recursiveFind
)



select

	vu.ID,
	vu.IsEnabled,
	vu.IsReadonly,
	vu.Email,
	vu.Username,
	vu.IsConfirmed,
	vu.IsApproved,
	vu.[Version],
	vu.[IsAdmin],
	vu.[Name],
	vu.[PhoneNumber],
	vu.[Major],
	vu.[Year],
	vu.ExpectedGradDate,
	vu.[Company],
	vu.[JobTitle],
	vu.[IsFinished],
	vu.[ParentID],
	vu.[IsDeleted],
	vu.[CreatedBy],
	vu.[CreatedDate],
	vu.[ModifiedBy],
	vu.[ModifiedDate],
	vu.[DeletedBy],
	vu.[DeletedDate]

from userSet us

inner join vUsers vu
on
	us.CreatedBy = vu.ID




end
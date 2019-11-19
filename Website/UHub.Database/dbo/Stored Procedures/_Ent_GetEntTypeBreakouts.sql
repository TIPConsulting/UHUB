CREATE proc [dbo].[_Ent_GetEntTypeBreakouts]
as
begin


	select
		EntTypeID,
		TableSchema,
		TableName,
		JoinType,
		OverrideComparer,
		ColumnWhiteList,
		ColumnBlackList
	from EntityTypeBreakoutXRef


end
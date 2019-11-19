CREATE proc [dbo].[_Ent_GetEntTypeBreakoutsByEntType]

	@EntTypeID smallint

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

	where 
		EntTypeID = @EntTypeID

end
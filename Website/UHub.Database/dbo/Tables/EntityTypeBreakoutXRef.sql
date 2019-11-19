CREATE TABLE [dbo].[EntityTypeBreakoutXRef] (
    [EntTypeID]        SMALLINT       NOT NULL,
    [TableSchema]      NVARCHAR (50)  NULL,
    [TableName]        NVARCHAR (100) NOT NULL,
    [JoinType]         NVARCHAR (50)  NULL,
    [OverrideComparer] NVARCHAR (250) NULL,
    [ColumnWhiteList]  NVARCHAR (MAX) NULL,
    [ColumnBlackList]  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_EntityTypeBreakoutXRef] PRIMARY KEY CLUSTERED ([EntTypeID] ASC, [TableName] ASC),
    CONSTRAINT [FK_EntityTypeBreakoutXRef_EntityTypes] FOREIGN KEY ([EntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID])
);


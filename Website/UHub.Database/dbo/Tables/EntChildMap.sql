CREATE TABLE [dbo].[EntChildMap] (
    [ParentEntType] SMALLINT       NOT NULL,
    [ChildEntType]  SMALLINT       NOT NULL,
    [Description]   NVARCHAR (250) NULL,
    CONSTRAINT [PK_EntChildMap] PRIMARY KEY CLUSTERED ([ParentEntType] ASC, [ChildEntType] ASC),
    CONSTRAINT [FK_EntChildMap_EntityTypes] FOREIGN KEY ([ParentEntType]) REFERENCES [dbo].[EntityTypes] ([ID]),
    CONSTRAINT [FK_EntChildMap_EntityTypes1] FOREIGN KEY ([ChildEntType]) REFERENCES [dbo].[EntityTypes] ([ID])
);


CREATE TABLE [dbo].[EntChildXRef] (
    [ParentEntID]   BIGINT   NOT NULL,
    [ChildEntID]    BIGINT   NOT NULL,
    [ParentEntType] SMALLINT NOT NULL,
    [ChildEntType]  SMALLINT NOT NULL,
    [IsDeleted]     BIT      CONSTRAINT [DF_EntChildXRef_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_EntChildXRef] PRIMARY KEY CLUSTERED ([ParentEntID] ASC, [ChildEntID] ASC),
    CONSTRAINT [FK_EntChildXRef_EntChildMap] FOREIGN KEY ([ParentEntType], [ChildEntType]) REFERENCES [dbo].[EntChildMap] ([ParentEntType], [ChildEntType]),
    CONSTRAINT [FK_EntChildXRef_Entities] FOREIGN KEY ([ParentEntID], [ParentEntType]) REFERENCES [dbo].[Entities] ([ID], [EntTypeID]),
    CONSTRAINT [FK_EntChildXRef_Entities1] FOREIGN KEY ([ChildEntID], [ChildEntType]) REFERENCES [dbo].[Entities] ([ID], [EntTypeID])
);










GO
CREATE NONCLUSTERED INDEX [IX_EntChildXRef_ChildID]
    ON [dbo].[EntChildXRef]([ChildEntID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntChildXRef_ParentID]
    ON [dbo].[EntChildXRef]([ParentEntID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntChildXRef_Parent_ChildType]
    ON [dbo].[EntChildXRef]([ParentEntID] ASC, [ChildEntType] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntChildXRef_Child_ParentType]
    ON [dbo].[EntChildXRef]([ChildEntID] ASC, [ParentEntType] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntChildXRef_ParentType]
    ON [dbo].[EntChildXRef]([ParentEntType] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntChildXRef_Parent_ChildID]
    ON [dbo].[EntChildXRef]([ParentEntID] ASC, [ChildEntID] ASC, [ParentEntType] ASC);


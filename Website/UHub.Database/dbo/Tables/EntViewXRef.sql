CREATE TABLE [dbo].[EntViewXRef] (
    [ViewID]          BIGINT   IDENTITY (1, 1) NOT NULL,
    [TargetEntID]     BIGINT   NOT NULL,
    [ActorEntID]      BIGINT   NOT NULL,
    [TargetEntTypeID] SMALLINT NOT NULL,
    [ActorEntTypeID]  SMALLINT NOT NULL,
    [ViewCount]       BIGINT   CONSTRAINT [DF_EntViewXRef_ViewCount] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_EntViewXRef] PRIMARY KEY CLUSTERED ([ViewID] ASC),
    CONSTRAINT [FK_EntViewXRef_Entities] FOREIGN KEY ([TargetEntID], [TargetEntTypeID]) REFERENCES [dbo].[Entities] ([ID], [EntTypeID]),
    CONSTRAINT [FK_EntViewXRef_Entities1] FOREIGN KEY ([ActorEntID], [ActorEntTypeID]) REFERENCES [dbo].[Entities] ([ID], [EntTypeID]),
    CONSTRAINT [FK_EntViewXRef_EntViewMap] FOREIGN KEY ([TargetEntTypeID], [ActorEntTypeID]) REFERENCES [dbo].[EntViewMap] ([TargetEntTypeID], [ActorEntTypeID])
);




GO
CREATE NONCLUSTERED INDEX [IX_EntViewXRef_TargetID]
    ON [dbo].[EntViewXRef]([TargetEntID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntViewXRef_ActorID]
    ON [dbo].[EntViewXRef]([ActorEntID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntViewXRef]
    ON [dbo].[EntViewXRef]([ActorEntID] ASC, [TargetEntID] ASC);


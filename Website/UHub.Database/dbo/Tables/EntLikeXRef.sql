CREATE TABLE [dbo].[EntLikeXRef] (
    [TargetEntID]     BIGINT             NOT NULL,
    [ActorEntID]      BIGINT             NOT NULL,
    [TargetEntTypeID] SMALLINT           NOT NULL,
    [ActorEntTypeID]  SMALLINT           NOT NULL,
    [CreatedDate]     DATETIMEOFFSET (6) CONSTRAINT [DF_EntLikeXRef_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    CONSTRAINT [PK_EntLikeXRef] PRIMARY KEY CLUSTERED ([TargetEntID] ASC, [ActorEntID] ASC),
    CONSTRAINT [FK_EntLikeXRef_Entities] FOREIGN KEY ([TargetEntID], [TargetEntTypeID]) REFERENCES [dbo].[Entities] ([ID], [EntTypeID]),
    CONSTRAINT [FK_EntLikeXRef_Entities1] FOREIGN KEY ([ActorEntID], [ActorEntTypeID]) REFERENCES [dbo].[Entities] ([ID], [EntTypeID]),
    CONSTRAINT [FK_EntLikeXRef_EntLikeMap] FOREIGN KEY ([TargetEntTypeID], [ActorEntTypeID]) REFERENCES [dbo].[EntLikeMap] ([TargetEntTypeID], [ActorEntTypeID])
);






GO
CREATE NONCLUSTERED INDEX [IX_EntLikeXRef_TargetID]
    ON [dbo].[EntLikeXRef]([TargetEntID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntLikeXRef_ActorID]
    ON [dbo].[EntLikeXRef]([ActorEntID] ASC);


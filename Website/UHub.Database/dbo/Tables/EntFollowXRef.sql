CREATE TABLE [dbo].[EntFollowXRef] (
    [TargetEntID]     BIGINT             NOT NULL,
    [ActorEntID]      BIGINT             NOT NULL,
    [TargetEntTypeID] SMALLINT           NOT NULL,
    [ActorEntTypeID]  SMALLINT           NOT NULL,
    [CreatedDate]     DATETIMEOFFSET (6) CONSTRAINT [DF_EntFollowXRef_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    CONSTRAINT [PK_EntFollowXRef] PRIMARY KEY CLUSTERED ([TargetEntID] ASC, [ActorEntID] ASC),
    CONSTRAINT [FK_EntFollowXRef_EntFollowMap] FOREIGN KEY ([TargetEntTypeID], [ActorEntTypeID]) REFERENCES [dbo].[EntFollowMap] ([TargetEntTypeID], [ActorEntTypeID]),
    CONSTRAINT [FK_EntFollowXRef_Entities] FOREIGN KEY ([TargetEntID], [TargetEntTypeID]) REFERENCES [dbo].[Entities] ([ID], [EntTypeID]),
    CONSTRAINT [FK_EntFollowXRef_Entities1] FOREIGN KEY ([ActorEntID], [ActorEntTypeID]) REFERENCES [dbo].[Entities] ([ID], [EntTypeID])
);






GO
CREATE NONCLUSTERED INDEX [IX_EntFollowXRef_TargetID]
    ON [dbo].[EntFollowXRef]([TargetEntID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntFollowXRef_ActorID]
    ON [dbo].[EntFollowXRef]([ActorEntID] ASC);


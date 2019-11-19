CREATE TABLE [dbo].[EntFollowMap] (
    [TargetEntTypeID] SMALLINT       NOT NULL,
    [ActorEntTypeID]  SMALLINT       NOT NULL,
    [Description]     NVARCHAR (250) NULL,
    CONSTRAINT [PK_EntFollowMap] PRIMARY KEY CLUSTERED ([TargetEntTypeID] ASC, [ActorEntTypeID] ASC),
    CONSTRAINT [FK_EntFollowMap_EntityTypes] FOREIGN KEY ([TargetEntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID]),
    CONSTRAINT [FK_EntFollowMap_EntityTypes1] FOREIGN KEY ([ActorEntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID])
);


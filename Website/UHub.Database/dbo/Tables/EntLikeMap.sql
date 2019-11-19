CREATE TABLE [dbo].[EntLikeMap] (
    [TargetEntTypeID] SMALLINT       NOT NULL,
    [ActorEntTypeID]  SMALLINT       NOT NULL,
    [Description]     NVARCHAR (250) NULL,
    CONSTRAINT [PK_EntLikeMap] PRIMARY KEY CLUSTERED ([TargetEntTypeID] ASC, [ActorEntTypeID] ASC),
    CONSTRAINT [FK_EntLikeMap_EntityTypes] FOREIGN KEY ([TargetEntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID]),
    CONSTRAINT [FK_EntLikeMap_EntityTypes1] FOREIGN KEY ([ActorEntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID])
);


CREATE TABLE [dbo].[EntViewMap] (
    [TargetEntTypeID] SMALLINT       NOT NULL,
    [ActorEntTypeID]  SMALLINT       NOT NULL,
    [Description]     NVARCHAR (250) NULL,
    CONSTRAINT [PK_EntViewMap] PRIMARY KEY CLUSTERED ([TargetEntTypeID] ASC, [ActorEntTypeID] ASC),
    CONSTRAINT [FK_EntViewMap_EntityTypes] FOREIGN KEY ([TargetEntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID]),
    CONSTRAINT [FK_EntViewMap_EntityTypes1] FOREIGN KEY ([ActorEntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID])
);


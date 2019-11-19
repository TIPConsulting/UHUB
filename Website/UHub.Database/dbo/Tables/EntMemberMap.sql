CREATE TABLE [dbo].[EntMemberMap] (
    [TargetEntTypeID] SMALLINT       NOT NULL,
    [ActorEntTypeID]  SMALLINT       NOT NULL,
    [Description]     NVARCHAR (250) NOT NULL,
    CONSTRAINT [PK_EntMemeberMap] PRIMARY KEY CLUSTERED ([TargetEntTypeID] ASC, [ActorEntTypeID] ASC),
    CONSTRAINT [FK_EntMemberMap_EntityTypes] FOREIGN KEY ([TargetEntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID]),
    CONSTRAINT [FK_EntMemberMap_EntityTypes1] FOREIGN KEY ([ActorEntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID])
);


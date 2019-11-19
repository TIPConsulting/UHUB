CREATE TABLE [dbo].[EntTagMap] (
    [TargetEntTypeID] SMALLINT       NOT NULL,
    [Description]     NVARCHAR (250) NULL,
    CONSTRAINT [PK_EntTagMap] PRIMARY KEY CLUSTERED ([TargetEntTypeID] ASC),
    CONSTRAINT [FK_EntTagMap_EntityTypes] FOREIGN KEY ([TargetEntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID])
);


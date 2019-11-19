CREATE TABLE [dbo].[EntPropertyRevisionMap] (
    [EntTypeID]   SMALLINT       NOT NULL,
    [PropID]      INT            NOT NULL,
    [Description] NVARCHAR (250) NULL,
    CONSTRAINT [PK_EntPropertyRevisionMAp] PRIMARY KEY CLUSTERED ([EntTypeID] ASC, [PropID] ASC),
    CONSTRAINT [FK_EntPropertyRevisionMap_EntityTypes] FOREIGN KEY ([EntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID]),
    CONSTRAINT [FK_EntPropertyRevisionMap_EntProperties] FOREIGN KEY ([PropID]) REFERENCES [dbo].[EntProperties] ([ID]),
    CONSTRAINT [FK_EntPropertyRevisionMap_EntPropertyMap] FOREIGN KEY ([EntTypeID], [PropID]) REFERENCES [dbo].[EntPropertyMap] ([EntTypeID], [PropID])
);


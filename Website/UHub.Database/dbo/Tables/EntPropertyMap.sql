CREATE TABLE [dbo].[EntPropertyMap] (
    [EntTypeID]         SMALLINT       NOT NULL,
    [PropID]            INT            NOT NULL,
    [DataTypeID]        SMALLINT       NOT NULL,
    [OverrideLength]    INT            NULL,
    [OverridePrecision] NCHAR (10)     NULL,
    [IsNullable]        BIT            NOT NULL,
    [DefaultValue]      NVARCHAR (MAX) NULL,
    [Description]       NVARCHAR (250) NULL,
    CONSTRAINT [PK_EntPropertyMap] PRIMARY KEY CLUSTERED ([EntTypeID] ASC, [PropID] ASC),
    CONSTRAINT [FK_EntPropertyMap_EntityTypes] FOREIGN KEY ([EntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID]),
    CONSTRAINT [FK_EntPropertyMap_EntProperties] FOREIGN KEY ([PropID], [DataTypeID]) REFERENCES [dbo].[EntProperties] ([ID], [DataTypeID])
);




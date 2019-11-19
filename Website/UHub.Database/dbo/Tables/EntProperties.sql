CREATE TABLE [dbo].[EntProperties] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [PropName]         VARCHAR (100)  NOT NULL,
    [PropFriendlyName] NVARCHAR (100) NOT NULL,
    [Description]      NVARCHAR (250) NULL,
    [DataTypeID]       SMALLINT       NOT NULL,
    [DataType]         VARCHAR (100)  NOT NULL,
    [DefaultLength]    INT            NULL,
    [DefaultPrecision] NCHAR (10)     NULL,
    CONSTRAINT [PK_EntProperties] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_EntProperties_DataTypes] FOREIGN KEY ([DataTypeID], [DataType]) REFERENCES [dbo].[DataTypes] ([ID], [Name])
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EntProperties]
    ON [dbo].[EntProperties]([ID] ASC, [DataTypeID] ASC);


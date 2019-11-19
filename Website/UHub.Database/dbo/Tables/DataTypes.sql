CREATE TABLE [dbo].[DataTypes] (
    [ID]               SMALLINT      NOT NULL,
    [Name]             VARCHAR (100) NOT NULL,
    [DefaultLength]    INT           NULL,
    [DefaultPrecision] NCHAR (10)    NULL,
    CONSTRAINT [PK_DataTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_DataTypes_Name]
    ON [dbo].[DataTypes]([Name] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_DataTypes]
    ON [dbo].[DataTypes]([ID] ASC, [Name] ASC);


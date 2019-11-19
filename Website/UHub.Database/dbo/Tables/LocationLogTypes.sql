CREATE TABLE [dbo].[LocationLogTypes] (
    [ID]          TINYINT        IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (250) NULL,
    [Description] NVARCHAR (250) NULL,
    CONSTRAINT [PK_LocationLogTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);


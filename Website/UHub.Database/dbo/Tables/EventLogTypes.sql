CREATE TABLE [dbo].[EventLogTypes] (
    [ID]          SMALLINT       NOT NULL,
    [Name]        NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR (250) NULL,
    CONSTRAINT [PK_EventLogTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);


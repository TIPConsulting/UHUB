CREATE TABLE [dbo].[ActivityLogTypes] (
    [ID]          SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (250) NULL,
    [Description] NVARCHAR (250) NULL,
    CONSTRAINT [PK_ActivityLogTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);


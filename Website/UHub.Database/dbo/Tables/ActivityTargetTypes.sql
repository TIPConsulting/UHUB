CREATE TABLE [dbo].[ActivityTargetTypes] (
    [ID]          SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (250) NULL,
    [Descritpion] NVARCHAR (250) NULL,
    CONSTRAINT [PK_ActivityTargetTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);


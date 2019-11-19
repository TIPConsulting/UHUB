CREATE TABLE [dbo].[UserClasses] (
    [ID]          TINYINT        IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (250) NOT NULL,
    [Description] NVARCHAR (250) NULL,
    [IsEnabled]   BIT            CONSTRAINT [DF_UserClasses_IsEnabled] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_UserClasses] PRIMARY KEY CLUSTERED ([ID] ASC)
);


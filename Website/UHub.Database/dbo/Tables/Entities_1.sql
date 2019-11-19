CREATE TABLE [dbo].[Entities] (
    [ID]           BIGINT             IDENTITY (1, 1) NOT NULL,
    [UID]          UNIQUEIDENTIFIER   CONSTRAINT [DF_Entities_UID] DEFAULT (newsequentialid()) NOT NULL,
    [EntTypeID]    SMALLINT           NOT NULL,
    [IsEnabled]    BIT                CONSTRAINT [DF_Entities_IsEnabled] DEFAULT ((1)) NOT NULL,
    [IsReadOnly]   BIT                CONSTRAINT [DF_Entities_IsReadOnly] DEFAULT ((0)) NOT NULL,
    [IsDeleted]    BIT                CONSTRAINT [DF_Entities_IsDeleted] DEFAULT ((0)) NOT NULL,
    [CreatedDate]  DATETIMEOFFSET (6) CONSTRAINT [DF_Entities_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]    BIGINT             NOT NULL,
    [ModifiedDate] DATETIMEOFFSET (6) NULL,
    [ModifiedBy]   BIGINT             NULL,
    [DeletedDate]  DATETIMEOFFSET (6) NULL,
    [DeletedBy]    BIGINT             NULL,
    CONSTRAINT [PK_Entities] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Entities_EntityTypes] FOREIGN KEY ([EntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID]),
    CONSTRAINT [FK_Entities_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([EntID]),
    CONSTRAINT [FK_Entities_Users1] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Users] ([EntID]),
    CONSTRAINT [FK_Entities_Users2] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[Users] ([EntID])
);








GO
CREATE NONCLUSTERED INDEX [IX_Entities_EntType]
    ON [dbo].[Entities]([EntTypeID] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Entities]
    ON [dbo].[Entities]([ID] ASC, [EntTypeID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Entities_Deleted3]
    ON [dbo].[Entities]([IsDeleted] ASC, [ID] ASC, [EntTypeID] ASC, [IsEnabled] ASC, [IsReadOnly] ASC, [CreatedDate] ASC, [CreatedBy] ASC, [ModifiedDate] ASC, [ModifiedBy] ASC, [DeletedBy] ASC, [DeletedDate] ASC);




GO
CREATE NONCLUSTERED INDEX [IX_Entities_Deleted2]
    ON [dbo].[Entities]([IsDeleted] ASC, [ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Entities_Deleted]
    ON [dbo].[Entities]([IsDeleted] ASC);


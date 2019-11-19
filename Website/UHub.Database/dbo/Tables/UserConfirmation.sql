CREATE TABLE [dbo].[UserConfirmation] (
    [UserID]        BIGINT             NOT NULL,
    [RefUID]        NVARCHAR (100)     CONSTRAINT [DF_UserConfirmation_RefUID] DEFAULT (replace(concat(newid(),newid(),newid()),'-','')) NOT NULL,
    [CreatedDate]   DATETIMEOFFSET (7) CONSTRAINT [DF_UserConfirmation_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [ConfirmedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_UserConfirmation_ConfirmedDate] DEFAULT (NULL) NULL,
    [IsConfirmed]   BIT                CONSTRAINT [DF_UserConfirmation_Expired] DEFAULT ((0)) NOT NULL,
    [IsDeleted]     BIT                CONSTRAINT [DF_UserConfirmation_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_UserConfirmation] PRIMARY KEY NONCLUSTERED ([UserID] ASC, [RefUID] ASC),
    CONSTRAINT [FK_UserConfirmation_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([EntID])
);




GO
CREATE NONCLUSTERED INDEX [IX_CreatedDate]
    ON [dbo].[UserConfirmation]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RefUID]
    ON [dbo].[UserConfirmation]([RefUID] ASC);


GO
CREATE CLUSTERED INDEX [IX_UserID]
    ON [dbo].[UserConfirmation]([UserID] ASC);


CREATE TABLE [dbo].[UserRecovery] (
    [UserID]       BIGINT             NOT NULL,
    [RecoveryID]   NVARCHAR (100)     CONSTRAINT [DF_UserRecovery_RecoveryID] DEFAULT (replace(concat(newid(),newid(),newid()),'-','')) NOT NULL,
    [RecoveryKey]  NVARCHAR (200)     NOT NULL,
    [EffFromDate]  DATETIMEOFFSET (7) CONSTRAINT [DF_UserRecovery_EffFromDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [EffToDate]    DATETIMEOFFSET (7) NOT NULL,
    [IsEnabled]    BIT                CONSTRAINT [DF_UserRecovery_IsEnabled] DEFAULT ((1)) NOT NULL,
    [AttemptCount] TINYINT            CONSTRAINT [DF_UserRecovery_AttemptCount] DEFAULT ((0)) NOT NULL,
    [IsOptional]   BIT                CONSTRAINT [DF_UserRecovery_IsOptional] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_UserRecovery] PRIMARY KEY CLUSTERED ([RecoveryID] ASC),
    CONSTRAINT [FK_UserRecovery_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([EntID])
);






GO
CREATE NONCLUSTERED INDEX [IX_UserRecovery_UserID]
    ON [dbo].[UserRecovery]([UserID] ASC);


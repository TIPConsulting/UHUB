CREATE TABLE [dbo].[UserAuthentication] (
    [UserID]                 BIGINT             NOT NULL,
    [PswdHash]               NVARCHAR (250)     NOT NULL,
    [Salt]                   NVARCHAR (100)     NULL,
    [PswdCreatedDate]        DATETIMEOFFSET (7) CONSTRAINT [DF_UserAuthentication_PswdCreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [PswdModifiedDate]       DATETIMEOFFSET (7) CONSTRAINT [DF_UserAuthentication_PswdModifiedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [LastLockoutDate]        DATETIMEOFFSET (7) NULL,
    [StartFailedPswdWindow]  DATETIMEOFFSET (7) NULL,
    [FailedPswdAttemptCount] TINYINT            CONSTRAINT [DF_UserAuthentication_FailedPswdAttemptCount] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_UserAuthentication] PRIMARY KEY CLUSTERED ([UserID] ASC),
    CONSTRAINT [FK_UserAuthentication_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([EntID])
);




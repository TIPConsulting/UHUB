CREATE TABLE [dbo].[Users] (
    [EntID]       BIGINT         NOT NULL,
    [Email]       NVARCHAR (250) NOT NULL,
    [Domain]      NVARCHAR (250) NULL,
    [Username]    NVARCHAR (100) NOT NULL,
    [IsConfirmed] BIT            CONSTRAINT [DF_Users_IsConfirmed] DEFAULT ((0)) NOT NULL,
    [IsApproved]  BIT            CONSTRAINT [DF_Users_IsApproved] DEFAULT ((0)) NOT NULL,
    [Version]     NVARCHAR (20)  NOT NULL,
    [IsAdmin]     BIT            CONSTRAINT [DF_Users_IsAdmin] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([EntID] ASC),
    CONSTRAINT [FK_Users_Entities] FOREIGN KEY ([EntID]) REFERENCES [dbo].[Entities] ([ID])
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Uname_Domain]
    ON [dbo].[Users]([Username] ASC, [Domain] ASC);


GO



GO
CREATE NONCLUSTERED INDEX [IX_Users_Username]
    ON [dbo].[Users]([Username] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email]
    ON [dbo].[Users]([Email] ASC);


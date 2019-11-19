CREATE TABLE [dbo].[UserClassXRef] (
    [UserID]      BIGINT  NOT NULL,
    [UserClassID] TINYINT NOT NULL,
    CONSTRAINT [PK_UserClassXRef] PRIMARY KEY CLUSTERED ([UserID] ASC, [UserClassID] ASC),
    CONSTRAINT [FK_UserClassXRef_UserClasses] FOREIGN KEY ([UserClassID]) REFERENCES [dbo].[UserClasses] ([ID]),
    CONSTRAINT [FK_UserClassXRef_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([EntID])
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserClassXRef]
    ON [dbo].[UserClassXRef]([UserID] ASC);


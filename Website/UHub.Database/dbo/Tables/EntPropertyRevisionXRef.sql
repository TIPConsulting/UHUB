CREATE TABLE [dbo].[EntPropertyRevisionXRef] (
    [EntID]          BIGINT             NOT NULL,
    [EntTypeID]      SMALLINT           NOT NULL,
    [PropID]         INT                NOT NULL,
    [PropValue]      NVARCHAR (MAX)     NULL,
    [CreatedBy]      BIGINT             NOT NULL,
    [CreatedDate]    DATETIMEOFFSET (6) CONSTRAINT [DF_EntPropertyRevisionXRef_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [DeprecatedDate] DATETIMEOFFSET (6) NULL,
    CONSTRAINT [FK_EntPropertyRevisionXRef_Entities] FOREIGN KEY ([EntID], [EntTypeID]) REFERENCES [dbo].[Entities] ([ID], [EntTypeID]),
    CONSTRAINT [FK_EntPropertyRevisionXRef_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([EntID])
);












GO
CREATE NONCLUSTERED INDEX [IX_EntPropertyRevisionXRef]
    ON [dbo].[EntPropertyRevisionXRef]([EntTypeID] ASC, [PropID] ASC);






GO
CREATE CLUSTERED INDEX [IX_EntPropertyRevisionXRef_Cluster]
    ON [dbo].[EntPropertyRevisionXRef]([EntID] ASC, [CreatedDate] ASC, [PropID] ASC);


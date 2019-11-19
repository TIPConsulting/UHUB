CREATE TABLE [dbo].[EntPropertyXRef] (
    [EntID]        BIGINT             NOT NULL,
    [EntTypeID]    SMALLINT           NOT NULL,
    [PropID]       INT                NOT NULL,
    [PropValue]    NVARCHAR (MAX)     NULL,
    [CreatedBy]    BIGINT             NOT NULL,
    [CreatedDate]  DATETIMEOFFSET (6) CONSTRAINT [DF_EntPropertyXRef_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [ModifiedBy]   BIGINT             NULL,
    [ModifiedDate] DATETIMEOFFSET (6) NULL,
    CONSTRAINT [PK_EntPropertyXRef] PRIMARY KEY CLUSTERED ([EntID] ASC, [PropID] ASC),
    CONSTRAINT [FK_EntPropertyXRef_Entities] FOREIGN KEY ([EntID], [EntTypeID]) REFERENCES [dbo].[Entities] ([ID], [EntTypeID]),
    CONSTRAINT [FK_EntPropertyXRef_EntPropertyMap] FOREIGN KEY ([EntTypeID], [PropID]) REFERENCES [dbo].[EntPropertyMap] ([EntTypeID], [PropID]),
    CONSTRAINT [FK_EntPropertyXRef_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([EntID]),
    CONSTRAINT [FK_EntPropertyXRef_Users1] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Users] ([EntID])
);








GO
CREATE NONCLUSTERED INDEX [IX_EntPropertyXRef_PropID]
    ON [dbo].[EntPropertyXRef]([PropID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntPropertyXRef_EntType]
    ON [dbo].[EntPropertyXRef]([EntTypeID] ASC);


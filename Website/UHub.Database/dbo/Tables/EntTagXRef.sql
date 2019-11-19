CREATE TABLE [dbo].[EntTagXRef] (
    [TargetEntID]     BIGINT             NOT NULL,
    [TargetEntTypeID] SMALLINT           NOT NULL,
    [TagValue]        NVARCHAR (250)     NOT NULL,
    [CreatedDate]     DATETIMEOFFSET (6) CONSTRAINT [DF_EntTagXRef_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    CONSTRAINT [FK_EntTagXRef_Entities] FOREIGN KEY ([TargetEntID], [TargetEntTypeID]) REFERENCES [dbo].[Entities] ([ID], [EntTypeID]),
    CONSTRAINT [FK_EntTagXRef_EntTagMap] FOREIGN KEY ([TargetEntTypeID]) REFERENCES [dbo].[EntTagMap] ([TargetEntTypeID])
);






GO
CREATE NONCLUSTERED INDEX [IX_EntTagXRef_TargetID]
    ON [dbo].[EntTagXRef]([TargetEntID] ASC);


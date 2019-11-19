CREATE TABLE [dbo].[EntViewDateXRef] (
    [ViewID]      BIGINT             NOT NULL,
    [CreatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_EntViewDateXRef_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    CONSTRAINT [FK_EntViewDateXRef_EntViewXRef] FOREIGN KEY ([ViewID]) REFERENCES [dbo].[EntViewXRef] ([ViewID])
);


GO
CREATE NONCLUSTERED INDEX [IX_EntViewDateXRef_1]
    ON [dbo].[EntViewDateXRef]([ViewID] ASC, [CreatedDate] ASC);


GO
CREATE CLUSTERED INDEX [IX_EntViewDateXRef]
    ON [dbo].[EntViewDateXRef]([ViewID] ASC);


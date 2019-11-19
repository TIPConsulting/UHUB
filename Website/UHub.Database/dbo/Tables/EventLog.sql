CREATE TABLE [dbo].[EventLog] (
    [EventTypeID] SMALLINT           NOT NULL,
    [EventID]     NVARCHAR (100)     NULL,
    [Content]     NVARCHAR (MAX)     NOT NULL,
    [CreatedBy]   BIGINT             CONSTRAINT [DF_EventLog_CreatedBy] DEFAULT ((0)) NOT NULL,
    [CreatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_EventLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    CONSTRAINT [FK_EventLog_EventLogTypes] FOREIGN KEY ([EventTypeID]) REFERENCES [dbo].[EventLogTypes] ([ID])
);


GO
CREATE CLUSTERED INDEX [IX_EventLog]
    ON [dbo].[EventLog]([EventID] ASC);


CREATE TABLE [dbo].[ActivityLogXRef] (
    [LogDate]      DATETIMEOFFSET (7) NULL,
    [SessionID]    UNIQUEIDENTIFIER   NULL,
    [UserID]       BIGINT             NULL,
    [LogTypeID]    SMALLINT           NULL,
    [TargetTypeID] SMALLINT           NULL,
    [Target]       NVARCHAR (500)     NULL
);


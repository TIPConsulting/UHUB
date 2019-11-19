CREATE TABLE [dbo].[TokenValidators] (
    [IssueDate]         DATETIMEOFFSET (7) NOT NULL,
    [TokenID]           CHAR (3)           NOT NULL,
    [MaxExpirationDate] DATETIMEOFFSET (7) NOT NULL,
    [IsPersistent]      BIT                NOT NULL,
    [TokenHash]         NVARCHAR (200)     NOT NULL,
    [IsValid]           BIT                CONSTRAINT [DF_TokenValidators_IsValid] DEFAULT ((1)) NOT NULL,
    [RequestID]         NVARCHAR (50)      NOT NULL,
    [SessionID]         NVARCHAR (50)      NOT NULL,
    CONSTRAINT [PK_TokenValidators] PRIMARY KEY CLUSTERED ([IssueDate] ASC, [TokenID] ASC)
);


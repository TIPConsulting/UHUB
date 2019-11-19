CREATE TABLE [dbo].[EntMemberXRef] (
    [TargetEntID]     BIGINT             NOT NULL,
    [ActorEntID]      BIGINT             NOT NULL,
    [TargetEntTypeID] SMALLINT           NOT NULL,
    [ActorEntTypeID]  SMALLINT           NOT NULL,
    [CreatedDate]     DATETIMEOFFSET (6) CONSTRAINT [DF_EntMemberXRef_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [IsApproved]      BIT                CONSTRAINT [DF_EntMemberXRef_IsApproved] DEFAULT ((0)) NOT NULL,
    [IsBanned]        BIT                CONSTRAINT [DF_EntMemberXRef_IsBanned] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_EntMemeberXRef] PRIMARY KEY CLUSTERED ([TargetEntID] ASC, [ActorEntID] ASC),
    CONSTRAINT [FK_EntMemberXRef_Entities] FOREIGN KEY ([TargetEntID], [TargetEntTypeID]) REFERENCES [dbo].[Entities] ([ID], [EntTypeID]),
    CONSTRAINT [FK_EntMemberXRef_Entities1] FOREIGN KEY ([ActorEntID], [ActorEntTypeID]) REFERENCES [dbo].[Entities] ([ID], [EntTypeID]),
    CONSTRAINT [FK_EntMemberXRef_EntMemberMap] FOREIGN KEY ([TargetEntTypeID], [ActorEntTypeID]) REFERENCES [dbo].[EntMemberMap] ([TargetEntTypeID], [ActorEntTypeID])
);






GO
CREATE NONCLUSTERED INDEX [IX_EntMemberXRef_TargetID]
    ON [dbo].[EntMemberXRef]([TargetEntID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EntMemberXRef_ActorID]
    ON [dbo].[EntMemberXRef]([ActorEntID] ASC);


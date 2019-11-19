CREATE view [dbo].[vFiles]
as


            with set1 as
	        (
		        select 
			        EntID,
                    
                    --
			        case when epx.PropID = 2
			        then PropValue
			        else NULL
			        end [Name],
                    --
			        case when epx.PropID = 14
			        then PropValue
			        else NULL
			        end [ViewCount],
                    --
			        case when epx.PropID = 23
			        then PropValue
			        else NULL
			        end [FilePath],
                    --
			        case when epx.PropID = 24
			        then PropValue
			        else NULL
			        end [Description],
                    --
			        case when epx.PropID = 25
			        then PropValue
			        else NULL
			        end [FileHash_SHA256],
                    --
			        case when epx.PropID = 26
			        then PropValue
			        else NULL
			        end [SourceName],
                    --
			        case when epx.PropID = 28
			        then PropValue
			        else NULL
			        end [SourceType],
                    --
			        case when epx.PropID = 29
			        then PropValue
			        else NULL
			        end [DownloadName]

		        from
			        EntPropertyXRef epx
		        where
			        epx.EntTypeID = 8
	        ),
	        set2 as
	        (
		        select
			        EntID,
			        min([Name])                          as [Name],
			        cast(min([ViewCount]) as bigint)     as [ViewCount],
			        min([FilePath])                      as [FilePath],
			        min([Description])                   as [Description],
			        min([FileHash_SHA256])               as [FileHash_SHA256],
			        min([SourceName])                    as [SourceName],
			        min([SourceType])                    as [SourceType],
			        min([DownloadName])                  as [DownloadName]
		        from set1
		        group by
			        EntID
	        )

	        select
		        ent.ID                   as [ID],
		        ent.EntTypeID            as [EntTypeID],
		        ent.IsEnabled            as [IsEnabled],
		        ent.IsReadonly           as [IsReadonly],
		        ent.IsDeleted            as [IsDeleted],
		        ent.CreatedBy            as [CreatedBy],
		        ent.CreatedDate          as [CreatedDate],
		        ent.ModifiedBy           as [ModifiedBy],
		        ent.ModifiedDate         as [ModifiedDate],
		        ent.DeletedBy            as [DeletedBy],
		        ent.DeletedDate          as [DeletedDate],
		        fkJoin_0.ParentEntID     as [ParentID],
		        s2.[Name]                as [Name],
		        s2.[ViewCount]           as [ViewCount],
		        s2.[FilePath]            as [FilePath],
		        s2.[Description]         as [Description],
		        s2.[FileHash_SHA256]     as [FileHash_SHA256],
		        s2.[SourceName]          as [SourceName],
		        s2.[SourceType]          as [SourceType],
		        s2.[DownloadName]        as [DownloadName]
	        from Entities ent

	        inner join set2 s2
	        on
		        ent.ID = s2.EntID


            INNER JOIN dbo.EntChildXRef fkJoin_0
	        on
		        fkJoin_0.ChildEntID = ent.ID

	        where
		        ent.IsDeleted = 0







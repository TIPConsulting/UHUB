CREATE view [dbo].[vPosts]
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
			        case when epx.PropID = 12
			        then PropValue
			        else NULL
			        end [Content],
                    --
			        case when epx.PropID = 13
			        then PropValue
			        else NULL
			        end [IsModified],
                    --
			        case when epx.PropID = 14
			        then PropValue
			        else NULL
			        end [ViewCount],
                    --
			        case when epx.PropID = 33
			        then PropValue
			        else NULL
			        end [IsLocked],
                    --
			        case when epx.PropID = 34
			        then PropValue
			        else NULL
			        end [CanComment],
                    --
			        case when epx.PropID = 35
			        then PropValue
			        else NULL
			        end [IsPublic]

		        from
			        EntPropertyXRef epx
		        where
			        epx.EntTypeID = 6
	        ),
	        set2 as
	        (
		        select
			        EntID,
			        min([Name])                          as [Name],
			        min([Content])                       as [Content],
			        cast(min([IsModified]) as bit)       as [IsModified],
			        cast(min([ViewCount]) as bigint)     as [ViewCount],
			        cast(min([IsLocked]) as bit)         as [IsLocked],
			        cast(min([CanComment]) as bit)       as [CanComment],
			        cast(min([IsPublic]) as bit)         as [IsPublic]
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
		        s2.[Content]             as [Content],
		        s2.[IsModified]          as [IsModified],
		        s2.[ViewCount]           as [ViewCount],
		        s2.[IsLocked]            as [IsLocked],
		        s2.[CanComment]          as [CanComment],
		        s2.[IsPublic]            as [IsPublic]
	        from Entities ent

	        inner join set2 s2
	        on
		        ent.ID = s2.EntID


            INNER JOIN dbo.EntChildXRef fkJoin_0
	        on
		        fkJoin_0.ChildEntID = ent.ID

	        where
		        ent.IsDeleted = 0










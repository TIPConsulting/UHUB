CREATE view [dbo].[vSchools]
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
			        case when epx.PropID = 15
			        then PropValue
			        else NULL
			        end [State],
                    --
			        case when epx.PropID = 16
			        then PropValue
			        else NULL
			        end [City],
                    --
			        case when epx.PropID = 17
			        then PropValue
			        else NULL
			        end [DomainValidator],
                    --
			        case when epx.PropID = 24
			        then PropValue
			        else NULL
			        end [Description]

		        from
			        EntPropertyXRef epx
		        where
			        epx.EntTypeID = 2
	        ),
	        set2 as
	        (
		        select
			        EntID,
			        min([Name])                as [Name],
			        min([State])               as [State],
			        min([City])                as [City],
			        min([DomainValidator])     as [DomainValidator],
			        min([Description])         as [Description]
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
		        s2.[Name]                as [Name],
		        s2.[State]               as [State],
		        s2.[City]                as [City],
		        s2.[DomainValidator]     as [DomainValidator],
		        s2.[Description]         as [Description]
	        from Entities ent

	        inner join set2 s2
	        on
		        ent.ID = s2.EntID



	        where
		        ent.IsDeleted = 0










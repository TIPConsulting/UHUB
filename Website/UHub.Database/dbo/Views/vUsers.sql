CREATE view [dbo].[vUsers]
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
			        case when epx.PropID = 6
			        then PropValue
			        else NULL
			        end [PhoneNumber],
                    --
			        case when epx.PropID = 7
			        then PropValue
			        else NULL
			        end [Major],
                    --
			        case when epx.PropID = 8
			        then PropValue
			        else NULL
			        end [Year],
                    --
			        case when epx.PropID = 10
			        then PropValue
			        else NULL
			        end [ExpectedGradDate],
                    --
			        case when epx.PropID = 30
			        then PropValue
			        else NULL
			        end [Company],
                    --
			        case when epx.PropID = 31
			        then PropValue
			        else NULL
			        end [JobTitle],
                    --
			        case when epx.PropID = 36
			        then PropValue
			        else NULL
			        end [IsFinished]

		        from
			        EntPropertyXRef epx
		        where
			        epx.EntTypeID = 1
	        ),
	        set2 as
	        (
		        select
			        EntID,
			        min([Name])                        as [Name],
			        min([PhoneNumber])                 as [PhoneNumber],
			        min([Major])                       as [Major],
			        min([Year])                        as [Year],
			        min([ExpectedGradDate])            as [ExpectedGradDate],
			        min([Company])                     as [Company],
			        min([JobTitle])                    as [JobTitle],
			        cast(min([IsFinished]) as bit)     as [IsFinished]
		        from set1
		        group by
			        EntID
	        )

	        select
		        ent.ID                     as [ID],
		        ent.EntTypeID              as [EntTypeID],
		        ent.IsEnabled              as [IsEnabled],
		        ent.IsReadonly             as [IsReadonly],
		        ent.IsDeleted              as [IsDeleted],
		        ent.CreatedBy              as [CreatedBy],
		        ent.CreatedDate            as [CreatedDate],
		        ent.ModifiedBy             as [ModifiedBy],
		        ent.ModifiedDate           as [ModifiedDate],
		        ent.DeletedBy              as [DeletedBy],
		        ent.DeletedDate            as [DeletedDate],
		        fkJoin_0.ParentEntID       as [ParentID],
		        s2.[Name]                  as [Name],
		        s2.[PhoneNumber]           as [PhoneNumber],
		        s2.[Major]                 as [Major],
		        s2.[Year]                  as [Year],
		        s2.[ExpectedGradDate]      as [ExpectedGradDate],
		        s2.[Company]               as [Company],
		        s2.[JobTitle]              as [JobTitle],
		        s2.[IsFinished]            as [IsFinished],
		        fkJoin_1.[Email]           as [Email],
		        fkJoin_1.[Domain]          as [Domain],
		        fkJoin_1.[Username]        as [Username],
		        fkJoin_1.[IsConfirmed]     as [IsConfirmed],
		        fkJoin_1.[IsApproved]      as [IsApproved],
		        fkJoin_1.[Version]         as [Version],
		        fkJoin_1.[IsAdmin]         as [IsAdmin]
	        from Entities ent

	        inner join set2 s2
	        on
		        ent.ID = s2.EntID


            INNER JOIN dbo.EntChildXRef fkJoin_0
	        on
		        fkJoin_0.ChildEntID = ent.ID

            INNER JOIN dbo.Users fkJoin_1
	        on
		        fkJoin_1.EntID = ent.ID

	        where
		        ent.IsDeleted = 0














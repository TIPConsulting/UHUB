CREATE view [dbo].[vSchoolClubModerators]
as


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
        fkJoin_1.PropValue       as [UserID],
        fkJoin_2.PropValue       as [IsOwner],
        fkJoin_3.PropValue       as [IsValid]

	from 
		dbo.Entities ent



    inner join dbo.EntChildXRef fkJoin_0
	on
		fkJoin_0.ChildEntID = ent.ID

    inner join dbo.EntPropertyXRef fkJoin_1
	on
		fkJoin_1.EntID = ent.ID
        and fkJoin_1.PropID = 18

    inner join dbo.EntPropertyXRef fkJoin_2
	on
		fkJoin_2.EntID = ent.ID
        and fkJoin_2.PropID = 19

    inner join dbo.EntPropertyXRef fkJoin_3
	on
		fkJoin_3.EntID = ent.ID
        and fkJoin_3.PropID = 22


	where
		ent.EntTypeID = 5
		AND ent.IsDeleted = 0;








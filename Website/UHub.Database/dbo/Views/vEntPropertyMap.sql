create view dbo.vEntPropertyMap
as

select
	epm.*,
	ep.DataType
from dbo.EntPropertyMap epm

inner join dbo.EntProperties ep
on
	epm.PropID = ep.ID


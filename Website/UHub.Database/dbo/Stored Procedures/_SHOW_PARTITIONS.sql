create proc dbo._SHOW_PARTITIONS
as
begin
select 
    object_schema_name(i.object_id) as [schema],
    object_name(i.object_id) as [object_name],
    t.name as [table_name],
    i.name as [index_name],
    s.name as [partition_scheme],
	p.partition_number
from sys.indexes i
    join sys.partition_schemes s on i.data_space_id = s.data_space_id
    join sys.tables t on i.object_id = t.object_id
	join sys.partitions p on p.object_id = t.object_id
end
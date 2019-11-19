using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.DataInterop
{
    public static partial class SqlHelper
    {

        internal static Dictionary<int, string> SqlObjectTypeMap;

        private const string TableRelationshipQuery = @"SELECT
                            FK.TABLE_SCHEMA		as FK_Schema,
                            FK.TABLE_NAME       as FK_Table,
                            CU.COLUMN_NAME      as FK_Column,
                            PK.TABLE_SCHEMA		as PK_Schema,
                            PK.TABLE_NAME       as PK_Table,
                            PT.COLUMN_NAME      as PK_Column,
                            C.CONSTRAINT_NAME   as Constraint_Name
                            FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
                            INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK
                            ON 
                                C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
                            INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK
                            ON 
                                C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
                            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU
                            ON 
                                C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
                            INNER JOIN
                            (
                                SELECT i1.TABLE_NAME, i2.COLUMN_NAME
                                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
                                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
                                WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
                            ) PT
                            ON 
                                PT.TABLE_NAME = PK.TABLE_NAME";



        static SqlHelper()
        {
            SqlObjectTypeMap = new Dictionary<int, string>
            {
                [1] = "AF",     //Aggregate Function
                [2] = "C",      //CHECK Constraint
                [3] = "D",      //DEFAULT
                [4] = "F",      //Foreign Key Constraint
                [5] = "FN",     //Scalar Value Function
                [6] = "FS",     //CLR Scalar Value Function
                [7] = "FT",     //CLR Table Value Function
                [8] = "IF",     //Inline Table Value Function
                [9] = "IT",     //Internal Table
                [10] = "P",     //Stored Proc
                [11] = "PC",    //CLR Stored Proc
                [12] = "PG",    //Plan Guid
                [13] = "PK",    //Primary Key Constraint
                [14] = "R",     //Rule
                [15] = "RF",    //Replication-Filter-Procedure
                [16] = "S",     //System Base Table
                [17] = "SN",    //Synonym
                [18] = "SO",    //Sequence Object
                [19] = "U",     //User Defined Table
                [20] = "V",     //View
                [21] = "EC"     //Edge Constraint

            };
        }


    }
}

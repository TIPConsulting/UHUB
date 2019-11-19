using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.DataInterop
{
    public enum SqlObjectType
    {
        /// <summary>
        /// Aggregate Function [AF]
        /// </summary>
        AggregateFunc = 1,

        /// <summary>
        /// CHECK Constraint [C]
        /// </summary>
        Check = 2,

        /// <summary>
        /// DEFAULT [D]
        /// </summary>
        Default = 3,

        /// <summary>
        /// Foreign Key Constraint [F]
        /// </summary>
        ForeignKey = 4,

        /// <summary>
        /// Scalar Value Function [FN]
        /// </summary>
        ScalarFunc = 5,

        /// <summary>
        /// CLR Scalar Value Function [FS]
        /// </summary>
        ClrScalarFunc = 6,

        /// <summary>
        /// CLR Table Value Function [FT]
        /// </summary>
        ClrTableFunc = 7,

        /// <summary>
        /// Inline Table Value Function [IF]
        /// </summary>
        TableFunc = 8,
        
        /// <summary>
        /// Internal Table [IT]
        /// </summary>
        InternalTable = 9,

        /// <summary>
        /// Stored Proc [P]
        /// </summary>
        Sproc = 10,

        /// <summary>
        /// CLR Stored Proc [PC]
        /// </summary>
        ClrSProc = 11,
        
        /// <summary>
        /// Plan Guide [PG]
        /// </summary>
        PlanGuide = 12,

        /// <summary>
        /// Primary Key Constraint [PK]
        /// </summary>
        PrimaryKey = 13,
        
        /// <summary>
        /// Rule (old) [R]
        /// </summary>
        Rule = 14,

        /// <summary>
        /// Replication-Filter-Procedure [RF]
        /// </summary>
        RFP = 15,

        /// <summary>
        /// System Base Table [S]
        /// </summary>
        SysTable = 16,

        /// <summary>
        /// Synonym [SN]
        /// </summary>
        Synonym = 17,

        /// <summary>
        /// Sequence Object [SO]
        /// </summary>
        SequenceObj = 18,

        /// <summary>
        /// User Defined Table [U]
        /// </summary>
        UserTable = 19,

        /// <summary>
        /// View [V]
        /// </summary>
        View = 20,

        /// <summary>
        /// Edge Constraint [EC]
        /// </summary>
        EdgeConstraint = 21
       
    }
}

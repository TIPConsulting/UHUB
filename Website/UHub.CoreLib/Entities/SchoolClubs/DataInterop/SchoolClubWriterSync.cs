using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.ClientFriendly;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
using static UHub.CoreLib.DataInterop.SqlConverters;

namespace UHub.CoreLib.Entities.SchoolClubs.DataInterop
{
    internal static partial class SchoolClubWriter
    {

        /// <summary>
        /// Create DB entity for school club
        /// </summary>
        /// <param name="Club"></param>
        /// <returns></returns>
        public static long? CreateClub(SchoolClub Club)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            long? clubID = SqlWorker.ExecScalar<long?>(
                _dbConn,
                "[dbo].[SchoolClub_Create]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = HandleParamEmpty(Club.Name);
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = HandleParamEmpty(Club.Description);
                    cmd.Parameters.Add("@ParentID", SqlDbType.BigInt).Value = Club.SchoolID;
                    cmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = Club.CreatedBy;
                    cmd.Parameters.Add("@IsReadOnly", SqlDbType.BigInt).Value = Club.IsReadOnly;
                });


            return clubID;

        }
    }
}

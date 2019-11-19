using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.SchoolClubs.DataInterop
{
    public static partial class SchoolClubReader
    {
        public static async Task<IEnumerable<long>> TryGetValidClubMembershipsAsync(long UserID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return await SqlWorker.ExecAdvQueryAsync<long>(
                    _dbConn,
                    "[dbo].[User_GetValidClubMemberships_Light]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                    },
                    (reader) =>
                    {
                        //READ COLUMN 0
                        //read column 0 as long
                        //this column has the ID of clubs where user is an approved member
                        return reader.GetInt64(0);
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("536D5F9A-E9AB-429F-A6DF-67A966461D1A");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.SchoolClubs.DataInterop
{
    public static partial class SchoolClubReader
    {

        /// <summary>
        /// Get DB school club full detail by LONG ID
        /// </summary>
        /// <param name="SchoolClubID"></param>
        /// <returns></returns>
        public static async Task<SchoolClub> TryGetClubAsync(long SchoolClubID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                var temp = SqlWorker.ExecEntityQueryAsync<SchoolClub>(
                    _dbConn,
                    "[dbo].[SchoolClub_GetByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolClubID", SqlDbType.BigInt).Value = SchoolClubID;
                    });

                return (await temp).SingleOrDefault();

            }
            catch (Exception ex)
            {
                var exID = new Guid("DE0D14EB-0091-4C9D-962C-CDCDA69B9775");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }



        /// <summary>
        /// Get all the school clubs in the DB
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<SchoolClub>> TryGetAllClubsAsync()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return await SqlWorker.ExecEntityQueryAsync<SchoolClub>(_dbConn, "[dbo].[SchoolClubs_GetAll]");

            }
            catch (Exception ex)
            {
                var exID = new Guid("1CE82570-22DA-4DA5-96FB-3B1A417A4B09");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }

        /// <summary>
        /// Get all the school clubs in the DB
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SchoolClub>> TryGetClubsBySchoolAsync(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return await SqlWorker.ExecEntityQueryAsync<SchoolClub>(
                    _dbConn,
                    "[dbo].[SchoolClubs_GetBySchool]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("887A70DF-2D82-4E34-9462-ABC0CA252E32");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }

        /// <summary>
        /// Get all the school clubs in the DB for a school using the email addr of a user
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SchoolClub>> TryGetClubsByEmailAsync(string Email)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmail(Email))
            {
                return Enumerable.Empty<SchoolClub>();
            }


            try
            {
                return await SqlWorker.ExecEntityQueryAsync<SchoolClub>(
                    _dbConn,
                    "[dbo].[SchoolClubs_GetByEmail]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("3B1D5714-49AC-444D-B65A-F14B8B34305B");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }


        /// <summary>
        /// Get all the school clubs in the DB for a school using the school domain
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SchoolClub>> TryGetClubsByDomainAsync(string Domain)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmailDomain(Domain))
            {
                return Enumerable.Empty<SchoolClub>();
            }


            try
            {
                return await SqlWorker.ExecEntityQueryAsync<SchoolClub>(
                    _dbConn,
                    "[dbo].[SchoolClubs_GetByDomain]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                    });
            }
            catch (Exception ex)
            {
                var exID = new Guid("920AD4FB-8F4C-431C-ADC6-63D1E835A122");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }


    }
}

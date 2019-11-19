using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.Entities.SchoolClubs.DataInterop
{
    public static partial class SchoolClubReader
    {

        /// <summary>
        /// Get DB school club full detail by LONG ID
        /// </summary>
        /// <param name="SchoolClubID"></param>
        /// <returns></returns>
        public static SchoolClub TryGetClub(long SchoolClubID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return SqlWorker.ExecEntityQuery<SchoolClub>(
                    _dbConn,
                    "[dbo].[SchoolClub_GetByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolClubID", SqlDbType.BigInt).Value = SchoolClubID;
                    })
                    .SingleOrDefault();

            }
            catch (Exception ex)
            {
                var exID = new Guid("DB0F9B85-F015-4898-9640-AC932B7BE290");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }




        /// <summary>
        /// Get all the school clubs in the DB
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SchoolClub> TryGetAllClubs()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return SqlWorker.ExecEntityQuery<SchoolClub>(_dbConn, "[dbo].[SchoolClubs_GetAll]");

            }
            catch (Exception ex)
            {
                var exID = new Guid("1632E25F-B6D8-4597-B45C-F96A6E3B2EE1");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }

        /// <summary>
        /// Get all the school clubs in the DB
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        public static IEnumerable<SchoolClub> TryGetClubsBySchool(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return SqlWorker.ExecEntityQuery<SchoolClub>(
                    _dbConn,
                    "[dbo].[SchoolClubs_GetBySchool]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("9F041C9F-1ACC-4BFE-BAEF-04906E62FCC4");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }

        /// <summary>
        /// Get all the school clubs in the DB for a school using the email addr of a user
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static IEnumerable<SchoolClub> TryGetClubsByEmail(string Email)
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
                return SqlWorker.ExecEntityQuery<SchoolClub>(
                    _dbConn,
                    "[dbo].[SchoolClubs_GetByEmail]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("47AC2532-FABD-4709-A436-07F307472741");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }


        /// <summary>
        /// Get all the school clubs in the DB for a school using the school domain
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static IEnumerable<SchoolClub> TryGetClubsByDomain(string Domain)
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
                return SqlWorker.ExecEntityQuery<SchoolClub>(
                    _dbConn,
                    "[dbo].[SchoolClubs_GetByDomain]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                    });
            }
            catch (Exception ex)
            {
                var exID = new Guid("9ED9915C-F4C5-4CB7-991F-B40ABDD6A965");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }


    }
}

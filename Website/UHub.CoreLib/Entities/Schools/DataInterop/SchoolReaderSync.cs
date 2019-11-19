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

namespace UHub.CoreLib.Entities.Schools.DataInterop
{
    public static partial class SchoolReader
    {


        public static IEnumerable<School> TryGetAllSchools()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return SqlWorker.ExecEntityQuery<School>(_dbConn, "[dbo].[Schools_GetAll]");
            }
            catch (Exception ex)
            {
                var exID = new Guid("90649690-2183-4086-8A87-4EC4CE12D329");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }

        }


        /// <summary>
        /// Get Db school full detail by ID
        /// </summary>
        /// <param name="ID">School ID</param>
        /// <returns></returns>
        public static School TryGetSchool(long SchoolID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return SqlWorker.ExecEntityQuery<School>(
                    _dbConn,
                    "[dbo].[School_GetByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    })
                    .SingleOrDefault();

            }
            catch (Exception ex)
            {
                var exID = new Guid("A9D37B0B-7567-4C82-A0CB-A4E979BE8EFB");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }


        /// <summary>
        /// Get Db school full detail by Name
        /// </summary>
        /// <param name="ID">School ID</param>
        /// <returns></returns>
        public static School TryGetSchoolByName(string SchoolName)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return SqlWorker.ExecEntityQuery<School>(
                    _dbConn,
                    "[dbo].[School_GetByName]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolName", SqlDbType.NVarChar).Value = @SchoolName;
                    })
                    .SingleOrDefault();

            }
            catch (Exception ex)
            {
                var exID = new Guid("306296C0-50D4-4781-A6C4-ADBAA61E12CC");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }



        /// <summary>
        /// Get Db school full detail by user email. Used to get a user's school at account creation
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static School TryGetSchoolByEmail(string Email)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return SqlWorker.ExecEntityQuery<School>(
                    _dbConn,
                    "[dbo].[School_GetByEmail]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                    })
                    .SingleOrDefault();

            }
            catch (Exception ex)
            {
                var exID = new Guid("6FBF084B-B86A-4DEA-8AD4-58E4B56E36BC");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }


        /// <summary>
        /// Get Db school full detail by email domain. Used to get a user's school at account creation
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static School TryGetSchoolByDomain(string Domain)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            if (!Validators.IsValidEmailDomain(Domain))
            {
                return null;
            }


            try
            {
                return SqlWorker.ExecEntityQuery<School>(
                    _dbConn,
                    "[dbo].[School_GetByDomain]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                    })
                    .SingleOrDefault();

            }
            catch (Exception ex)
            {
                var exID = new Guid("C58714AF-812E-446A-A756-955DF6FBA428");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }


        public static bool TryIsEmailValid(string Email)
        {
            if (!Validators.IsValidEmail(Email))
            {
                return false;
            }

            var domain = Email.GetEmailDomain();

            try
            {
                return SqlWorker.ExecScalar<bool>(
                    _dbConn,
                    "[dbo].[School_IsDomainValid]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = domain;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("57C20943-768B-4C32-9CC5-85DBFDE563B5");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return false;
            }
        }

        public static bool TryIsDomainValid(string Domain)
        {

            if (!Validators.IsValidEmailDomain(Domain))
            {
                return false;
            }


            try
            {
                return SqlWorker.ExecScalar<bool>(
                    _dbConn,
                    "[dbo].[School_IsDomainValid]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("66D95D70-3261-4E26-BC06-9C01BF476E08");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return false;
            }
        }
    }
}

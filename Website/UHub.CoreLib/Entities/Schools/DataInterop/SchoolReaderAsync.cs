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

        public static async Task<IEnumerable<School>> TryGetAllSchoolsAsync()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return await SqlWorker.ExecEntityQueryAsync<School>(_dbConn, "[dbo].[Schools_GetAll]");

            }
            catch (Exception ex)
            {
                var exID = new Guid("A9C7339B-7FD0-4224-AFAB-C8447611F25D");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }


        /// <summary>
        /// Get Db school full detail by ID
        /// </summary>
        /// <param name="ID">School ID</param>
        /// <returns></returns>
        public static async Task<School> TryGetSchoolAsync(long SchoolID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                var temp = await SqlWorker.ExecEntityQueryAsync<School>(
                    _dbConn,
                    "[dbo].[School_GetByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    });
                return temp.SingleOrDefault();


            }
            catch (Exception ex)
            {
                var exID = new Guid("F22F4424-6D73-4F7E-B9AC-87A49EACC88C");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }


        }


        /// <summary>
        /// Get Db school full detail by Name
        /// </summary>
        /// <param name="ID">School ID</param>
        /// <returns></returns>
        public static async Task<School> TryGetSchoolByNameAsync(string SchoolName)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                var temp = await SqlWorker.ExecEntityQueryAsync<School>(
                    _dbConn,
                    "[dbo].[School_GetByName]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolName", SqlDbType.NVarChar).Value = @SchoolName;
                    });

                return temp.SingleOrDefault();

            }
            catch (Exception ex)
            {
                var exID = new Guid("8B7670E6-9D51-4961-ACD6-28E979BE0EA7");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }



        /// <summary>
        /// Get Db school full detail by user email. Used to get a user's school at account creation
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static async Task<School> TryGetSchoolByEmailAsync(string Email)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                var temp = await SqlWorker.ExecEntityQueryAsync<School>(
                    _dbConn,
                    "[dbo].[School_GetByEmail]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                    });
                return temp.SingleOrDefault();


            }
            catch (Exception ex)
            {
                var exID = new Guid("E52D97EE-2CED-4F27-994D-B701C8547311");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }

        }


        /// <summary>
        /// Get Db school full detail by email domain. Used to get a user's school at account creation
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static async Task<School> TryGetSchoolByDomainAsync(string Domain)
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
                var temp = await SqlWorker.ExecEntityQueryAsync<School>(
                    _dbConn,
                    "[dbo].[School_GetByDomain]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                    });

                return temp.SingleOrDefault();


            }
            catch (Exception ex)
            {
                var exID = new Guid("23AFFEF9-8D06-4FA1-91BF-BF233B11736D");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }


        public static async Task<bool> TryIsEmailValidAsync(string Email)
        {
            if (!Validators.IsValidEmail(Email))
            {
                return false;
            }


            var domain = Email.GetEmailDomain();


            try
            {
                return await SqlWorker.ExecScalarAsync<bool>(
                    _dbConn,
                    "[dbo].[School_IsDomainValid]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = domain;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("150FFD55-0B39-43FF-BEE4-C6EC97931D15");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return false;
            }
        }

        public static async Task<bool> TryIsDomainValidAsync(string Domain)
        {
            if (!Validators.IsValidEmailDomain(Domain))
            {
                return false;
            }


            try
            {
                return await SqlWorker.ExecScalarAsync<bool>(
                    _dbConn,
                    "[dbo].[School_IsDomainValid]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("A18440AB-870D-49B9-8813-09363C0FF1B4");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return false;
            }
        }
    }
}

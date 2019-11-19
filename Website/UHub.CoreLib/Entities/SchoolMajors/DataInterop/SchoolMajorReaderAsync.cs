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

namespace UHub.CoreLib.Entities.SchoolMajors.DataInterop
{
    public static partial class SchoolMajorReader
    {


        /// <summary>
        /// Get DB school major full detail by LONG ID
        /// </summary>
        /// <param name="SchoolMajorID"></param>
        /// <returns></returns>
        public static async Task<SchoolMajor> TryGetMajorAsync(long SchoolMajorID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                var temp = await SqlWorker.ExecEntityQueryAsync<SchoolMajor>(
                    _dbConn,
                    "[dbo].[SchoolMajor_GetByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolMajorID", SqlDbType.BigInt).Value = SchoolMajorID;
                    });

                return temp.SingleOrDefault();

            }
            catch (Exception ex)
            {
                var exID = new Guid("E80BCB26-D221-458A-999C-C885A0D4F018");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }



        /// <summary>
        /// Get all the school majors in the DB
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<SchoolMajor>> TryGetAllMajorsAsync()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return await SqlWorker.ExecEntityQueryAsync<SchoolMajor>(_dbConn, "[dbo].[SchoolMajors_GetAll]");

            }
            catch (Exception ex)
            {
                var exID = new Guid("B007CC88-B728-4BDE-854C-C125A263FDB5");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }

        /// <summary>
        /// Get all the school majors in the DB by school
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SchoolMajor>> TryGetMajorsBySchoolAsync(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return await SqlWorker.ExecEntityQueryAsync<SchoolMajor>(
                    _dbConn,
                    "[dbo].[SchoolMajors_GetBySchool]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("03710860-1CB4-4867-BFE8-2ED83F0ED6A2");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }


        /// <summary>
        /// Get all the school majors in the DB for a school using the email addr of a user
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SchoolMajor>> TryGetMajorsByEmailAsync(string Email)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmail(Email))
            {
                return Enumerable.Empty<SchoolMajor>();
            }

            try
            {
                return await SqlWorker.ExecEntityQueryAsync<SchoolMajor>(
                    _dbConn,
                    "[dbo].[SchoolMajors_GetByEmail]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("EA923AAB-04CE-4B39-8F83-6DED5538CE76");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }

        /// <summary>
        /// Get all the school majors in the DB for a school using the school domain
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SchoolMajor>> TryGetMajorsByDomainAsync(string Domain)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmailDomain(Domain))
            {
                return Enumerable.Empty<SchoolMajor>();
            }


            try
            {
                return await SqlWorker.ExecEntityQueryAsync<SchoolMajor>(
                    _dbConn,
                    "[dbo].[SchoolMajors_GetByDomain]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("203FD5B2-3C56-4A67-9D8F-01DC54A1F5B2");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return null;
            }
        }

    }
}

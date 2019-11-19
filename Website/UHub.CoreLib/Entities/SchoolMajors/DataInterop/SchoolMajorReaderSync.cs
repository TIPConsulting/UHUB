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
        public static SchoolMajor TryGetMajor(long SchoolMajorID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {

                return SqlWorker.ExecEntityQuery<SchoolMajor>(
                    _dbConn,
                    "[dbo].[SchoolMajor_GetByID]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolMajorID", SqlDbType.BigInt).Value = SchoolMajorID;
                    })
                    .SingleOrDefault();

            }
            catch (Exception ex)
            {
                var exID = new Guid("DF24DBC5-577C-46E9-9E83-AF67CD1C40E3");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }



        /// <summary>
        /// Get all the school majors in the DB
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SchoolMajor> TryGetAllMajors()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            try
            {
                return SqlWorker.ExecEntityQuery<SchoolMajor>(_dbConn, "[dbo].[SchoolMajors_GetAll]");

            }
            catch (Exception ex)
            {
                var exID = new Guid("3D63B30C-80CE-4A2B-95E9-B51A13A49994");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }

        /// <summary>
        /// Get all the school majors in the DB by school
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        public static IEnumerable<SchoolMajor> TryGetMajorsBySchool(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            try
            {
                return SqlWorker.ExecEntityQuery<SchoolMajor>(
                    _dbConn,
                    "[dbo].[SchoolMajors_GetBySchool]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("411B43FA-9F3E-4C9F-9433-BA366EA20D55");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }


        /// <summary>
        /// Get all the school majors in the DB for a school using the email addr of a user
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static IEnumerable<SchoolMajor> TryGetMajorsByEmail(string Email)
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

                return SqlWorker.ExecEntityQuery<SchoolMajor>(
                    _dbConn,
                    "[dbo].[SchoolMajors_GetByEmail]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("EEE91FB7-9F50-4D13-A88D-349421B3EA6D");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }

        /// <summary>
        /// Get all the school majors in the DB for a school using the school domain
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static IEnumerable<SchoolMajor> TryGetMajorsByDomain(string Domain)
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
                return SqlWorker.ExecEntityQuery<SchoolMajor>(
                    _dbConn,
                    "[dbo].[SchoolMajors_GetByDomain]",
                    (cmd) =>
                    {
                        cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                    });

            }
            catch (Exception ex)
            {
                var exID = new Guid("2B103763-54EE-4DBE-8372-E0659F246100");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return null;
            }
        }


    }
}

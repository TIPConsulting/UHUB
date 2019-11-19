using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Entities.Users.Interfaces;
using static UHub.CoreLib.DataInterop.SqlConverters;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;

namespace UHub.CoreLib.Entities.Users.DataInterop
{
    public static partial class UserReader
    {


        #region Individual
        public static bool DoesUserExist(long UserID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecScalar<bool>(
                _dbConn,
                "[dbo].[User_DoesExistByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });
        }

        public static bool DoesUserExist(string Email)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmail(Email))
            {
                return false;
            }


            return SqlWorker.ExecScalar<bool>(
                _dbConn,
                "[dbo].[User_DoesExistByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = HandleParamEmpty(Email);
                });
        }

        public static bool DoesUserExist(string Username, string Domain)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmailDomain(Domain))
            {
                return false;
            }

            return SqlWorker.ExecScalar<bool>(
                _dbConn,
                "[dbo].[User_DoesExistByUsername]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = HandleParamEmpty(Username);
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = HandleParamEmpty(Domain);
                });
        }



        /// <summary>
        /// Get user ID from email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static long? GetUserID(string Email)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmail(Email))
            {
                return null;
            }

            return SqlWorker.ExecScalar<long>(
                _dbConn,
                "[dbo].[User_GetIDByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = HandleParamEmpty(Email);
                });
        }

        /// <summary>
        /// Get user ID from username and domain
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public static long? GetUserID(string Username, string Domain)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmailDomain(Domain))
            {
                return null;
            }


            return SqlWorker.ExecScalar<long>(
                _dbConn,
                "[dbo].[User_GetIDByUsername]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = HandleParamEmpty(Username);
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = HandleParamEmpty(Domain);
                });
        }

        /// <summary>
        /// Get DB User full detail by ID
        /// </summary>
        /// <param name="UserUID"></param>
        /// <returns></returns>
        public static User GetUser(long UserID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecEntityQuery<User>(
                _dbConn,
                "[dbo].[User_GetByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                })
                .SingleOrDefault();
        }

        /// <summary>
        /// Get DB User full detail by email
        /// </summary>
        ///<param name="Email"
        /// <returns></returns>
        public static User GetUser(string Email)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmail(Email))
            {
                return null;
            }



            return SqlWorker.ExecEntityQuery<User>(
                _dbConn,
                "[dbo].[User_GetByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = HandleParamEmpty(Email);
                })
                .SingleOrDefault();
        }

        /// <summary>
        /// Get DB User full detail by username and domain
        /// </summary>
        ///<param name="Username"
        /// <returns></returns>
        public static User GetUser(string Username, string Domain)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmailDomain(Domain))
            {
                return null;
            }


            return SqlWorker.ExecEntityQuery<User>(
                _dbConn,
                "[dbo].[User_GetByUsername]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = HandleParamEmpty(Username);
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = HandleParamEmpty(Domain);
                })
                .SingleOrDefault();
        }
        #endregion Individual


        #region Group

        public static IEnumerable<User> GetAllUsers()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return SqlWorker.ExecEntityQuery<User>(_dbConn, "[dbo].[Users_GetAll]");
        }



        public static IEnumerable<User> GetAllBySchool(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecEntityQuery<User>(
                _dbConn,
                "[dbo].[Users_GetBySchoolID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                });
        }
        #endregion Group


        #region System
        private static long GetUserSystemID()
        {
            //HARDCODE
            //CONSTANT
            return Constants.SYSTEM_USER_ID;
        }

        #endregion System


    }
}

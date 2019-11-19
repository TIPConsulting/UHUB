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
        public static async Task<bool> DoesUserExistAsync(long UserID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }



            return await SqlWorker.ExecScalarAsync<bool>(
            _dbConn,
            "[dbo].[User_DoesExistByID]",
            (cmd) =>
            {
                cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
            });

        }

        public static async Task<bool> DoesUserExistAsync(string Email)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmail(Email))
            {
                return false;
            }



            return await SqlWorker.ExecScalarAsync<bool>(
            _dbConn,
            "[dbo].[User_DoesExistByEmail]",
            (cmd) =>
            {
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = HandleParamEmpty(Email);
            });

        }

        public static async Task<bool> DoesUserExistAsync(string Username, string Domain)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmailDomain(Domain))
            {
                return false;
            }



            return await SqlWorker.ExecScalarAsync<bool>(
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
        public static async Task<long?> GetUserIDAsync(string Email)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmail(Email))
            {
                return null;
            }


            return await SqlWorker.ExecScalarAsync<long>(
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
        public static async Task<long?> GetUserIDAsync(string Username, string Domain)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmailDomain(Domain))
            {
                return null;
            }


            return await SqlWorker.ExecScalarAsync<long>(
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
        public static async Task<User> GetUserAsync(long UserID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }



            var temp = await SqlWorker.ExecEntityQueryAsync<User>(
                _dbConn,
                "[dbo].[User_GetByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = UserID;
                });


            return temp.SingleOrDefault();

        }

        /// <summary>
        /// Get DB User full detail by email
        /// </summary>
        ///<param name="Email"
        /// <returns></returns>
        public static async Task<User> GetUserAsync(string Email)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmail(Email))
            {
                return null;
            }



            var temp = await SqlWorker.ExecEntityQueryAsync<User>(
            _dbConn,
            "[dbo].[User_GetByEmail]",
            (cmd) =>
            {
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = HandleParamEmpty(Email);
            });

            return temp.SingleOrDefault();

        }

        /// <summary>
        /// Get DB User full detail by username and domain
        /// </summary>
        ///<param name="Username"
        /// <returns></returns>
        public static async Task<User> GetUserAsync(string Username, string Domain)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Validators.IsValidEmailDomain(Domain))
            {
                return null;
            }



            var temp = await SqlWorker.ExecEntityQueryAsync<User>(
                _dbConn,
                "[dbo].[User_GetByUsername]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = HandleParamEmpty(Username);
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = HandleParamEmpty(Domain);
                });

            return temp.SingleOrDefault();

        }
        #endregion Individual


        #region Group
        /// <summary>
        /// Get users with lower security class
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }



            return await SqlWorker.ExecEntityQueryAsync<User>(_dbConn, "[dbo].[Users_GetAll]");

        }



        public static async Task<IEnumerable<User>> GetAllBySchoolAsync(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return await SqlWorker.ExecEntityQueryAsync<User>(
                _dbConn,
                "[dbo].[Users_GetBySchoolID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                });
        }
        #endregion Group




    }
}

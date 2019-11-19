using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.RegExp.Compiled;

namespace UHub.CoreLib.DataInterop
{
    /// <summary>
    /// SQL encapsulation provider
    /// </summary>
    public static partial class SqlWorker
    {

        /// <summary>
        /// Attempts to perform simple cast from DB type to CLR type for a single value
        /// </summary>
        /// <param name="SqlConn">The DB connection string being used</param>
        /// <param name="CmdName">The name of the DB sProc being called</param>
        /// <param name="SetParams">A function to set parameters for the SQL cmd before the DB call</param>
        /// <returns></returns>
        /// <exception cref="SystemDisabledException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidCastException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="AccessForbiddenException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <exception cref="DuplicateNameException"/>
        /// <exception cref="EntityGoneException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="SqlException"/>
        /// <exception cref="ObjectDisposedException"/>
        public static async Task<T> ExecScalarAsync<T>(SqlConfig SqlConn, string CmdName, Action<SqlCommand> SetParams)
        {

            try
            {
                using (var conn = new SqlConnection(SqlConn))
                {
                    conn.Credential = SqlConn.UserCredential;
                    conn.Open();
                    using (var cmd = new SqlCommand(CmdName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 30;

                        SetParams?.Invoke(cmd);


                        var result = await cmd.ExecuteScalarAsync();
                        return (T)(dynamic)result;

                    }
                }
            }
            catch (Exception ex)
            {

                var errCode = ex.Message.Substring(0, 4);
                var errMsg = ex.Message.Substring(ex.Message.IndexOf(": ") + 2);


                if (errCode == "400:")
                {
                    var isInvalidArg = RgxCompiled.DataInterop.INVALID_ARGUMENT_B.IsMatch(errMsg);

                    //general argument exception
                    if (errMsg.Contains("Invalid request arguments"))
                        throw new ArgumentException(errMsg);
                    //specific argument does not meet requirements
                    if (isInvalidArg)
                        throw new ArgumentOutOfRangeException("", errMsg);
                    //argument is null/empty
                    else if (errMsg.Contains("cannot be null or empty"))
                        throw new ArgumentNullException("", errMsg);
                    //argument casting error
                    else if (errMsg.Contains("value cannot be converted"))
                        throw new InvalidDBCastException(errMsg);
                    //general
                    else
                        throw new InvalidOperationException(errMsg);
                }
                //invalid authentication/authorization
                else if (errCode == "403:")
                {
                    throw new AccessForbiddenException(errMsg);
                }
                //not found
                else if (errCode == "404:")
                {
                    throw new KeyNotFoundException(errMsg);
                }
                //duplicate conflict
                else if (errCode == "409:")
                {
                    throw new DuplicateNameException(errMsg);
                }
                //parent gone
                else if (errCode == "410:")
                {
                    throw new EntityGoneException(errMsg);
                }
                //invalid file type
                else if (errCode == "415:")
                {
                    throw new InvalidOperationException(errMsg);
                }
                //throw original exception
                else
                {
                    throw;
                }

            }

        }
        /// <summary>
        /// Attempt to execute non-returning query against DB
        /// </summary>
        /// <param name="SqlConn">The DB connection string being used</param>
        /// <param name="CmdName"></param>
        /// <param name="SetParams"></param>
        /// <returns></returns>
        /// <exception cref="SystemDisabledException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidDBCastException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="AccessForbiddenException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <exception cref="DuplicateNameException"/>
        /// <exception cref="EntityGoneException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="SqlException"/>
        /// <exception cref="ObjectDisposedException"/>
        public static async Task ExecNonQueryAsync(SqlConfig SqlConn, string CmdName, Action<SqlCommand> SetParams)
        {
            try
            {
                using (var conn = new SqlConnection(SqlConn))
                {
                    conn.Credential = SqlConn.UserCredential;
                    conn.Open();
                    using (var cmd = new SqlCommand(CmdName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 30;

                        SetParams?.Invoke(cmd);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {

                var errCode = ex.Message.Substring(0, 4);
                var errMsg = ex.Message.Substring(ex.Message.IndexOf(": ") + 2);

                if (errCode == "400:")
                {
                    var isInvalidArg = RgxCompiled.DataInterop.INVALID_ARGUMENT_B.IsMatch(errMsg);

                    //general argument exception
                    if (errMsg.Contains("Invalid request arguments"))
                        throw new ArgumentException(errMsg);
                    //specific argument does not meet requirements
                    if (isInvalidArg)
                        throw new ArgumentOutOfRangeException("", errMsg);
                    //argument is null/empty
                    else if (errMsg.Contains("cannot be null or empty"))
                        throw new ArgumentNullException("", errMsg);
                    //argument casting error
                    else if (errMsg.Contains("value cannot be converted"))
                        throw new InvalidDBCastException(errMsg);
                    //general
                    else
                        throw new InvalidOperationException(errMsg);
                }
                //invalid authentication/authorization
                else if (errCode == "403:")
                {
                    throw new AccessForbiddenException(errMsg);
                }
                //not found
                else if (errCode == "404:")
                {
                    throw new KeyNotFoundException(errMsg);
                }
                //duplicate conflict
                else if (errCode == "409:")
                {
                    throw new DuplicateNameException(errMsg);
                }
                //parent gone
                else if (errCode == "410:")
                {
                    throw new EntityGoneException(errMsg);
                }
                //invalid file type
                else if (errCode == "415:")
                {
                    throw new InvalidOperationException(errMsg);
                }
                //throw original exception
                else
                {
                    throw;
                }
            }
        }


        /// <summary>
        /// Attempts to perform complex conversion from DB record to DBEntity
        /// </summary>
        /// <param name="SqlConn">The DB connection string being used</param>
        /// <param name="CmdName">The name of the DB sProc being called</param>
        /// <param name="SetParams">A function to set parameters for the SQL cmd before the DB call</param>
        /// <param name="InitQuery">Specify custom method to initiate SQL query</param>
        /// <returns></returns>
        /// <exception cref="SystemDisabledException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidDBCastException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="AccessForbiddenException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <exception cref="DuplicateNameException"/>
        /// <exception cref="EntityGoneException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="SqlException"/>
        /// <exception cref="ObjectDisposedException"/>
        public static async Task<IEnumerable<T>> ExecEntityQueryAsync<T>(SqlConfig SqlConn, string CmdName, Action<SqlCommand> SetParams = null)
            where T : DBEntityBase, new()
        {

            using (var conn = new SqlConnection(SqlConn))
            {
                conn.Credential = SqlConn.UserCredential;
                conn.Open();
                using (var cmd = new SqlCommand(CmdName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;

                    SetParams?.Invoke(cmd);


                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var returnSet = new ConcurrentBag<T>();

                            while (await reader.ReadAsync())
                            {
                                var newEnt = new T();
                                newEnt.LoadDataReader(reader);

                                returnSet.Add(newEnt);
                            }

                            return returnSet;
                        }

                    }
                    catch (Exception ex)
                    {
                        var errCode = ex.Message.Substring(0, 4);
                        var errMsg = ex.Message.Substring(ex.Message.IndexOf(": ") + 2);

                        if (errCode == "400:")
                        {
                            var isInvalidArg = RgxCompiled.DataInterop.INVALID_ARGUMENT_B.IsMatch(errMsg);


                            //general argument exception
                            if (errMsg.Contains("Invalid request arguments"))
                                throw new ArgumentException(errMsg);
                            //specific argument does not meet requirements
                            if (isInvalidArg)
                                throw new ArgumentOutOfRangeException("", errMsg);
                            //argument is null/empty
                            else if (errMsg.Contains("cannot be null or empty"))
                                throw new ArgumentNullException("", errMsg);
                            //argument casting error
                            else if (errMsg.Contains("value cannot be converted"))
                                throw new InvalidDBCastException(errMsg);
                            //general
                            else
                                throw new InvalidOperationException(errMsg);
                        }
                        //invalid authentication/authorization
                        else if (errCode == "403:")
                        {
                            throw new AccessForbiddenException(errMsg);
                        }
                        //not found
                        else if (errCode == "404:")
                        {
                            throw new KeyNotFoundException(errMsg);
                        }
                        //duplicate conflict
                        else if (errCode == "409:")
                        {
                            throw new DuplicateNameException(errMsg);
                        }
                        //parent gone
                        else if (errCode == "410:")
                        {
                            throw new EntityGoneException(errMsg);
                        }
                        //invalid file type
                        else if (errCode == "415:")
                        {
                            throw new InvalidOperationException(errMsg);
                        }
                        //throw original exception
                        else
                        {
                            throw;
                        }

                    }
                }
            }
        }



        /// <summary>
        /// Attempts to perform simple cast from DB type to CLR type for a single column of values
        /// </summary>
        /// <param name="SqlConn">The DB connection string being used</param>
        /// <param name="CmdName">The name of the DB sProc being called</param>
        /// <param name="SetParams">A function to set parameters for the SQL cmd before the DB call</param>
        /// <param name="InitQuery">Specify custom method to initiate SQL query</param>
        /// <returns></returns>
        /// <exception cref="SystemDisabledException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidDBCastException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="AccessForbiddenException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <exception cref="DuplicateNameException"/>
        /// <exception cref="EntityGoneException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="SqlException"/>
        /// <exception cref="ObjectDisposedException"/>
        public static async Task<IEnumerable<T>> ExecBasicQueryAsync<T>(SqlConfig SqlConn, string CmdName, Action<SqlCommand> SetParams = null)
        {

            using (var conn = new SqlConnection(SqlConn))
            {
                conn.Credential = SqlConn.UserCredential;
                conn.Open();
                using (var cmd = new SqlCommand(CmdName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;

                    SetParams?.Invoke(cmd);


                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var returnSet = new ConcurrentBag<T>();

                            while (await reader.ReadAsync())
                            {
                                returnSet.Add((T)(dynamic)reader[0]);
                            }

                            return returnSet;
                        }

                    }
                    catch (Exception ex)
                    {
                        var errCode = ex.Message.Substring(0, 4);
                        var errMsg = ex.Message.Substring(ex.Message.IndexOf(": ") + 2);

                        if (errCode == "400:")
                        {
                            var isInvalidArg = RgxCompiled.DataInterop.INVALID_ARGUMENT_B.IsMatch(errMsg);


                            //general argument exception
                            if (errMsg.Contains("Invalid request arguments"))
                                throw new ArgumentException(errMsg);
                            //specific argument does not meet requirements
                            if (isInvalidArg)
                                throw new ArgumentOutOfRangeException("", errMsg);
                            //argument is null/empty
                            else if (errMsg.Contains("cannot be null or empty"))
                                throw new ArgumentNullException("", errMsg);
                            //argument casting error
                            else if (errMsg.Contains("value cannot be converted"))
                                throw new InvalidDBCastException(errMsg);
                            //general
                            else
                                throw new InvalidOperationException(errMsg);
                        }
                        //invalid authentication/authorization
                        else if (errCode == "403:")
                        {
                            throw new AccessForbiddenException(errMsg);
                        }
                        //not found
                        else if (errCode == "404:")
                        {
                            throw new KeyNotFoundException(errMsg);
                        }
                        //duplicate conflict
                        else if (errCode == "409:")
                        {
                            throw new DuplicateNameException(errMsg);
                        }
                        //parent gone
                        else if (errCode == "410:")
                        {
                            throw new EntityGoneException(errMsg);
                        }
                        //invalid file type
                        else if (errCode == "415:")
                        {
                            throw new InvalidOperationException(errMsg);
                        }
                        //throw original exception
                        else
                        {
                            throw;
                        }

                    }
                }
            }
        }


        /// <summary>
        /// Attempts to perform caller defined complex conversion from DB type to arbitrary object type for multiple records
        /// </summary>
        /// <param name="SqlConn">The DB connection string being used</param>
        /// <param name="CmdName">The name of the DB sProc being called</param>
        /// <param name="ReturnValParseFunc">Function used to parse object data from SQL return set</param>
        /// <param name="InitQuery">Specify custom method to initiate SQL query</param>
        /// <returns></returns>
        /// <exception cref="SystemDisabledException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidDBCastException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="AccessForbiddenException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <exception cref="DuplicateNameException"/>
        /// <exception cref="EntityGoneException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="SqlException"/>
        /// <exception cref="ObjectDisposedException"/>
        public static async Task<IEnumerable<T>> ExecAdvQueryAsync<T>(SqlConfig SqlConn, string CmdName, Func<SqlDataReader, T> ReturnValParseFunc)
        {

            using (var conn = new SqlConnection(SqlConn))
            {
                conn.Credential = SqlConn.UserCredential;
                conn.Open();
                using (var cmd = new SqlCommand(CmdName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;


                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var returnSet = new ConcurrentBag<T>();

                            while (await reader.ReadAsync())
                            {
                                returnSet.Add(ReturnValParseFunc(reader));
                            }

                            return returnSet;
                        }

                    }
                    catch (Exception ex)
                    {

                        var errCode = ex.Message.Substring(0, 4);
                        var errMsg = ex.Message.Substring(ex.Message.IndexOf(": ") + 2);

                        if (errCode == "400:")
                        {
                            var isInvalidArg = RgxCompiled.DataInterop.INVALID_ARGUMENT_B.IsMatch(errMsg);

                            //general argument exception
                            if (errMsg.Contains("Invalid request arguments"))
                                throw new ArgumentException(errMsg);
                            //specific argument does not meet requirements
                            if (isInvalidArg)
                                throw new ArgumentOutOfRangeException("", errMsg);
                            //argument is null/empty
                            else if (errMsg.Contains("cannot be null or empty"))
                                throw new ArgumentNullException("", errMsg);
                            //argument casting error
                            else if (errMsg.Contains("value cannot be converted"))
                                throw new InvalidDBCastException(errMsg);
                            //general
                            else
                                throw new InvalidOperationException(errMsg);
                        }
                        //invalid authentication/authorization
                        else if (errCode == "403:")
                        {
                            throw new AccessForbiddenException(errMsg);
                        }
                        //not found
                        else if (errCode == "404:")
                        {
                            throw new KeyNotFoundException(errMsg);
                        }
                        //duplicate conflict
                        else if (errCode == "409:")
                        {
                            throw new DuplicateNameException(errMsg);
                        }
                        //parent gone
                        else if (errCode == "410:")
                        {
                            throw new EntityGoneException(errMsg);
                        }
                        //invalid file type
                        else if (errCode == "415:")
                        {
                            throw new InvalidOperationException(errMsg);
                        }
                        //throw original exception
                        else
                        {
                            throw;
                        }

                    }
                }
            }
        }



        /// <summary>
        /// Attempts to perform caller defined complex conversion from DB type to arbitrary object type for multiple records
        /// </summary>
        /// <param name="SqlConn">The DB connection string being used</param>
        /// <param name="CmdName">The name of the DB sProc being called</param>
        /// <param name="SetParams">A function to set parameters for the SQL cmd before the DB call</param>
        /// <param name="ReturnValParseFunc">Function used to parse object data from SQL return set</param>
        /// <param name="InitQuery">Specify custom method to initiate SQL query</param>
        /// <returns></returns>
        /// <exception cref="SystemDisabledException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidDBCastException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="AccessForbiddenException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <exception cref="DuplicateNameException"/>
        /// <exception cref="EntityGoneException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="SqlException"/>
        /// <exception cref="ObjectDisposedException"/>
        public static async Task<IEnumerable<T>> ExecAdvQueryAsync<T>(SqlConfig SqlConn, string CmdName, Action<SqlCommand> SetParams, Func<SqlDataReader, T> ReturnValParseFunc)
        {

            using (var conn = new SqlConnection(SqlConn))
            {
                conn.Credential = SqlConn.UserCredential;
                conn.Open();
                using (var cmd = new SqlCommand(CmdName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;

                    SetParams?.Invoke(cmd);



                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var returnSet = new ConcurrentBag<T>();

                            while (await reader.ReadAsync())
                            {
                                returnSet.Add(ReturnValParseFunc(reader));
                            }

                            return returnSet;
                        }

                    }
                    catch (Exception ex)
                    {

                        var errCode = ex.Message.Substring(0, 4);
                        var errMsg = ex.Message.Substring(ex.Message.IndexOf(": ") + 2);

                        if (errCode == "400:")
                        {
                            var isInvalidArg = RgxCompiled.DataInterop.INVALID_ARGUMENT_B.IsMatch(errMsg);

                            //general argument exception
                            if (errMsg.Contains("Invalid request arguments"))
                                throw new ArgumentException(errMsg);
                            //specific argument does not meet requirements
                            if (isInvalidArg)
                                throw new ArgumentOutOfRangeException("", errMsg);
                            //argument is null/empty
                            else if (errMsg.Contains("cannot be null or empty"))
                                throw new ArgumentNullException("", errMsg);
                            //argument casting error
                            else if (errMsg.Contains("value cannot be converted"))
                                throw new InvalidDBCastException(errMsg);
                            //general
                            else
                                throw new InvalidOperationException(errMsg);
                        }
                        //invalid authentication/authorization
                        else if (errCode == "403:")
                        {
                            throw new AccessForbiddenException(errMsg);
                        }
                        //not found
                        else if (errCode == "404:")
                        {
                            throw new KeyNotFoundException(errMsg);
                        }
                        //duplicate conflict
                        else if (errCode == "409:")
                        {
                            throw new DuplicateNameException(errMsg);
                        }
                        //parent gone
                        else if (errCode == "410:")
                        {
                            throw new EntityGoneException(errMsg);
                        }
                        //invalid file type
                        else if (errCode == "415:")
                        {
                            throw new InvalidOperationException(errMsg);
                        }
                        //throw original exception
                        else
                        {
                            throw;
                        }

                    }
                }
            }
        }




    }
}

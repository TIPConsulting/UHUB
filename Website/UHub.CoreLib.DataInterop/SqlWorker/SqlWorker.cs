using System;
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
        public static T ExecScalar<T>(SqlConfig SqlConn, string CmdName, Action<SqlCommand> SetParams)
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


                        return (T)(dynamic)cmd.ExecuteScalar();

                    }
                }
            }
            catch (SystemDisabledException)
            {
                throw;
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
        public static void ExecNonQuery(SqlConfig SqlConn, string CmdName, Action<SqlCommand> SetParams)
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

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SystemDisabledException)
            {
                throw;
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
        public static IEnumerable<T> ExecEntityQuery<T>(SqlConfig SqlConn, string CmdName, Action<SqlCommand> SetParams = null)
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


                    var hasError = false;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = cmd.ExecuteReader();
                    }
                    catch (SystemDisabledException)
                    {
                        hasError = true;
                        throw;
                    }
                    catch (Exception ex)
                    {
                        hasError = true;
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
                    finally
                    {
                        if (hasError)
                        {
                            try
                            {
                                reader.Close();
                            }
                            catch { }
                        }
                    }


                    if (!hasError)
                    {

                        while (reader.Read())
                        {
                            var newEnt = new T();
                            newEnt.LoadDataReader(reader);

                            yield return newEnt;
                        }

                        reader.Close();
                    }
                }
            }
        }


        /// <summary>
        /// Attempts to perform simple cast from DB type to CLR type for multiple values
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
        public static IEnumerable<T> ExecBasicQuery<T>(SqlConfig SqlConn, string CmdName, Action<SqlCommand> SetParams = null)
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


                    var hasError = false;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = cmd.ExecuteReader();
                    }
                    catch (SystemDisabledException)
                    {
                        hasError = true;
                        throw;
                    }
                    catch (Exception ex)
                    {
                        hasError = true;
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
                    finally
                    {
                        if (hasError)
                        {
                            try
                            {
                                reader.Close();
                            }
                            catch { }
                        }
                    }


                    if (!hasError)
                    {

                        while (reader.Read())
                        {
                            yield return (T)(dynamic)reader[0];
                        }

                        reader.Close();
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
        public static IEnumerable<T> ExecAdvQuery<T>(SqlConfig SqlConn, string CmdName, Func<SqlDataReader, T> ReturnValParseFunc)
        {

            using (var conn = new SqlConnection(SqlConn))
            {
                conn.Credential = SqlConn.UserCredential;
                conn.Open();
                using (var cmd = new SqlCommand(CmdName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;


                    var hasError = false;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = cmd.ExecuteReader();
                    }
                    catch (SystemDisabledException)
                    {
                        hasError = true;
                        throw;
                    }
                    catch (Exception ex)
                    {
                        hasError = true;
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
                    finally
                    {
                        if (hasError)
                        {
                            try
                            {
                                reader.Close();
                            }
                            catch { }
                        }
                    }


                    if (!hasError)
                    {

                        while (reader.Read())
                        {
                            yield return ReturnValParseFunc(reader);
                        }

                        reader.Close();
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
        public static IEnumerable<T> ExecAdvQuery<T>(SqlConfig SqlConn, string CmdName, Action<SqlCommand> SetParams, Func<SqlDataReader, T> ReturnValParseFunc)
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


                    var hasError = false;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = cmd.ExecuteReader();
                    }
                    catch (SystemDisabledException)
                    {
                        hasError = true;
                        throw;
                    }
                    catch (Exception ex)
                    {
                        hasError = true;
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
                    finally
                    {
                        if (hasError)
                        {
                            try
                            {
                                reader.Close();
                            }
                            catch { }
                        }
                    }


                    if (!hasError)
                    {

                        while (reader.Read())
                        {
                            yield return ReturnValParseFunc(reader);
                        }

                        reader.Close();
                    }
                }
            }
        }


        /// <summary>
        /// Wrap a common protocol for performing DB lookups against sProcs.  Allows single DB call to handle multiple return sets.
        /// </summary>
        /// <param name="SqlConn">The DB connection string being used</param>
        /// <param name="CmdName">The name of the DB sProc being called (alt: Sql cmd text; must set cmdType=text)</param>
        /// <param name="SetParams">A function to set parameters for the SQL cmd before the DB call</param>
        /// <param name="ReturnValParseFunc">
        ///<para>A dictionary for processing SQL return sets</para>
        ///     <para>Key: Select data set by index - useful for using SQL sProcs where multiple data sets are returned</para>
        ///     <para>Value: Function used to parse data from SQL sets and return objects based on specified rule set</para>
        /// </param>
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
        public static void ExecMultiQuery<T>(SqlConfig SqlConn, string CmdName, Action<SqlCommand> SetParams, Dictionary<int, Func<DataRow, T>> ReturnValParseFunc, bool AllowDBMultithreading, bool AllowMultithreading = false)
        {
            var lockObject = new object();

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


                        //ADVANCED QUERY
                        //DOES NOT RETURN DATA SET
                        //CLIENT MUST PROCESS OUTPUT IN THE PARSE DICTIONARY

                        using (var adap = new SqlDataAdapter(cmd))
                        {
                            using (var ds = new DataSet())
                            {
                                adap.Fill(ds);


                                if (AllowDBMultithreading)
                                {

                                    Parallel.ForEach(ReturnValParseFunc.Keys, (key) =>
                                    {
                                        foreach (var row in ds.Tables[key].Rows.Cast<DataRow>())
                                        {
                                            ReturnValParseFunc[key](row);
                                        }
                                    });

                                }
                                else
                                {
                                    foreach (var key in ReturnValParseFunc.Keys)
                                    {
                                        foreach (var row in ds.Tables[key].Rows.Cast<DataRow>())
                                        {
                                            ReturnValParseFunc[key](row);
                                        }
                                    }
                                }
                            }
                        }

                    }
                }

            }
            catch (SystemDisabledException)
            {
                throw;
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
                //reset stack trace
                else
                {
                    throw;
                }
            }
        }
    }
}

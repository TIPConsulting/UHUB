using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace UHub.CoreLib.DataInterop
{
    /// <summary>
    /// Configuration tool to simplify the construction of connection strings.
    /// </summary>
    public sealed class SqlConfig
    {

        private bool? _isValid = null;
        private string _connectionString = null;


        /// <summary>
        /// Create context connection for sql CLR procs
        /// </summary>
        private bool IsContextConnection { get; } = false;

        /// <summary>
        /// Server name
        /// </summary>
        private string Server { get; } = "";

        /// <summary>
        /// Database name
        /// </summary>
        private string Database { get; } = "";


        /// <summary>
        /// Flag to determine whether the connection should allow async processing
        /// </summary>
        private bool EnableAsync { get; } = true;

        /// <summary>
        /// Flag to determine if integrated security should be used or SQL authentication
        /// </summary>
        private bool EnableIntegratedSecurity { get; } = true;

        /// <summary>
        /// Connection username for SQL auth
        /// </summary>
        private string Username { get; } = "";

        /// <summary>
        /// Connection password for SQL auth
        /// </summary>
        private string Password { get; } = "";


        public SqlCredential UserCredential { get; } = null;


        /// <summary>
        /// Specify SQL connection timeout
        /// </summary>
        private int ConnectionTimeout { get; } = 10;



        /// <summary>
        /// Create new Context Connection
        /// </summary>
        /// <param name="Server">SQL server name or IP</param>
        /// <param name="Database">SQL DB name</param>
        /// <param name="UseIntegratedSecurity"></param>
        public SqlConfig(bool EnableAsync, int Timeout = 10)
        {
            this.EnableAsync = EnableAsync;
            this.IsContextConnection = true;
        }
        /// <summary>
        /// Create new server connection using default database and LDAP credentials
        /// </summary>
        /// <param name="Server">SQL server name or IP</param>
        /// <param name="Database">SQL DB name</param>
        /// <param name="UseIntegratedSecurity"></param>
        public SqlConfig(string Server, bool EnableAsync, int Timeout = 10)
        {
            this.Server = Server;
            this.EnableAsync = EnableAsync;
            this.EnableIntegratedSecurity = true;

        }
        /// <summary>
        /// Create new server connection using LDAP credentials
        /// </summary>
        /// <param name="Server">SQL server name or IP</param>
        /// <param name="Database">SQL DB name</param>
        /// <param name="UseIntegratedSecurity"></param>
        public SqlConfig(string Server, string Database, bool EnableAsync, int Timeout = 10)
        {
            this.Server = Server;
            this.Database = Database;
            this.EnableAsync = EnableAsync;
            this.EnableIntegratedSecurity = true;
        }
        /// <summary>
        /// Create new server connection using specified credentials
        /// </summary>
        /// <param name="Server">SQL server name or IP</param>
        /// <param name="Database">SQL DB name</param>
        /// <param name="Username">SQL username</param>
        /// <param name="Password">SQL user password</param>
        public SqlConfig(string Server, string Database, bool EnableAsync, string Username, string Password, int Timeout = 10)
        {
            this.Server = Server;
            this.Database = Database;
            this.EnableAsync = EnableAsync;
            this.EnableIntegratedSecurity = false;
            this.Username = Username;
            this.Password = Password;
        }
        /// <summary>
        /// Create new server connection using specified credentials
        /// </summary>
        /// <param name="Server">SQL server name or IP</param>
        /// <param name="Database">SQL DB name</param>
        /// <param name="Username">SQL username</param>
        /// <param name="Password">SQL user password</param>
        public SqlConfig(string Server, string Database, bool EnableAsync, SqlCredential UserCredential, int Timeout = 10)
        {
            this.Server = Server;
            this.Database = Database;
            this.EnableAsync = EnableAsync;
            this.EnableIntegratedSecurity = false;
            this.UserCredential = UserCredential;
        }
        /// <summary>
        /// Initialiazer
        /// </summary>
        /// <param name="ConnectionString"></param>
        public SqlConfig(string ConnectionString)
        {
            this._isValid = true;
            this._connectionString = ConnectionString;
        }
        /// <summary>
        /// Initialiazer
        /// </summary>
        /// <param name="ConnectionString"></param>
        public SqlConfig(string ConnectionString, SqlCredential UserCredential)
        {
            this._isValid = true;
            this._connectionString = ConnectionString;
            this.UserCredential = UserCredential;
        }
        /// <summary>
        /// Create copy of existing SQL config
        /// </summary>
        /// <param name="ConfigBuilder"></param>
        public SqlConfig(SqlConfigBuilder ConfigBuilder)
        {
            this.IsContextConnection = ConfigBuilder.IsContextConnection;
            this.Server = ConfigBuilder.Server;
            this.Database = ConfigBuilder.Database;
            this.EnableAsync = ConfigBuilder.EnableAsync;

            this.EnableIntegratedSecurity = ConfigBuilder.EnableIntegratedSecurity;
            if (ConfigBuilder.UserCredential != null)
            {
                this.UserCredential = ConfigBuilder.UserCredential;
            }
            else
            {
                this.Username = ConfigBuilder.Username;
                this.Password = ConfigBuilder.Password;
            }

            this.ConnectionTimeout = ConfigBuilder.ConnectionTimeout;
        }
        /// <summary>
        /// Create copy of existing SQL config
        /// </summary>
        /// <param name="Config"></param>
        internal SqlConfig(SqlConfig Config)
        {
            if (string.IsNullOrWhiteSpace(Config._connectionString))
            {
                this.IsContextConnection = Config.IsContextConnection;
                this.Server = Config.Server;
                this.Database = Config.Database;
                this.EnableAsync = Config.EnableAsync;
                this.EnableIntegratedSecurity = Config.EnableIntegratedSecurity;
                this.Username = Config.Username;
                this.Password = Config.Password;
                this.UserCredential = Config.UserCredential;
                this.ConnectionTimeout = Config.ConnectionTimeout;
            }
            else
            {
                this._connectionString = Config._connectionString;
            }
            this._isValid = Config._isValid;
        }



        /// <summary>
        /// Get full connection string for DB connection
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_connectionString != null)
            {
                return _connectionString;
            }

            return GetConnectionString();
        }

        /// <summary>
        /// Auto cast SqlConfig to string for DB connection
        /// </summary>
        /// <seealso cref="SqlConfig.ToString()"/>
        /// <param name="config"></param>
        public static implicit operator string(SqlConfig config)
        {
            return config.ToString();
        }




        /// <summary>
        /// Ensure that all properties are valid
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private bool IsValid()
        {
            if (_isValid != null)
            {
                return _isValid.Value;
            }


            if (IsContextConnection)
            {
                return true;
            }


            if (string.IsNullOrWhiteSpace(Server))
            {
                _isValid = false;
                throw new ArgumentException("Server cannot be null or empty");
            }

            if (!EnableIntegratedSecurity)
            {
                if (UserCredential == null && string.IsNullOrWhiteSpace(Username))
                {
                    _isValid = false;
                    throw new ArgumentException("Username cannot be null or empty");
                }

            }



            _isValid = true;
            return true;
        }


        /// <summary>
        /// Get connection string from properties
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        private string GetConnectionString()
        {
            if (_connectionString != null)
            {
                return _connectionString;
            }

            if (!IsValid())
            {
                throw new InvalidOperationException("Cannot get connection string from invalid config");
            }

            if (IsContextConnection)
            {
                return "context connection=true";
            }


            StringBuilder builder = new StringBuilder();

            //server
            builder.AppendFormat("Server={0};", Server);
            //database
            builder.AppendFormat("Database={0};", Database);
            //Async
            if (EnableAsync)
            {
                builder.Append("Asynchronous Processing=True;");
            }

            //IntegratedSecurity
            if (EnableIntegratedSecurity)
            {
                builder.Append("Integrated Security=SSPI;");
            }
            //username
            if (!string.IsNullOrWhiteSpace(Username))
            {
                string uidAdj;
                if (Username.Contains(";"))
                {
                    if (Username.StartsWith("'"))
                    {
                        uidAdj = "\"" + Username + "\"";
                    }
                    else
                    {
                        uidAdj = "'" + Username + "'";
                    }
                }
                else
                {
                    uidAdj = Username;
                }


                builder.AppendFormat("Uid={0};", uidAdj);
            }
            //password
            if (!string.IsNullOrEmpty(Password))
            {
                string psdAdj;
                if (Password.Contains(";"))
                {
                    if (Password.StartsWith("'"))
                    {
                        psdAdj = "\"" + Password + "\"";
                    }
                    else
                    {
                        psdAdj = "'" + Password + "'";
                    }
                }
                else
                {
                    psdAdj = Password;
                }

                builder.AppendFormat("Pwd={0};", psdAdj);
            }

            builder.AppendFormat("Connection Timeout={0};", ConnectionTimeout);

            _connectionString = builder.ToString();
            return _connectionString;
        }

        /// <summary>
        /// Test connection to ensure that the DB can be accessed
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public bool ValidateConnection()
        {
            string connection = GetConnectionString();

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Credential = this.UserCredential;
                    conn.Open(); // throws if invalid
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

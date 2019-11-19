using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace UHub.CoreLib.DataInterop
{
    /// <summary>
    /// Configuration tool to simplify the construction of connection strings
    /// </summary>
    public sealed class SqlConfigBuilder
    {
        /// <summary>
        /// Create context connection for SQL CLR proc
        /// <para></para>
        /// Default: false
        /// </summary>
        public bool IsContextConnection { get; set; } = false;



        /// <summary>
        /// Server name
        /// </summary>
        public string Server { get; set; }



        /// <summary>
        /// Database name
        /// </summary>
        public string Database { get; set; }



        /// <summary>
        /// Flag to determine whether the connection should allow async processing
        /// <para></para>
        /// Default: true
        /// </summary>
        public bool EnableAsync { get; set; } = true;



        /// <summary>
        /// Flag to determine if integrated security should be used or SQL authentication
        /// <para></para>
        /// Default: true
        /// </summary>
        public bool EnableIntegratedSecurity { get; set; } = true;



        /// <summary>
        /// Connection username for SQL auth
        /// </summary>
        public string Username { get; set; }



        /// <summary>
        /// Connection password for SQL auth
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// Secure credential option for SQL auth. Supercedes plaintext creds if both are set
        /// </summary>
        public SqlCredential UserCredential { get; set; }


        /// <summary>
        /// Specify SQL connection timeout
        /// <para></para>
        /// Default: 10
        /// </summary>
        public int ConnectionTimeout { get; set; } = 10;

    }
}

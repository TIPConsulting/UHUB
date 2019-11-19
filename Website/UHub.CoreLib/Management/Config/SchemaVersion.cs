using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;

namespace UHub.CoreLib.Management.Config
{

    /// <summary>
    /// CMS DB schema validation
    /// </summary>
    public struct SchemaVersion
    {
        /// <summary>
        /// Schema version for CMS DB user objects
        /// </summary>
        public decimal UserVersion { get; private set; }
        /// <summary>
        /// Schema version for CMS DB entity objects
        /// </summary>
        public decimal EntityVersion { get; private set; }
        /// <summary>
        /// Schema version for CMS DB general interface members
        /// </summary>
        public decimal InterfaceVersion { get; private set; }
        /// <summary>
        /// Schema version for CMS DB authorization systems
        /// </summary>
        public decimal AuthVersion { get; private set; }
        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="UserVersion"></param>
        /// <param name="EntityVersion"></param>
        /// <param name="InterfaceVersion"></param>
        /// <param name="AuthVersion"></param>
        public SchemaVersion(decimal UserVersion, decimal EntityVersion, decimal InterfaceVersion, decimal AuthVersion)
        {
            this.UserVersion = UserVersion;
            this.EntityVersion = EntityVersion;
            this.InterfaceVersion = InterfaceVersion;
            this.AuthVersion = AuthVersion;
        }

        /// <summary>
        /// Validate that the specified DB schema matches this schema
        /// </summary>
        /// <param name="CmsDBConfig"></param>
        /// <returns></returns>
        public bool Validate(SqlConfig CmsDBConfig)
        {
            string query =
                @"select * from [dbo].SchemaVersioning";

            decimal userVersion = -1, entVersion = -1, intfcVersion = -1, authVersion = -1;

            try
            {

                SqlWorker.ExecAdvQuery<int>(
                    CmsDBConfig,
                    query,
                    (cmd) =>
                    {
                        cmd.CommandType = CommandType.Text;
                    },
                    (reader) =>
                    {
                        var name = (string)reader["Name"];
                        var version = (decimal)reader["Version"];

                        if (name == "UserSchemaVersion")
                        {
                            userVersion = version;
                        }
                        else if (name == "EntitySchemaVersion")
                        {
                            entVersion = version;
                        }
                        else if (name == "InterfaceSchemaVersion")
                        {
                            intfcVersion = version;
                        }
                        else if (name == "AuthSchemaVersion")
                        {
                            authVersion = version;
                        }

                        return 0;
                    }).ToList();


            }
            catch
            {
                return false;
            }




            if (this.UserVersion != userVersion)
            {
                return false;
            }
            if (this.EntityVersion != entVersion)
            {
                return false;
            }
            if (this.InterfaceVersion != intfcVersion)
            {
                return false;
            }
            if (this.AuthVersion != authVersion)
            {
                return false;
            }

            return true;
        }
    }
}

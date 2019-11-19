using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SysSec = System.Web.Security;
using System.Web.Security;
using UHub.CoreLib.Tools;
using UHub.CoreLib.Tools.Extensions;


namespace UHub.CoreLib.Security.Authentication
{
    public sealed class AuthenticationToken
    {
        private const string AES_PURPOSE = "AuthTokenEncryption";
        private const short TOKEN_PART_COUNT = 9;
        private const short TOKEN_SALT_LENGTH = 8;



        public bool IsPersistent { get; private set; }
        public string TokenID { get; private set; }
        private string TokenSalt { get; set; }
        public DateTimeOffset IssueDate { get; private set; }
        public DateTimeOffset ExpirationDate { get; private set; }
        public long UserID { get; private set; }
        public int SystemVersion { get; private set; }
        public string UserVersion { get; private set; }
        public string SessionID { get; private set; }

        public AuthenticationToken(bool IsPersistent, DateTimeOffset IssueDate, DateTimeOffset ExpirationDate, long UserID, int SystemVersion, string UserVersion, string SessionID)
        {
            Random rnd = new Random();

            //get rnd ID in case multiple treads hit at same time
            //1296 --> 100
            //46655 --> ZZZ
            //46656 --> 1000
            //1679615 --> ZZZZ
            var tknIntID = rnd.Next(1296, 46655);
            var tknID = Base36.IntToString(tknIntID);
            this.TokenID = tknID.ToLower();


            string salt = SysSec.Membership.GeneratePassword(TOKEN_SALT_LENGTH, 0);
            salt = salt.Replace('|', '0');
            this.TokenSalt = salt;

            this.IsPersistent = IsPersistent;
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
            this.UserID = UserID;
            this.SystemVersion = SystemVersion;
            this.UserVersion = UserVersion;
            this.SessionID = SessionID ?? "";
        }

        private AuthenticationToken(string TokenID, string TokenSalt, bool IsPersistent, DateTimeOffset IssueDate, DateTimeOffset ExpirationDate, long UserID, int SystemVersion, string UserVersion, string SessionID)
        {
            this.TokenID = TokenID;
            this.TokenSalt = TokenSalt;
            this.IsPersistent = IsPersistent;
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
            this.UserID = UserID;
            this.SystemVersion = SystemVersion;
            this.UserVersion = UserVersion;
            this.SessionID = SessionID ?? "";
        }

        public void SlideExpiration(TimeSpan offset)
        {
            ExpirationDate = ExpirationDate.Add(offset);
        }
        public void SetExpiration(DateTimeOffset newDate)
        {
            ExpirationDate = newDate;
        }


        public string Serialize()
        {
            long issueTicks_Norm = IssueDate.UtcTicks;
            long issueTicks_Denorm = DenormalizeTicks(issueTicks_Norm);

            long expireTicks_Norm = ExpirationDate.UtcTicks;
            long expireTicks_Denorm = DenormalizeTicks(expireTicks_Norm, issueTicks_Norm);


            StringBuilder data = new StringBuilder();

            data.Append(TokenID);
            data.Append("|");
            data.Append(TokenSalt);
            data.Append("|");
            data.Append(IsPersistent ? "1" : "0");
            data.Append("|");
            data.Append(Base36.LongToString(issueTicks_Denorm));
            data.Append("|");
            data.Append(Base36.LongToString(expireTicks_Denorm));
            data.Append("|");
            data.Append(Base36.LongToString(UserID));
            data.Append("|");
            data.Append(SystemVersion);
            data.Append("|");
            data.Append(UserVersion);
            data.Append("|");
            data.Append(SessionID);

            return data.ToString();
        }

        /// <summary>
        /// Get string from AuthToken for use in cookies 
        /// <para/> Strips out data that should not be send to client
        /// </summary>
        /// <returns></returns>
        public string Encrypt()
        {
            return this.Serialize().Encrypt(AES_PURPOSE);

        }

        /// <summary>
        /// Get AuthToken from string
        /// <para/> Retrieves data that should not be sent to client
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static AuthenticationToken Decrypt(string data)
        {
            try
            {
                var plainData = data.Decrypt(AES_PURPOSE);


                var parts = plainData.Split('|');
                if (parts.Count() != TOKEN_PART_COUNT)
                {
                    throw new Exception("Invalid token data");
                }

                //ID
                string tokenId = parts[0];
                //SALT
                string tokenSalt = parts[1];
                //PERSIST
                bool persist = (parts[2] == "1");
                //ISSUE DATE
                long issueTicks_Denorm = Base36.StringToLong(parts[3]);
                long issueTicks_Norm = NormalizeTicks(issueTicks_Denorm);
                DateTimeOffset issueDate = new DateTimeOffset(issueTicks_Norm, new TimeSpan(0));
                //EXPIRE DATE
                long expireTicks_Denorm = Base36.StringToLong(parts[4]);
                long expireTicks_Norm = NormalizeTicks(expireTicks_Denorm, issueTicks_Norm);
                DateTimeOffset expirationDate = new DateTimeOffset(expireTicks_Norm, new TimeSpan(0));
                //USER ID
                long userID = Base36.StringToLong(parts[5]);
                //SYSTEM VERSION
                int systemV = int.Parse(parts[6]);
                //USER VERSION
                string userV = parts[7];
                //SESSION ID
                string sessionId = parts[8];


                return new AuthenticationToken(tokenId, tokenSalt, persist, issueDate, expirationDate, userID, systemV, userV, sessionId);
            }
            catch
            {
                throw;
            }
        }


        public string GetTokenHash(HashType HashType = HashType.HMACSHA256)
        {
            //hash all fields except Expiration date
            //this allows the client to slide the token expiration without affecting the hash
            //to maintain security, the max token lifespan is stored in the DB validator

            StringBuilder data = new StringBuilder();

            data.Append(TokenID);
            data.Append("|");
            data.Append(TokenSalt);
            data.Append("|");
            data.Append(IsPersistent);
            data.Append("|");
            data.Append(IssueDate.UtcTicks);
            data.Append("|");
            data.Append(UserID);
            data.Append("|");
            data.Append(SystemVersion);
            data.Append("|");
            data.Append(UserVersion);
            data.Append("|");
            data.Append(SessionID);


            return data.ToString().GetHash(HashType);
        }


        //JAN 1 2018 UTC
        private const long DATUM_TICKS = 636503616000000000L;
        /// <summary>
        /// Process token ticks for use as a delta from a given datum point.
        /// Allows client token to be compressed more
        /// </summary>
        /// <param name="rawTicks">The raw date tick value to adjust</param>
        /// <param name="datum">The datum used for delta generation </param>
        /// <returns></returns>
        private static long DenormalizeTicks(long rawTicks, long? datum = null)
        {
            return rawTicks - (datum ?? DATUM_TICKS);
        }
        /// <summary>
        /// Process token ticks for trueDate given delta and a datum point
        /// Allows client token to be compressed more
        /// </summary>
        /// <param name="deltaTicks">The date delta ticks</param>
        /// <param name="datum">The datum used for trueDate generation</param>
        /// <returns></returns>
        private static long NormalizeTicks(long deltaTicks, long? datum = null)
        {
            return deltaTicks + (datum ?? DATUM_TICKS);
        }


    }
}

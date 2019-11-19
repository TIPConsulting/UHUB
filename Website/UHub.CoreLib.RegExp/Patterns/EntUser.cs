using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UHub.CoreLib.RegExp.Attributes;

namespace UHub.CoreLib.RegExp
{
    partial class RgxPtrns
    {

        public static class EntUser
        {
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string USERNAME = @"\S{3,50}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string USERNAME_B = @"^\S{3,50}$";



            //ASSOC: 5F6FC523-2852-4C5A-91A0-3A3F05556594
            [RegexOptionFlag(Value = (int)(RegexOptions.Multiline | RegexOptions.IgnoreCase))]
            public const string PASSWORD = @".{8,150}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Multiline | RegexOptions.IgnoreCase))]
            public const string PASSWORD_B = @"^.{8,150}$";



            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string EMAIL = @".{3,250}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string EMAIL_B = @"^.{3,250}$";



            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string EMAIL_DOMAIN = @".{1,250}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string EMAIL_DOMAIN_B = @"^.{1,250}$";



            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string NAME = @"(([ \u00c0-\u01ffA-z'\-])+){2,200}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string NAME_B = @"^(([ \u00c0-\u01ffA-z'\-])+){2,200}$";



            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string REF_UID = @"[a-fA-F0-9]{96}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string REF_UID_B = @"^[a-fA-F0-9]{96}$";



            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string PHONE = @"([0-1][ .-])?((\([0-9]{3}\)[ .-]?)|([0-9]{3}[ .-]?))([0-9]{3}[ .-]?)([0-9]{4})";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string PHONE_B = @"^([0-1][ .-])?((\([0-9]{3}\)[ .-]?)|([0-9]{3}[ .-]?))([0-9]{3}[ .-]?)([0-9]{4})$";



            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string MAJOR = @".{2,250}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string MAJOR_B = @"^.{2,250}$";



            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string YEAR = @"Freshman|Sophomore|Junior|Senior\+?";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string YEAR_B = @"^Freshman|Sophomore|Junior|Senior\+?$";



            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string GRADE_DATE = @".{1,10}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string GRAD_DATE_B = @"^.{1,10}$";



            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string COMPANY = @".{0,100}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string COMPANY_B = @"^.{0,100}$";



            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string JOB_TITLE = @".{0,100}";
            [RegexOptionFlag(Value = (int)(RegexOptions.Singleline | RegexOptions.IgnoreCase))]
            public const string JOB_TITLE_B = @"^.{0,100}$";
        }
    }
}

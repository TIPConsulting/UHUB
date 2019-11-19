using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.RegExp;


namespace UHub.CoreLib.Entities.SchoolClubs.Management
{
    public static partial class SchoolClubManager
    {

        private static class Shared
        {
            internal static void TryCreate_HandleAttrTrim(ref SchoolClub NewClub)
            {
                NewClub.Name = NewClub.Name?.Trim();
                NewClub.Description = NewClub.Description?.Trim();
            }



            internal static SchoolClubResultCode TryCreate_ValidateClubAttrs(in SchoolClub NewClub)
            {

                //Validate Content
                if (NewClub.Name.IsEmpty())
                {
                    return SchoolClubResultCode.NameEmpty;
                }
                if (!NewClub.Name.RgxIsMatch(RgxPtrns.EntSchoolClub.NAME_B, RegexOptions.Singleline))
                {
                    return SchoolClubResultCode.NameInvalid;
                }


                //validate description
                if(NewClub.Description.IsNotEmpty())
                {
                    if(!NewClub.Description.RgxIsMatch(RgxPtrns.EntSchoolClub.DESCRIPTION_B, RegexOptions.Multiline))
                    {
                        return SchoolClubResultCode.DescriptionInvalid;
                    }
                }


                return SchoolClubResultCode.Success;
            }


            internal static void TryCreate_AttrConversionHandler(ref SchoolClub NewClub)
            {

                var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
                if ((sanitizerMode & HtmlSanitizerMode.OnWrite) != 0)
                {
                    NewClub.Description = NewClub.Description?.SanitizeHtml().HtmlDecode();
                }

            }

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Management;
using UHub.CoreLib.Security;
using UHub.CoreLib.RegExp;


namespace UHub.CoreLib.Entities.SchoolMajors.Management
{
#pragma warning disable 612,618

    public static partial class SchoolMajorManager
    {
        private static class Shared
        {

            internal static void TryCreate_HandleAttrTrim(ref SchoolMajor NewMajor)
            {
                NewMajor.Name = NewMajor.Name?.Trim();
                NewMajor.Description = NewMajor.Description?.Trim();
            }



            internal static SchoolMajorResultCode TryCreate_ValidateMajorAttrs(in SchoolMajor NewMajor)
            {

                //Validate Content
                if (NewMajor.Name.IsEmpty())
                {
                    return SchoolMajorResultCode.NameEmpty;
                }
                if (!NewMajor.Name.RgxIsMatch(RgxPtrns.EntSchoolClub.NAME_B, RegexOptions.Singleline))
                {
                    return SchoolMajorResultCode.NameInvalid;
                }


                //validate description
                if (NewMajor.Description.IsNotEmpty())
                {
                    if (!NewMajor.Description.RgxIsMatch(RgxPtrns.EntSchoolClub.DESCRIPTION_B))
                    {
                        return SchoolMajorResultCode.DescriptionInvalid;
                    }
                }


                return SchoolMajorResultCode.Success;
            }


            internal static void TryCreate_AttrConversionHandler(ref SchoolMajor NewMajor)
            {

                var sanitizerMode = CoreFactory.Singleton.Properties.HtmlSanitizerMode;
                if ((sanitizerMode & HtmlSanitizerMode.OnWrite) != 0)
                {
                    NewMajor.Description = NewMajor.Description?.SanitizeHtml().HtmlDecode();
                }

            }

        }
    }
#pragma warning restore

}


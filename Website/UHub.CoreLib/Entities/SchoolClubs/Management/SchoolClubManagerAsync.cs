using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.SchoolClubs.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.SchoolClubs.Management
{
#pragma warning disable 612,618

    public static partial class SchoolClubManager
    {
        public static async Task<(long? ClubID, SchoolClubResultCode ResultCode)> TryCreateClubAsync(SchoolClub NewClub)
        {
            if (NewClub == null)
            {
                return (null, SchoolClubResultCode.NullArgument);
            }



            Shared.TryCreate_HandleAttrTrim(ref NewClub);

            Shared.TryCreate_AttrConversionHandler(ref NewClub);

            var attrValidateCode = Shared.TryCreate_ValidateClubAttrs(NewClub);
            if (attrValidateCode != 0)
            {
                return (null, attrValidateCode);
            }



            long? id = null;
            try
            {
                id = await SchoolClubWriter.CreateClubAsync(NewClub);
            }
            catch (ArgumentOutOfRangeException)
            {
                return (null, SchoolClubResultCode.InvalidArgument);
            }
            catch (ArgumentNullException)
            {
                return (null, SchoolClubResultCode.NullArgument);
            }
            catch (ArgumentException)
            {
                return (null, SchoolClubResultCode.InvalidArgument);
            }
            catch (InvalidCastException)
            {
                return (null, SchoolClubResultCode.InvalidArgumentType);
            }
            catch (InvalidOperationException)
            {
                return (null, SchoolClubResultCode.InvalidOperation);
            }
            catch (AccessForbiddenException)
            {
                return (null, SchoolClubResultCode.AccessDenied);
            }
            catch (EntityGoneException)
            {
                return (null, SchoolClubResultCode.InvalidOperation);
            }
            catch (Exception ex)
            {
                var exID = new Guid("45CB4726-3D28-4D65-A0FE-AB53EFA3C705");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return (null, SchoolClubResultCode.UnknownError);
            }





            if (id == null)
            {
                return (null, SchoolClubResultCode.UnknownError);
            }
            return (id, SchoolClubResultCode.Success);

        }

    }
#pragma warning restore

}

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
        public static (long? ClubID, SchoolClubResultCode ResultCode) TryCreateClub(SchoolClub NewClub)
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
                id = SchoolClubWriter.CreateClub(NewClub);
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
                var exID = new Guid("196DC520-0ADE-4B16-AF2E-E33A49F9CD73");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
                return (null, SchoolClubResultCode.UnknownError);
            }




            if (id == null)
            {
                return (id, SchoolClubResultCode.UnknownError);
            }
            return (id, SchoolClubResultCode.Success);

        }

    }
#pragma warning restore

}

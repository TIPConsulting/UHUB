using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.SchoolMajors.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.SchoolMajors.Management
{
#pragma warning disable 612,618

    public static partial class SchoolMajorManager
    {
        public static async Task<(long? ClubID, SchoolMajorResultCode ResultCode)> TryCreateMajorAsync(SchoolMajor NewMajor, long ParentID)
        {
            if (NewMajor == null)
            {
                return (null, SchoolMajorResultCode.NullArgument);
            }



            Shared.TryCreate_HandleAttrTrim(ref NewMajor);

            Shared.TryCreate_AttrConversionHandler(ref NewMajor);

            var attrValidateCode = Shared.TryCreate_ValidateMajorAttrs(NewMajor);
            if (attrValidateCode != 0)
            {
                return (null, attrValidateCode);
            }



            long? id = null;
            try
            {
                id = await SchoolMajorWriter.CreateSchoolMajorAsync(NewMajor, ParentID);
            }
            catch (ArgumentOutOfRangeException)
            {
                return (null, SchoolMajorResultCode.InvalidArgument);
            }
            catch (ArgumentNullException)
            {
                return (null, SchoolMajorResultCode.NullArgument);
            }
            catch (ArgumentException)
            {
                return (null, SchoolMajorResultCode.InvalidArgument);
            }
            catch (InvalidCastException)
            {
                return (null, SchoolMajorResultCode.InvalidArgumentType);
            }
            catch (InvalidOperationException)
            {
                return (null, SchoolMajorResultCode.InvalidOperation);
            }
            catch (AccessForbiddenException)
            {
                return (null, SchoolMajorResultCode.AccessDenied);
            }
            catch (EntityGoneException)
            {
                return (null, SchoolMajorResultCode.InvalidOperation);
            }
            catch (Exception ex)
            {
                var exID = new Guid("B19909EB-E344-4A45-97C1-D8A504950D70");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return (null, SchoolMajorResultCode.UnknownError);
            }




            if (id == null)
            {
                return (id, SchoolMajorResultCode.UnknownError);
            }
            return (id, SchoolMajorResultCode.Success);

        }

    }
#pragma warning restore

}

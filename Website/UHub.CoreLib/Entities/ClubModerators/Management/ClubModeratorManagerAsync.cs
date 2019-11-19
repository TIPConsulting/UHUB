using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Entities.ClubModerators.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.ClubModerators.Management
{
#pragma warning disable 612, 618
    public static partial class ClubModeratorManager
    {
        public static async Task<(long? ClubModID, ClubModeratorResultCode ResultCode)> TryCreateClubModeratorAsync(ClubModerator NewModerator, long ParentID)
        {
            if (NewModerator == null)
            {
                return (null, ClubModeratorResultCode.NullArgument);
            }


            long? id = null;
            try
            {
                id = await ClubModeratorWriter.CreateClubModeratorAsync(NewModerator, ParentID);
            }
            catch (ArgumentOutOfRangeException)
            {
                return (null, ClubModeratorResultCode.InvalidArgument);
            }
            catch (ArgumentNullException)
            {
                return (null, ClubModeratorResultCode.NullArgument);
            }
            catch (ArgumentException)
            {
                return (null, ClubModeratorResultCode.InvalidArgument);
            }
            catch (InvalidCastException)
            {
                return (null, ClubModeratorResultCode.InvalidArgumentType);
            }
            catch (InvalidOperationException)
            {
                return (null, ClubModeratorResultCode.InvalidOperation);
            }
            catch (AccessForbiddenException)
            {
                return (null, ClubModeratorResultCode.AccessDenied);
            }
            catch (EntityGoneException)
            {
                return (null, ClubModeratorResultCode.InvalidOperation);
            }
            catch (Exception ex)
            {
                var exID = new Guid("169CBE64-325B-4820-9E21-833B1BC8DE4D");
                await CoreFactory.Singleton.Logging.CreateErrorLogAsync(ex, exID);
                return (null, ClubModeratorResultCode.UnknownError);
            }




            if (id == null)
            {
                return (id, ClubModeratorResultCode.UnknownError);
            }

            return (id, ClubModeratorResultCode.Success);
        }

    }
#pragma warning restore

}

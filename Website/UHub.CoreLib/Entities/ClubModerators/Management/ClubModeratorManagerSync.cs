using System;
using System.Collections.Generic;
using System.Data;
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
        public static (long? ClubModID, ClubModeratorResultCode ResultCode) TryCreateClubModerator(ClubModerator NewModerator, long ParentID)
        {
            if (NewModerator == null)
            {
                return (null, ClubModeratorResultCode.NullArgument);
            }


            long? id = null;
            try
            {
                id = ClubModeratorWriter.CreateClubModerator(NewModerator, ParentID);
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
            catch(EntityGoneException)
            {
                return (null, ClubModeratorResultCode.InvalidOperation);
            }
            catch (Exception ex)
            {
                var exID = new Guid("BA84325E-26A2-4BB2-892B-19DCE9704F29");
                CoreFactory.Singleton.Logging.CreateErrorLog(ex, exID);
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

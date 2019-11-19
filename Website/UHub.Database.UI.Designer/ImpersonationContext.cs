using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace UHub.Database.UI.Designer
{

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public sealed class ImpersonationContext : IDisposable
    {
        private readonly SafeTokenHandle _handle;
        private readonly WindowsImpersonationContext _context;

        

        /// <summary>
        /// Attempts to impersonate the user with the supplied information.
        /// Call from a <c>using</c> block, or ensure that <see cref="Dispose"/> is called on
        /// the resulting <see cref="ImpersonationContext"/> object upon completion.
        /// </summary>
        /// <param name="domain">The domain name or machine name, or <c>.</c> for the local machine.</param>
        /// <param name="username">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="logonType">The logon type.</param>
        /// <returns>An <see cref="ImpersonationContext"/> object, which should be disposed when done impersonating the user.</returns>
        public static ImpersonationContext CreateContext(string domain, string username, string password, LogonType logonType)
        {
            return new ImpersonationContext(domain, username, password, logonType);
        }

        /// <summary>
        /// Attempts to impersonate the user with the supplied information.
        /// Call from a <c>using</c> block, or ensure that <see cref="Dispose"/> is called on
        /// the resulting <see cref="ImpersonationContext"/> object upon completion.
        /// </summary>
        /// <param name="domain">The domain name or machine name, or <c>.</c> for the local machine.</param>
        /// <param name="username">The user name.</param>
        /// <param name="password">The password, as a <see cref="SecureString"/>.</param>
        /// <param name="logonType">The logon type.</param>
        /// <returns>An <see cref="ImpersonationContext"/> object, which should be disposed when done impersonating the user.</returns>
        public static ImpersonationContext CreateContext(string domain, string username, SecureString password, LogonType logonType)
        {
            return new ImpersonationContext(domain, username, password, logonType);
        }

        private ImpersonationContext(string domain, string username, SecureString password, LogonType logonType)
        {
            IntPtr token;
            IntPtr passPtr = Marshal.SecureStringToGlobalAllocUnicode(password);

            bool success;
            try
            {
                success = NativeMethods.LogonUser(username, domain, passPtr, (int)logonType, 0, out token);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(passPtr);
            }

            CompleteImpersonation(success, token, out _handle, out _context);
        }

        private ImpersonationContext(string domain, string username, string password, LogonType logonType)
        {
            IntPtr token;
            bool success = NativeMethods.LogonUser(username, domain, password, (int)logonType, 0, out token);
            CompleteImpersonation(success, token, out _handle, out _context);
        }

        private void CompleteImpersonation(bool success, IntPtr token, out SafeTokenHandle handle, out WindowsImpersonationContext context)
        {
            if (!success)
            {
                var errorCode = Marshal.GetLastWin32Error();

                if (token != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(token);
                }

                //throw new ImpersonationException(new Win32Exception(errorCode));
            }

            handle = new SafeTokenHandle(token);
            context = WindowsIdentity.Impersonate(_handle.DangerousGetHandle());
        }

        /// <summary>
        /// Disposes the <see cref="ImpersonationContext"/> object, ending impersonation and restoring the original user.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
            _handle.Dispose();
        }
    }

    /// <summary>
    /// Specifies the type of login used.
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa378184.aspx
    /// </summary>
    public enum LogonType
    {
        /// <summary>
        /// This logon type is intended for users who will be interactively using the computer, such as a user being logged
        /// on by a terminal server, remote shell, or similar process. This logon type has the additional expense of caching
        /// logon information for disconnected operations; therefore, it is inappropriate for some client/server applications,
        /// such as a mail server.
        /// </summary>
        Interactive = 2,

        /// <summary>
        /// This logon type is intended for high performance servers to authenticate plaintext passwords.
        /// The LogonUser function does not cache credentials for this logon type.
        /// </summary>
        Network = 3,

        /// <summary>
        /// This logon type is intended for batch servers, where processes may be executing on behalf of a user
        /// without their direct intervention. This type is also for higher performance servers that process many
        /// plaintext authentication attempts at a time, such as mail or web servers.
        /// </summary>
        Batch = 4,

        /// <summary>
        /// Indicates a service-type logon. The account provided must have the service privilege enabled.
        /// </summary>
        Service = 5,

        /// <summary>
        /// GINAs are no longer supported.
        /// Windows Server 2003 and Windows XP:  This logon type is for GINA DLLs that log on users who will be
        /// interactively using the computer. This logon type can generate a unique audit record that shows when
        /// the workstation was unlocked.
        /// </summary>
        Unlock = 7,

        /// <summary>
        /// This logon type preserves the name and password in the authentication package, which allows the server
        /// to make connections to other network servers while impersonating the client. A server can accept plaintext
        /// credentials from a client, call LogonUser, verify that the user can access the system across the network,
        /// and still communicate with other servers.
        /// </summary>
        NetworkCleartext = 8,

        /// <summary>
        /// This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
        /// The new logon session has the same local identifier but uses different credentials for other network connections.
        /// This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
        /// </summary>
        NewCredentials = 9,
    }

    internal class NativeMethods
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool LogonUser(String lpszUsername, String lpszDomain, IntPtr phPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr handle);
    }

    internal sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeTokenHandle(IntPtr handle)
            : base(true)
        {
            this.handle = handle;
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(handle);
        }
    }
}

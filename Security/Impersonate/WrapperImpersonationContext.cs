using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace WM.Common.Security.Impersonate
{
    public class WrapperImpersonationContext {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(String lpszUsername, String lpszDomain,
        String lpszPassword, int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static bool CloseHandle(IntPtr handle);

        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private const int LOGON32_LOGON_INTERACTIVE = 2;

        private string m_Domain;
        private string m_Password;
        private string m_Username;

        public WindowsImpersonationContext Context = null;

        protected bool IsInContext {
            get { return Context != null; }
        }

        public WrapperImpersonationContext(string domain, string username, string password) {
            m_Domain = domain;
            m_Username = username;
            m_Password = password;

            Impersonate();
        }

        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private void Impersonate() {
            if (this.IsInContext) return;
            SafeTokenHandle m_Token;

            bool logonSuccessfull = LogonUser(
                m_Username,
                m_Domain,
                m_Password,
                LOGON32_LOGON_INTERACTIVE,
                LOGON32_PROVIDER_DEFAULT,
                out m_Token);

            if (logonSuccessfull == false) {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            WindowsImpersonationContext identity = WindowsIdentity.Impersonate(m_Token.DangerousGetHandle());
            Context = identity;
        }
    }

    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid {
        private SafeTokenHandle()
            : base(true) {
        }

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle() {
            return CloseHandle(handle);
        }
    }
}



namespace WM.Common.Security.Impersonate.Tools
{
    #region Using directives.
    // ----------------------------------------------------------------------

    using System;
    using System.Security.Principal;
    using System.Runtime.InteropServices;
    using System.ComponentModel;

    // ----------------------------------------------------------------------
    #endregion

    /////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Impersonation of a user. Allows to execute code under another
    /// user context.
    /// Please note that the account that instantiates the Impersonator class
    /// needs to have the 'Act as part of operating system' privilege set.
    /// </summary>
    /// <remarks>    
    /// This class is based on the information in the Microsoft knowledge base
    /// article http://support.microsoft.com/default.aspx?scid=kb;en-us;Q306158
    /// 
    /// Encapsulate an instance into a using-directive like e.g.:
    /// 
    ///        ...
    ///        using ( new Impersonator( "myUsername", "myDomainname", "myPassword" ) )
    ///        {
    ///            ...
    ///            [code that executes under the new context]
    ///            ...
    ///        }
    ///        ...
    /// 
    /// Please contact the author Uwe Keim (mailto:uwe.keim@zeta-software.de)
    /// for questions regarding this class.
    /// </remarks>
    public class Impersonator :
        IDisposable
    {
        #region Public methods.
        // ------------------------------------------------------------------

        /// <summary>
        /// Constructor. Starts the impersonation with the given credentials.
        /// Please note that the account that instantiates the Impersonator class
        /// needs to have the 'Act as part of operating system' privilege set.
        /// </summary>
        /// <param name="userName">The name of the user to act as.</param>
        /// <param name="domainName">The domain name of the user to act as.</param>
        /// <param name="password">The password of the user to act as.</param>
        public Impersonator(
            string userName,
            string domainName,
            string password)
        {
            ImpersonateValidUser(userName, domainName, password);
        }

        // ------------------------------------------------------------------
        #endregion

        #region IDisposable member.
        // ------------------------------------------------------------------

        public void Dispose()
        {
            UndoImpersonation();
        }

        // ------------------------------------------------------------------
        #endregion

        #region P/Invoke.
        // ------------------------------------------------------------------

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int LogonUser(
            string lpszUserName,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DuplicateToken(
            IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(
            IntPtr handle);

        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_PROVIDER_DEFAULT = 0;

        // ------------------------------------------------------------------
        #endregion

        #region Private member.
        // ------------------------------------------------------------------

        /// <summary>
        /// Does the actual impersonation.
        /// </summary>
        /// <param name="userName">The name of the user to act as.</param>
        /// <param name="domainName">The domain name of the user to act as.</param>
        /// <param name="password">The password of the user to act as.</param>
        private void ImpersonateValidUser(
            string userName,
            string domain,
            string password)
        {
            WindowsIdentity tempWindowsIdentity = null;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            try
            {
                if (RevertToSelf())
                {
                    if (LogonUser(
                        userName,
                        domain,
                        password,
                        LOGON32_LOGON_INTERACTIVE,
                        LOGON32_PROVIDER_DEFAULT,
                        ref token) != 0)
                    {
                        if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                        {
                            tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                            impersonationContext = tempWindowsIdentity.Impersonate();
                        }
                        else
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                    }
                    else
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            finally
            {
                if (token != IntPtr.Zero)
                {
                    CloseHandle(token);
                }
                if (tokenDuplicate != IntPtr.Zero)
                {
                    CloseHandle(tokenDuplicate);
                }
            }
        }

        /// <summary>
        /// Reverts the impersonation.
        /// </summary>
        private void UndoImpersonation()
        {
            if (impersonationContext != null)
            {
                impersonationContext.Undo();
            }
        }

        private WindowsImpersonationContext impersonationContext = null;

        // ------------------------------------------------------------------
        #endregion
    }

    /////////////////////////////////////////////////////////////////////////
}


//using System;
//using System.Runtime.InteropServices;
//using System.Security.Principal;
//using System.Security.Permissions;
//using System.ComponentModel;

//public class WrapperImpersonationContext
//{
//    [DllImport("advapi32.dll", SetLastError = true)]
//    public static extern bool LogonUser(String lpszUsername, String lpszDomain,
//    String lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

//    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
//    public extern static bool CloseHandle(IntPtr handle);

//    private const int LOGON32_PROVIDER_DEFAULT = 0;
//    private const int LOGON32_LOGON_INTERACTIVE = 2;

//    private string m_Domain;
//    private string m_Password;
//    private string m_Username;
//    private IntPtr m_Token;

//    public WindowsImpersonationContext m_Context = null;

//    protected bool IsInContext
//    {
//        get { return m_Context != null; }
//    }

//    public WrapperImpersonationContext(string domain, string username, string password)
//    {
//        m_Domain = domain;
//        m_Username = username;
//        m_Password = password;

//        Enter();
//    }

//    [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
//    public void Enter()
//    {
//        if (this.IsInContext) return;
//        m_Token = new IntPtr(0);

//        try
//        {
//            m_Token = IntPtr.Zero;
//            bool logonSuccessfull = LogonUser(
//               m_Username,
//               m_Domain,
//               m_Password,
//               LOGON32_LOGON_INTERACTIVE,
//               LOGON32_PROVIDER_DEFAULT,
//               ref m_Token);
//            if (logonSuccessfull == false)
//            {
//                int error = Marshal.GetLastWin32Error();
//                throw new Win32Exception(error);
//            }
//            WindowsIdentity identity = new WindowsIdentity(m_Token);
//            m_Context = identity.Impersonate();
//        }
//        catch (Exception exception)
//        {
//            // Catch exceptions here
//        }
//    }


//    [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
//    public void Leave()
//    {
//        if (this.IsInContext == false) return;
//        m_Context.Undo();

//        if (m_Token != IntPtr.Zero) CloseHandle(m_Token);
//        m_Context = null;
//    }
//}
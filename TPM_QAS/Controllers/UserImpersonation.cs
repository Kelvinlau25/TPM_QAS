using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace TPM_QAS.Controllers
{
    public class UserImpersonation : IDisposable
    {
        private WindowsImpersonationContext _windowsImpersonationContext;
        private IntPtr _tokenHandle;
        private string _userName;
        private string _domain;
        private string _passWord;

        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_LOGON_INTERACTIVE = 2;

        [DllImport("advapi32.dll")]
        private static extern bool LogonUser(String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        public UserImpersonation(string userName, string domain, string passWord)
        {
            _userName = userName;
            _domain = domain;
            _passWord = passWord;
        }

        public bool ImpersonateValidUser()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows impersonation is only available on Windows
                return false;
            }

            bool returnValue = LogonUser(_userName, _domain, _passWord,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    ref _tokenHandle);

            if (false == returnValue)
            {
                return false;
            }

            WindowsIdentity newId = new WindowsIdentity(_tokenHandle);
            _windowsImpersonationContext = newId.Impersonate();
            return true;
        }

        public void Dispose()
        {
            if (_windowsImpersonationContext != null)
                _windowsImpersonationContext.Undo();
            if (_tokenHandle != IntPtr.Zero)
                CloseHandle(_tokenHandle);
        }
    }
}

using System;
using System.ComponentModel;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace XperiCode.Impersonator
{
    public class Impersonator : IDisposable
    {
        #region "Unmanaged"

        [DllImport("advapi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LogonUserA(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int DuplicateToken(IntPtr existingTokenHandle, int impersonationLevel, ref IntPtr duplicateTokenHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern long CloseHandle(IntPtr handle);

        private const int Logon32LogonInteractive = 2;
        private const int Logon32ProviderDefault = 0;

        #endregion

        private WindowsImpersonationContext _impersonationContext = null;

        public Impersonator(WindowsIdentity identity)
        {
            Impersonate(identity);
        }

        public Impersonator(string userName, string password)
        {
            string[] temp = userName.Split('\\');
            if (temp.Length != 2)
            {
                throw new ArgumentException(@"userName should include domainName, in the format <domainName>\<userName>", "userName");
            }

            Impersonate(temp[0], temp[1], password);
        }

        public Impersonator(string domainName, string userName, string password)
        {
            Impersonate(domainName, userName, password);
        }

        private void Impersonate(string domainName, string userName, string password)
        {
            var identity = GetIdentity(domainName, userName, password);

            Impersonate(identity);
        }

        private void Impersonate(WindowsIdentity identity)
        {
            try
            {
                _impersonationContext = identity.Impersonate();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(@"An error occured while trying to impersonate {0}.", identity.Name), ex);
            }
        }

        public static WindowsIdentity GetIdentity(string userName, string password)
        {
            string[] temp = userName.Split('\\');
            if (temp.Length != 2)
            {
                throw new ArgumentException(@"userName should include domainName, in the format <domainName>\<userName>", "userName");
            }

            return GetIdentity(temp[0], temp[1], password);
        }

        public static WindowsIdentity GetIdentity(string domainName, string userName, string password)
        {
            var token = IntPtr.Zero;
            var tokenDuplicate = IntPtr.Zero;

            try
            {
                if (!RevertToSelf())
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                if (LogonUserA(userName, domainName, password, Logon32LogonInteractive, Logon32ProviderDefault, ref token) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                if (DuplicateToken(token, 2, ref tokenDuplicate) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                return new WindowsIdentity(tokenDuplicate);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(@"Could not obtain a WindowsIdentity for {0}\{1} using the supplied credentials.", domainName, userName), ex);
            }
            finally
            {
                if (!tokenDuplicate.Equals(IntPtr.Zero))
                {
                    CloseHandle(tokenDuplicate);
                }

                if (!token.Equals(IntPtr.Zero))
                {
                    CloseHandle(token);
                }
            }
        }

        public void Dispose()
        {
            if (_impersonationContext != null)
            {
                _impersonationContext.Undo();
            }
        }
    }
}

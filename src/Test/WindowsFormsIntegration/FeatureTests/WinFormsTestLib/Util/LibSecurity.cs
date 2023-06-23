namespace WFCTestLib.Util 
{
    using System;
    using System.Drawing.Printing;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
	using System.Windows.Forms;

    public class LibSecurity
	{
		public static readonly FileIOPermission UnrestrictedFileIO         = new FileIOPermission(PermissionState.Unrestricted);
		public static readonly SecurityPermission    UnmanagedCode         = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
        public static readonly SecurityPermission    GetPermissionState    = new SecurityPermission(SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy);
        public static readonly SecurityPermission    ControlThread         = new SecurityPermission(SecurityPermissionFlag.ControlThread);
        public static readonly ReflectionPermission  Reflection            = new ReflectionPermission(PermissionState.Unrestricted);
        public static readonly EnvironmentPermission Environment           = new EnvironmentPermission(PermissionState.Unrestricted);
        public static readonly EnvironmentPermission SensitiveSystemInformation = Environment;

        public static readonly UIPermission          AllWindows            = new UIPermission(UIPermissionWindow.AllWindows);
        public static readonly UIPermission          SafeTopLevelWindows   = new UIPermission(UIPermissionWindow.SafeTopLevelWindows);
        public static readonly UIPermission          SafeSubWindows        = new UIPermission(UIPermissionWindow.SafeSubWindows);
        public static readonly UIPermission          AllClipboard          = new UIPermission(UIPermissionClipboard.AllClipboard);

        public static readonly FileIOPermission      FileDialogCustomization = new FileIOPermission(PermissionState.Unrestricted);
        public static readonly FileDialogPermission  FileDialogOpenFile    = new FileDialogPermission(FileDialogPermissionAccess.Open);
        public static readonly FileDialogPermission  FileDialogSaveFile    = new FileDialogPermission(FileDialogPermissionAccess.Save);

        public static readonly PrintingPermission    AllPrinting           = new PrintingPermission(PrintingPermissionLevel.AllPrinting);
        public static readonly PrintingPermission    DefaultPrinting       = new PrintingPermission(PrintingPermissionLevel.DefaultPrinting);
        public static readonly PrintingPermission    SafePrinting          = new PrintingPermission(PrintingPermissionLevel.SafePrinting);
		
        public static readonly NamedPermissionSet    FullTrust             = new NamedPermissionSet("FullTrust");

        public static FileIOPermission GetReadFileIO(string filename) 
		{
            string full = filename;
            
            FileIOPermission fiop = new FileIOPermission(PermissionState.None);
            fiop.AllFiles = FileIOPermissionAccess.PathDiscovery;

            fiop.Assert();
            full = Path.GetFullPath(filename);
            CodeAccessPermission.RevertAssert();

            return new FileIOPermission(FileIOPermissionAccess.Read, full);
        }

        public static FileIOPermission GetWriteFileIO(string filename) 
		{
            string full = filename;

			FileIOPermission fiop = new FileIOPermission(PermissionState.None);
			fiop.AllFiles = FileIOPermissionAccess.PathDiscovery;

			fiop.Assert();
            full = Path.GetFullPath(filename);
            CodeAccessPermission.RevertAssert();

            return new FileIOPermission(FileIOPermissionAccess.Write, full);
        }

		public static FileIOPermission GetPathDiscovery(string filename)
		{
			string full = filename;

			FileIOPermission fiop = new FileIOPermission(PermissionState.None);
			fiop.AllFiles = FileIOPermissionAccess.PathDiscovery;

			fiop.Assert();
			full = Path.GetFullPath(filename);
			CodeAccessPermission.RevertAssert();

			return new FileIOPermission(FileIOPermissionAccess.PathDiscovery, full);
		}

		public static void CrackSecurityException(SecurityException se, out IPermission dip, out PermissionSet dps/*, out PermissionSetCollection dpsc*/)
		{
			SecurityPermission sp = new SecurityPermission(PermissionState.Unrestricted);
			sp.Assert();
			dip = se.Demanded as IPermission;
			dps = se.Demanded as PermissionSet;
//			dpsc = se.Demanded as PermissionSetCollection;
			CodeAccessPermission.RevertAssert();
		}
    }
}

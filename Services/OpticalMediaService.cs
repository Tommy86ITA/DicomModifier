using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DicomModifier.Services
{
    /// <summary>
    /// Provides helper methods to detect and eject optical media used as import sources.
    /// </summary>
    public partial class OpticalMediaService
    {
        private const uint GenericRead = 0x80000000;
        private const uint GenericWrite = 0x40000000;
        private const uint FileShareRead = 0x00000001;
        private const uint FileShareWrite = 0x00000002;
        private const uint OpenExisting = 3;
        private const int InvalidHandleValue = -1;
        private const uint IoctlStorageEjectMedia = 0x2D4808;

        /// <summary>
        /// Gets the optical drive root for the specified path.
        /// </summary>
        /// <param name="sourcePath">The imported file or folder path.</param>
        /// <returns>The drive root if the source path is on optical media; otherwise, <see langword="null" />.</returns>
        public static string? GetOpticalDriveRoot(string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                return null;
            }

            string? driveRoot = Path.GetPathRoot(sourcePath);
            if (string.IsNullOrWhiteSpace(driveRoot))
            {
                return null;
            }

            try
            {
                DriveInfo driveInfo = new(driveRoot);
                if (!driveInfo.IsReady)
                {
                    Debug.WriteLine($"Drive {driveRoot} is not ready.");
                    return null;
                }

                bool isOpticalMedia = driveInfo.DriveType == DriveType.CDRom;
                Debug.WriteLine($"Drive {driveRoot} optical media: {isOpticalMedia} ({driveInfo.DriveType}).");
                return isOpticalMedia ? driveRoot : null;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
            {
                Debug.WriteLine($"Unable to inspect drive for path '{sourcePath}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Attempts to eject the optical media containing the specified source path.
        /// </summary>
        /// <param name="sourcePath">The imported file or folder path.</param>
        /// <param name="errorMessage">The error message returned when the eject operation fails.</param>
        /// <returns><see langword="true" /> if the media was ejected; otherwise, <see langword="false" />.</returns>
        public static bool TryEject(string sourcePath, out string? errorMessage)
        {
            errorMessage = null;

            string? driveRoot = GetOpticalDriveRoot(sourcePath);
            if (driveRoot is null)
            {
                errorMessage = "Il percorso selezionato non si trova su un CD/DVD.";
                return false;
            }

            string volumePath = NormalizeVolumePath(driveRoot);
            IntPtr handle = CreateFile(
                volumePath,
                GenericRead | GenericWrite,
                FileShareRead | FileShareWrite,
                IntPtr.Zero,
                OpenExisting,
                0,
                IntPtr.Zero);

            if (handle.ToInt64() == InvalidHandleValue)
            {
                errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                return false;
            }

            try
            {
                int bytesReturned = 0;
                bool success = DeviceIoControl(
                    handle,
                    IoctlStorageEjectMedia,
                    IntPtr.Zero,
                    0,
                    IntPtr.Zero,
                    0,
                    ref bytesReturned,
                    IntPtr.Zero);

                if (!success)
                {
                    errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                }

                return success;
            }
            finally
            {
                CloseHandle(handle);
            }
        }

        /// <summary>
        /// Converts a drive root path to the Win32 volume path expected by <c>CreateFile</c>.
        /// </summary>
        /// <param name="driveRoot">The drive root path.</param>
        /// <returns>The Win32 volume path.</returns>
        private static string NormalizeVolumePath(string driveRoot)
        {
            string normalizedDrive = driveRoot.TrimEnd('\\');
            return @"\\.\" + normalizedDrive;
        }

        [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        private static partial IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            IntPtr lpOutBuffer,
            uint nOutBufferSize,
            ref int lpBytesReturned,
            IntPtr lpOverlapped);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool CloseHandle(IntPtr hObject);
    }
}

using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace ProjectMTGCollector.Utils
{
    internal static class WebBrowserCompat
    {
        public static void EnsureIeEmulation()
        {
            try
            {
                // Use IE11 Edge mode for the WebBrowser control
                const int IE11_EMULATION = 11001;
                string exeName = Path.GetFileName(Process.GetCurrentProcess().MainModule?.FileName ?? "");
                if (string.IsNullOrEmpty(exeName)) return;

                using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION");
                key?.SetValue(exeName, IE11_EMULATION, RegistryValueKind.DWord);

                // Optional: enable GPU rendering
                using var gpuKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_GPU_RENDERING");
                gpuKey?.SetValue(exeName, 1, RegistryValueKind.DWord);
            }
            catch
            {
                // Ignore registry write failures
            }
        }
    }
}

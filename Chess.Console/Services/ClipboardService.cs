using System.Diagnostics;

namespace Chess.Console.Services;

public static class ClipboardService
{
    public static void Copy(string text)
    {
        var psi = BuildStartInfo();

        using var proc = Process.Start(psi)!;
        proc.StandardInput.Write(text);
        proc.StandardInput.Close();
        proc.WaitForExit();
    }

    internal static ProcessStartInfo BuildStartInfo()
    {
        if (OperatingSystem.IsMacOS())
        {
            return new ProcessStartInfo("pbcopy") { RedirectStandardInput = true, UseShellExecute = false };
        }

        if (OperatingSystem.IsWindows())
        {
            return new ProcessStartInfo("clip") { RedirectStandardInput = true, UseShellExecute = false };
        }

        return new ProcessStartInfo("xclip", "-selection clipboard") { RedirectStandardInput = true, UseShellExecute = false };
    }
}

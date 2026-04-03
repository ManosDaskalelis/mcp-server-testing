using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

[McpServerToolType]
public class DemoTools
{
    const uint EWX_LOGOFF = 0x00000000;

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool LockWorkStation();

    [McpServerTool, Description("Logs out of the current session")]
    public static void ExitWindows()
    {
        bool success = ExitWindowsEx(EWX_LOGOFF, 0);

        if (!success)
        {
            Console.WriteLine("Failed to log off. Error: " + Marshal.GetLastWin32Error());
        }
    }

    [McpServerTool, Description("Locks the workstation")]
    public static void LockStation()
    {
        bool success = LockWorkStation();

        if (!success)
        {
            Console.WriteLine("Failed to log off. Error: " + Marshal.GetLastWin32Error());
        }
    }

    [McpServerTool, Description("Echoes the input message back to the caller.")]
    public static string Echo([Description("The message to echo")] string message)
    {
        return message;
    }

    [McpServerTool, Description("Returns the current UTC date and time in ISO 8601 format.")]
    public static string UtcNow()
    {
        return DateTime.UtcNow.ToString("o");
    }

    [McpServerTool, Description("Returns the content of a file")]
    public static string ReadFile([Description("The path of the file you want to read")] string filepath)
    {
        try
        {
            byte[] buffer = new byte[1024];
            Span<byte> span = buffer.AsSpan();
            var file = File.Open(filepath, FileMode.Open, FileAccess.Read);
            int bytesRead = file.Read(span);

            string result = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            return result;
        }
        catch (UnauthorizedAccessException ex)
        {
            throw ex;
        }
    }

    [McpServerTool, Description("Opens a file or folder")]
    public static bool OpenSomething([Description("The path of the file or folder you want to open")] string pathToFind)
    {
        try
        {
            string defaultPath = @"C:\Users\";

            Process.Start(new ProcessStartInfo
            {
                FileName = pathToFind ?? defaultPath,
                UseShellExecute = true
            });

            return false;
        }
        catch (UnauthorizedAccessException ex)
        {
            throw ex;
        }
    }

    [McpServerTool, Description("Terminates the web server gracefully.")]
    public static string TerminateServer(IHostApplicationLifetime lifetime)
    {
        lifetime.StopApplication();
        return "Server is shutting down.";
    }

    static void Main()
    {
        bool success = ExitWindowsEx(EWX_LOGOFF, 0);

        if (!success)
        {
            Console.WriteLine("Failed to log off. Error: " + Marshal.GetLastWin32Error());
        }
    }
}

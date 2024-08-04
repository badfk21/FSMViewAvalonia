using System.Runtime.Versioning;

namespace FSMViewAvalonia2;
internal partial class Program
{

    [SupportedOSPlatform("windows")]
    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial void MessageBoxW(IntPtr _, string lpText, string lpCaption, uint uType);

    public static Thread mainThread = null;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    public static void Main(string[] args)
    {
        mainThread = Thread.CurrentThread;

        try
        {
            _ = BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                MessageBoxW(IntPtr.Zero, e.ToString(), "Exception!", 0x10);
            }
        }

    }



    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                ;
}

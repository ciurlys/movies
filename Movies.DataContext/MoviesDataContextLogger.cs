using static System.Environment;

namespace Movies.EntityModels;

public class MoviesDataContextLogger
{
    private static readonly string logFilePath;

    static MoviesDataContextLogger()
    {
        string folder = Path.Combine("..", "Movies.Logs");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string dateTimeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        logFilePath = Path.Combine(folder, $"movieslog-{dateTimeStamp}");
    }

    public static void WriteLine(string message)
    {
        using (StreamWriter logFile = File.AppendText(logFilePath))
        {
            logFile.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }
    }
}

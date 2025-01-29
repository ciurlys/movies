using static System.Environment;

namespace Movies.EntityModels;

public class MoviesDataContextLogger
{
    public static void WriteLine(string message)
    {
	string folder = Path.Combine("..", "Movies.Logs");

	if (!Directory.Exists(folder))
	    Directory.CreateDirectory(folder);

	string dateTimeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

	string path = Path.Combine(folder, $"movieslog-{dateTimeStamp}");

	StreamWriter logFile = File.AppendText(path);
	logFile.WriteLine(message);
	logFile.Close();
    }

}

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class DeleteFilesService
{
    private readonly string outputFolder;
    private readonly DateTime deleteBeforeTimestamp;
    private bool isRunning = true;
    private bool isVideoCreated = false;

    public DeleteFilesService(string outputFolder, DateTime deleteBeforeTimestamp)
    {
        this.outputFolder = outputFolder;
        this.deleteBeforeTimestamp = deleteBeforeTimestamp;
    }
    public void Start()
    {
        Task.Run(() =>
        {
            while (isRunning)
            {
                DeleteOldFiles();
                Thread.Sleep(60000);
            }
        });
    }
    public void Stop()
    {
        isRunning = false;
    }
    public void MarkVideoAsCreated()
    {
        isVideoCreated = true;
    }

    public void DeleteOldFiles()
    {
        try
        {
            var files = Directory.GetFiles(outputFolder, "*.*")
                                 .Where(f => f.EndsWith(".png") || f.EndsWith(".avi"));
            foreach (var file in files)
            {
                DateTime? fileDate = ExtractTimestampFromFilename(file);
                if (fileDate.HasValue)
                {
                    if (file.EndsWith(".png") && fileDate.Value < deleteBeforeTimestamp)
                    {
                        File.Delete(file);
                        Console.WriteLine($"Deleted image: {file}");
                    }
                    else if (file.EndsWith(".avi") && fileDate.Value < deleteBeforeTimestamp)
                    {
                        if (isVideoCreated)
                        {
                            File.Delete(file);
                            Console.WriteLine($"Deleted video: {file}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting files: {ex.Message}");
        }
    }
    private DateTime? ExtractTimestampFromFilename(string filePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string[] parts = fileName.Split('_');

        if (parts.Length >= 3)
        {
            string datePart = parts[1];
            string timePart = parts[2];
            if (DateTime.TryParseExact(datePart + timePart, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
        }
        return null;
    }
}

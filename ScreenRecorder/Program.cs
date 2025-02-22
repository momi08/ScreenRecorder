using System;
using ScreenRecorder.Services;

class Program
{
    static void Main()
    {
        string outputFolder = @"C:\Users\tomis\Desktop\snimke";
        int interval = 500;

        Console.WriteLine("Do you want to delete files older than a specific timestamp? (y/n):");
        string deleteOption = Console.ReadLine()?.ToLower();

        if (deleteOption == "n")
        {
            Console.WriteLine("You chose not to delete any files. Skipping timestamp input.");
            ScreenCaptureService screenCaptureService = new ScreenCaptureService(outputFolder, interval);
            VideoService videoService = new VideoService(outputFolder);
            screenCaptureService.StartRecording();
            Console.WriteLine("Screen capture started. Press 'Enter' to stop.");
            Console.ReadLine();
            screenCaptureService.StopRecording();

            Console.WriteLine("Creating video...");
            videoService.CreateVideoFromImages();
            Console.WriteLine("Screen capture stopped.");
        }
        else if (deleteOption == "y")
        {
            Console.WriteLine("Enter the timestamp (yyyy-MM-dd HH:mm:ss) to delete files older than this time:");
            string timestampInput = Console.ReadLine();

            DateTime deleteBeforeTimestamp;
            if (DateTime.TryParseExact(timestampInput, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out deleteBeforeTimestamp))
            {
                ScreenCaptureService screenCaptureService = new ScreenCaptureService(outputFolder, interval);
                VideoService videoService = new VideoService(outputFolder);
                DeleteFilesService deleteFiles = new DeleteFilesService(outputFolder, deleteBeforeTimestamp);

                screenCaptureService.StartRecording();
                deleteFiles.Start();
                Console.WriteLine("Screen capture started. Press 'Enter' to stop.");
                Console.ReadLine();
                screenCaptureService.StopRecording();
                Console.WriteLine("Creating video...");
                videoService.CreateVideoFromImages();
                deleteFiles.MarkVideoAsCreated();
                deleteFiles.Stop();
                Console.WriteLine("Screen capture stopped.");
            }
            else
            {
                Console.WriteLine("Wrong timestamp format. Please use yyyy-MM-dd HH:mm:ss.");
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Exiting program.");
        }
    }
}

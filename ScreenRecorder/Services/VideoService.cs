using OpenCvSharp;
using System;
using System.IO;

public class VideoService
{
    private readonly string outputFolder;
    public VideoService(string outputFolder)
    {
        this.outputFolder = outputFolder;
    }
    public void CreateVideoFromImages()
    {
        string[] imageFiles = Directory.GetFiles(outputFolder, "*.png");

        if (imageFiles.Length == 0)
        {
            Console.WriteLine("No images found.");
            return;
        }
        string videoPath = Path.Combine(outputFolder, "output_video.avi");
        using (var writer = new VideoWriter(videoPath, FourCC.MJPG, 30, new OpenCvSharp.Size(1920, 1080)))
        {
            foreach (string imagePath in imageFiles)
            {
                using (var frame = new Mat(imagePath))
                {
                    writer.Write(frame);
                }
            }
        }
        Console.WriteLine($"Video saved at: {videoPath}");
    }
}

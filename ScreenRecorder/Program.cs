using System;
using ScreenRecorder.Services;

class Program
{
    static void Main()
    {
        string outputFolder = @"C:\Users\tomis\Desktop\snimke";
        int interval = 1000;
        ScreenCaptureService screenCaptureService = new ScreenCaptureService(outputFolder, interval);
        screenCaptureService.StartRecording();
        Console.WriteLine("Screen capture started. Press 'Enter' to stop.");
        Console.ReadLine();
        screenCaptureService.StopRecording();
        Console.WriteLine("Screen capture stopped.");
    }
}

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ScreenRecorder.Services
{
    public class ScreenCaptureService
    {
        private readonly string outputFolder;
        private readonly int interval;
        private bool isRecording = false;
        public bool IsRecording => isRecording;

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hwnd, ref Rectangle lpRect);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        public ScreenCaptureService(string outputFolder, int interval)
        {
            this.outputFolder = outputFolder;
            this.interval = interval;
            Directory.CreateDirectory(outputFolder);
        }
        public void StartRecording()
        {
            isRecording = true;
            Task.Run(() =>
            {
                while (isRecording)
                {
                    CaptureScreen();
                    Thread.Sleep(interval);
                }
            }
            );
        }
        public void StopRecording()
        {
            isRecording = false;
        }
        private void CaptureScreen()
        {
            try
            {
                var hwnd = GetDesktopWindow();
                var rect = new Rectangle();
                GetClientRect(hwnd, ref rect);
                int width = rect.Width;
                int height = rect.Height;
                Console.WriteLine($"Capturing screen of size: {width}x{height}");
                using (Bitmap bitmap = new Bitmap(width, height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(width, height));
                    }
                    using (var ms = new MemoryStream())
                    {
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Seek(0, SeekOrigin.Begin);
                        using (var skiaImage = SKImage.FromEncodedData(ms))
                        using (var data = skiaImage.Encode())
                        {
                            if (!Directory.Exists(outputFolder))
                            {
                                Console.WriteLine("Output folder does not exist, creating...");
                                Directory.CreateDirectory(outputFolder);
                            }
                            string filePath = Path.Combine(outputFolder, "screenshot_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".png");
                            using (var stream = File.OpenWrite(filePath))
                            {
                                data.SaveTo(stream);
                            }
                            Console.WriteLine($"Screenshot saved at: {filePath}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing screen: {ex.Message}");
            }
        }
    }
}

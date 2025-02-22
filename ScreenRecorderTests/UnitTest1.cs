using NUnit.Framework;
using System.IO;
using ScreenRecorder.Services;

namespace ScreenRecorder.Tests
{
    [TestFixture]
    public class ScreenCaptureServiceTests
    {
        private string outputFolder = @"C:\Test";
        private ScreenCaptureService screenCaptureService;

        [SetUp]
        public void SetUp()
        {
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            screenCaptureService = new ScreenCaptureService(outputFolder, 1000);
        }

        [Test]
        public void StartRecording_ShouldStartRecording()
        {
            screenCaptureService.StartRecording();
            var isRecording = screenCaptureService.IsRecording;
            Assert.IsTrue(isRecording);
        }

        [Test]
        public void StopRecording_ShouldStopRecording() { 
            screenCaptureService.StartRecording();
            screenCaptureService.StopRecording();
            var isRecording = screenCaptureService.IsRecording;
            Assert.IsFalse(isRecording);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up
            if (Directory.Exists(outputFolder))
            {
                Directory.Delete(outputFolder, true);
            }
        }
    }
    [TestFixture]
    public class DeleteFilesServiceTests
    {
        private string testDirectory;

        [SetUp]
        public void SetUp()
        {
            testDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles");
            Directory.CreateDirectory(testDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }

        [Test]
        public void DeleteFiles_ShouldDeleteOldImages()
        {
            string oldFileName = Path.Combine(testDirectory, "image_20230101_123456.png");
            File.WriteAllText(oldFileName, "Test file");

            DateTime deleteBeforeTimestamp = new DateTime(2023, 1, 2, 0, 0, 0);
            var deleteService = new DeleteFilesService(testDirectory, deleteBeforeTimestamp);

            deleteService.DeleteOldFiles();

            Assert.IsFalse(File.Exists(oldFileName));
        }

        [Test]
        public void DeleteFiles_ShouldNotDeleteNewImages()
        {
            string newFileName = Path.Combine(testDirectory, "image_20230102_123456.png");
            File.WriteAllText(newFileName, "Test file");

            DateTime deleteBeforeTimestamp = new DateTime(2023, 1, 1, 0, 0, 0);
            var deleteService = new DeleteFilesService(testDirectory, deleteBeforeTimestamp);

            deleteService.DeleteOldFiles();

            Assert.IsTrue(File.Exists(newFileName));
        }

        [Test]
        public void DeleteFiles_ShouldDeleteVideoAfterMarkingAsCreated()
        {
            string videoFileName = Path.Combine(testDirectory, "video_20230101_123456.avi");
            File.WriteAllText(videoFileName, "Test video");

            DateTime deleteBeforeTimestamp = new DateTime(2023, 1, 2, 0, 0, 0);
            var deleteService = new DeleteFilesService(testDirectory, deleteBeforeTimestamp);

            deleteService.MarkVideoAsCreated();
            deleteService.DeleteOldFiles();
            Assert.IsFalse(File.Exists(videoFileName));
        }

        [Test]
        public void DeleteFiles_ShouldNotDeleteVideoBeforeMarkingAsCreated()
        {
            string videoFileName = Path.Combine(testDirectory, "video_20230101_123456.avi");
            File.WriteAllText(videoFileName, "Test video");

            DateTime deleteBeforeTimestamp = new DateTime(2023, 1, 2, 0, 0, 0);
            var deleteService = new DeleteFilesService(testDirectory, deleteBeforeTimestamp);

            deleteService.DeleteOldFiles();
            Assert.IsTrue(File.Exists(videoFileName));
        }
    }
    [TestFixture]
    public class VideoServiceTests
    {
        private string outputFolder;

        [SetUp]
        public void Setup()
        {
            outputFolder = Path.Combine(Path.GetTempPath(), "TestOutput");
            Directory.CreateDirectory(outputFolder);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(outputFolder))
            {
                Directory.Delete(outputFolder, true);
            }
        }

        [Test]
        public void CreateVideoFromImages_ShouldCreateVideo_WhenImagesExist()
        {
            for (int i = 0; i < 5; i++)
            {
                string imagePath = Path.Combine(outputFolder, $"image_{DateTime.Now:yyyyMMdd_HHmmss}_{i}.png");
                File.WriteAllText(imagePath, "Test image");
            }

            var videoService = new VideoService(outputFolder);

            videoService.CreateVideoFromImages();

            string videoFileName = Directory.GetFiles(outputFolder, "output_video_*.avi")[0]; // Get the created video file
            Assert.IsTrue(File.Exists(videoFileName), "The video file should be created.");
        }

        [Test]
        public void CreateVideoFromImages_ShouldNotCreateVideo_WhenNoImagesExist()
        {
            var videoService = new VideoService(outputFolder);

            videoService.CreateVideoFromImages();

            string[] videoFiles = Directory.GetFiles(outputFolder, "*.avi");
            Assert.IsEmpty(videoFiles, "No video file should be created when no images are found.");
        }
        [Test]
        public void CreateVideoFromImages_ShouldCreateMultipleVideos_WhenMultipleImagesExist()
        {
            for (int i = 0; i < 10; i++)
            {
                string imagePath = Path.Combine(outputFolder, $"image_{DateTime.Now:yyyyMMdd_HHmmss}_{i}.png");
                File.WriteAllText(imagePath, "Test image");
            }

            var videoService = new VideoService(outputFolder);
            videoService.CreateVideoFromImages();

            string[] videoFiles = Directory.GetFiles(outputFolder, "*.avi");
            Assert.IsNotEmpty(videoFiles, "The video file should be created when there are images.");
        }
    }
}

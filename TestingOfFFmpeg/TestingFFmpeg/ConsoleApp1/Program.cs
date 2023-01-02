using System.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using System.Collections;
//using static System.Net.WebRequestMethods;
using System.Runtime.InteropServices;
using SkiaSharp;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Reflection.Emit;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace TestingFFmpeg
{
    class Program
    {
        static void Main(string[] args)
        {
            //NumberCheck();

            Run().GetAwaiter().GetResult();


        }

        private static async Task Run()
        {
            //First download and install FFmpeg into the folder
            string outputFolder = nameof(TestingFFmpeg) + GetCurrentTime();
            string convertedVideoDir, testOutputFolder, ffmpegFolder;
            CreateTemporaryFolders(outputFolder, out convertedVideoDir, out testOutputFolder, out ffmpegFolder);
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ffmpegFolder);
            FFmpeg.SetExecutablesPath(ffmpegFolder);
            Console.WriteLine("FFmpeg is downloaded");

            //Read the video file
            string testFilePath = Path.Combine("C:\\Work and files\\Cloud Computing Project\\TestingOfFFmpeg\\TestingFFmpeg\\TestingFiles", "circle.mp4");
            string fileName = Path.GetFileName(testFilePath);
            Console.WriteLine($"Found the file named {fileName}");

            //Turn Video into images
            Func<string, string> outputFileNameBuilder = (number) => { return convertedVideoDir + "\\fileNameNo" + number + ".bmp"; };
            IMediaInfo info = await FFmpeg.GetMediaInfo(testFilePath).ConfigureAwait(false);
            IVideoStream videoStream = info.VideoStreams.First()?.SetCodec(VideoCodec.bmp);
            IConversionResult conversionResult = await FFmpeg.Conversions.New().AddStream(videoStream).ExtractEveryNthFrame(2, outputFileNameBuilder).SetOutput(convertedVideoDir).Start();

            /*//Turn images into Video
            List<string> files = Directory.EnumerateFiles(convertedVideoDir).ToList();
            //Conversion imageToVideo = new Conversion();
            //await imageToVideo.SetInputFrameRate(1).BuildVideoFromImages(files).SetInputFrameRate(1).SetFrameRate(25).SetOutput(Path.Combine(testFilePath, "outputfile.avi")).Start();
            //await FFmpeg.Conversions.New().Se
            //tInputFrameRate(1).BuildVideoFromImages(files).SetFrameRate(36).SetPixelFormat(PixelFormat.yuv420p).SetOutput(Path.Combine(testFilePath, "outputfile.mp4")).Start();
            await FFmpeg.Conversions.New().SetFrameRate(2.0).BuildVideoFromImages(files).SetOutput(Path.Combine(testFilePath, "outputFile")).SetPixelFormat(PixelFormat.yuv420p).SetOutputFormat(Format.avi).SetVideoBitrate(120).Start();
            */

            var files = Directory.GetFiles(convertedVideoDir);
            foreach (var file in files)
            {
                using(MemoryStream memStream = new MemoryStream())
                using (SKManagedWStream wstream = new SKManagedWStream(memStream))
                {
                    var imageStream = new FileStream(file, FileMode.Open);
                    SKBitmap resourceBitmap = SKBitmap.Decode(imageStream);
                    //The problem is the byte data of the picture is not read and converted to byte data correctly
                    Byte[] imageByteData = resourceBitmap.Bytes;
                    File.WriteAllBytes(Path.Combine(testOutputFolder, Path.GetFileName(file)), imageByteData);
                }
                
                
            }
        }

        private static void CreateTemporaryFolders(string outputFolder, out string convertedVideoDir, out string testOutputFolder, out string ffmpegFolder)
        {
            MakeDirectoryIfRequired(outputFolder);

            convertedVideoDir = Path.Combine(outputFolder, "Converted");
            MakeDirectoryIfRequired(convertedVideoDir);

            testOutputFolder = Path.Combine(outputFolder, "TEST");
            MakeDirectoryIfRequired(testOutputFolder);

            ffmpegFolder = Path.Combine(outputFolder, "FFmpeg");
            MakeDirectoryIfRequired(ffmpegFolder);
        }

        private static void MakeDirectoryIfRequired(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        private static string GetCurrentTime()
        {
            var currentTime = DateTime.Now.ToString();
            var timeWithoutSpace = currentTime.Split();
            var timeWithUnderScore = string.Join("_", timeWithoutSpace);
            var timeWithoutColon = timeWithUnderScore.Replace(':', '-');
            var timeWithoutSlash = timeWithoutColon.Replace('/', '-');
            return timeWithoutSlash;
        }
        private static void NumberCheck()
        {
            //Here have to change the number of bitmap images and the name is not matching with frame keys and saved frames
            for(int i = 0; i < 100; i++)
            {
                string paddedResult = i.ToString().PadLeft(3, '0');
                int val = Convert.ToInt32(paddedResult);
                Console.WriteLine(val);
            }
            
        }
    }
}


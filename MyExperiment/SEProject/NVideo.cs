using System.ComponentModel;
//using Emgu.CV;
using Xabe.FFmpeg;
using System.Collections.Generic;
//using System.Drawing;
using SkiaSharp;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyExperiment.SEProject
{
    /// <summary>
    /// File extension is required to create video file from Emgu library's
    /// VideoWriter class, Four character codec and file extension must be compatible
    /// </summary>
    enum CorrespondingFileExtension
    {
        //if new codec is used, put the file extension name in the Description(*) and then the codec with a , at last
        [Description(".mp4")]
        mp4v,

        [Description(".mp4")]
        H264,

        [Description(".avi")]
        MJPG,

    }

    /// <summary>
    /// To get the string correspondance of an enumerator value, wrapped up 
    /// class is requred as each enumerator itself is a class
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// It will give the corresponding string value of the enumeraotr value 
        /// </summary>
        /// <param name="value">while calling this method value is not required to be passed</param>
        /// <returns>Corresponding string value is returned</returns>
        public static string GetEnumDescription(this Enum value)
        {
            System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;

            else
                return value.ToString();
        }
    }

    /// <summary>
    /// <para>
    /// <br>Represent a single video, which contains</br>
    /// <br>Name of the video, but not including suffix format</br>
    /// <br>List of int[], with each int[] is a frame in chronological order start - end</br>
    /// <br>The Image can be scaled</br>
    /// </para>
    /// </summary>
    public class NVideo
    {
        public string name;
        public List<NFrame> nFrames;
        public string label;

        public readonly ColorMode colorMode;
        public readonly int frameWidth;
        public readonly int frameHeight;
        public readonly double frameRate;
        /// <summary>
        /// Generate a Video object
        /// </summary>
        /// <param name="videoPath">full path to the video</param>
        /// <param name="colorMode">Color mode to encode each frame in the Video, see enum VideoSet.ColorMode</param>
        /// <param name="frameHeight">height in pixels of the video resolution</param>
        /// <param name="frameWidth">width in pixels of the video resolution</param>
        public NVideo(string videoPath, string label, ColorMode colorMode, string imageDirectoryPath, int frameWidth, int frameHeight, double frameRate)
        {
            this.colorMode = colorMode;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.frameRate = frameRate;
            this.label = label;

            this.nFrames = new List<NFrame>();
            this.name = Path.GetFileNameWithoutExtension(videoPath);

            //Need to work here async is not finishing before going for the file reading
            //var videoReadingTask = ReadVideo(videoPath, imageDirectoryPath, frameRate);
            List<SKBitmap> fromBitmaps = WrapperMethod(videoPath, imageDirectoryPath, frameRate).Result;
            for (int i = 0; i < fromBitmaps.Count; i++)
            {
                //
                NFrame tempFrame = new NFrame(fromBitmaps[i], name, label, i+1, frameWidth, frameHeight, colorMode);
                nFrames.Add(tempFrame);
            }
        }
        private static async Task<List<SKBitmap>> WrapperMethod(string videoPath, string imageDirectory, double framerate = 0)
        {
            await ReadVideo(videoPath, imageDirectory, framerate);
            List<SKBitmap> sKBitmaps = ReadImageFiles(imageDirectory);
            return sKBitmaps;
        }
        /// <summary>
        /// <para>Method to read a video into a list of Bitmap, from video path to a list of Bitmap images</para>
        /// The current implementation uses OpenCV wrapper emgu
        /// </summary>
        /// <param name="videoPath"> full path of the video to be read </param>
        /// <returns>List of Bitmaps</returns>
        private static async Task ReadVideo(string videoPath, string imageDirectory, double framerate = 0)
        {
            //One Calculation required using frame rate how many frames are to be get from ffmpeg
            IMediaInfo videoInfo = await FFmpeg.GetMediaInfo(videoPath).ConfigureAwait(false);
            string videoFileName = Path.GetFileNameWithoutExtension(videoPath);
            IVideoStream videoStream = videoInfo.VideoStreams.First()?.SetCodec(VideoCodec.png);
            double framerateDefault = videoStream.Framerate;

            Func<string, string> outputFileNameBuilder = (number) => { return Path.Combine(imageDirectory + $"\\{videoFileName}_{videoFileName}_{number}.png"); };
            Task<IConversionResult> conversionResult = FFmpeg.Conversions.New().AddStream(videoStream).ExtractEveryNthFrame(2, outputFileNameBuilder).Start();
            await conversionResult.ConfigureAwait(true);            
        }

        private static List<SKBitmap> ReadImageFiles(string imageDirectory)
        {
            List<SKBitmap> videoBitmapArray = new List<SKBitmap>();
            //Need to Work here the files are not getting from the directory folder
            var files = Directory.GetFiles(imageDirectory);
            foreach (var file in files)
            {
                var imageStream = new FileStream(file, FileMode.Open);
                SKBitmap bitmapImage = SKBitmap.Decode(file);
                videoBitmapArray.Add(bitmapImage);
            }
            return videoBitmapArray;
        }
        /*public int[] GetEncodedFrame(string key)
        {
            foreach (NFrame nf in nFrames)
            {
                if (nf.FrameKey == key)
                {
                    return nf.EncodedBitArray;
                }
            }
            return new int[] { 4, 2, 3 };
        }*/

        /*/// <summary>
        /// Method to create video from Image Frames list
        /// </summary>
        /// <param name="bitmapList">indexing a list of objects for sorting,searching and manipulating</param>
        /// <param name="videoOutputPath">Folder path of the output of the video</param>
        /// <param name="frameRate">Rate of the frame in fps</param>
        /// <param name="dimension">Height & Width of Image Frames</param>
        /// <param name="isColor">Color of the images with boolean value True or False as colored or balck&white</param>
        /// <param name="codec">Coding decoding technique which requires four char values associated with VideoWriter.Fourcc method</param>
        public static void CreateVideoFromFrames(List<NFrame> bitmapList, string videoOutputPath, int frameRate, SKSize dimension, bool isColor, char[] codec = null)
        {
            //Set the default codec of fourcc
            if (codec == null)
            {
                codec = new char[] { 'm', 'p', '4', 'v' };
            }
            int fourcc = VideoWriter.Fourcc(codec[0], codec[1], codec[2], codec[3]);
            string codecString = new string(codec);
            CorrespondingFileExtension extension = (CorrespondingFileExtension)Enum.Parse(typeof(CorrespondingFileExtension), codecString, true);
            string fileExtension = extension.GetEnumDescription();
            var dimension1 = ConvertFromSKtoSystemDrawing_size(dimension);

            //There was a -1 instead of fourcc which works on older framework to bring the drop down menu selection of codec
            using (VideoWriter videoWriter = new VideoWriter(videoOutputPath + fileExtension, fourcc, (int)frameRate, dimension1, isColor))
            {
                foreach (NFrame frame in bitmapList)
                {
                    SKBitmap tempBitmap = frame.IntArrayToSKBitmap(frame.EncodedBitArray);
                    videoWriter.Write(ImageMaker.ImageToMat(tempBitmap));
                }
            }
        }*/

        /// <summary>
        /// This converts Skiasharp library image size into System Drawing library Image size
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        private static System.Drawing.Size ConvertFromSKtoSystemDrawing_size(SKSize dimension)
        {
            System.Drawing.Size res = new System.Drawing.Size((int)dimension.Width, (int)dimension.Height);
            return res;
        }
    }
}

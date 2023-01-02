using MyCloudProject.Common;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading;
using MyExperiment;
using System.Threading.Tasks;
using Xabe.FFmpeg.Downloader;
using Xabe.FFmpeg;
using System.IO;
using System.Reflection;

namespace MyCloudProject
{
    class Program
    {
        /// <summary>
        /// Your project ID from the last semester.
        /// </summary>
        private static string projectName = "ML21/22-Video Learning Project Migration";

        static void Main(string[] args)
        {
            //this is where the problem is finally. if the path is fixed the download is not done then ffmpeg does not work, if the path is not fixed and coppied in docker then permission gets denied in linux
            string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = System.IO.Path.GetDirectoryName(assemblyPath);
            string ffmpegDirectory = Path.Combine(assemblyDirectory, "ffmpegexecutable");

            DownloadFFMPEG(ffmpegDirectory).GetAwaiter().GetResult();

            CancellationTokenSource tokeSrc = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                tokeSrc.Cancel();
            };

            Console.WriteLine($"Started experiment: {projectName}");

            //init configuration
            var cfgRoot = Common.InitHelpers.InitConfiguration(args);

            var cfgSec = cfgRoot.GetSection("MyConfig");

            // InitLogging
            var logFactory = InitHelpers.InitLogging(cfgRoot);
            var logger = logFactory.CreateLogger("Train.Console");

            logger?.LogInformation($"{DateTime.Now} -  Started experiment: {projectName}");

            IFileStorageProvider storageProvider = new AzureBlobStorageProvider(cfgSec);

            Experiment experiment = new Experiment(cfgSec, storageProvider, projectName, logger/* put some additional config here */);

            experiment.RunQueueListener(tokeSrc.Token).Wait();

            logger?.LogInformation($"{DateTime.Now} -  Experiment exit: {projectName}");
        }

        /// <summary>
        /// This will download the ffmpeg dll file and set the ffmpeg executable path to it
        /// </summary>
        /// <param name="ffmpegFolder">Folder where the ffmpeg dll will be downloaded</param>
        /// <returns></returns>
        private static async Task DownloadFFMPEG(string ffmpegFolder)
        {
            if (!Directory.Exists(ffmpegFolder))
            {
                Directory.CreateDirectory(ffmpegFolder);
            }
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ffmpegFolder);
            FFmpeg.SetExecutablesPath(ffmpegFolder);
            Console.WriteLine($"Ffmpeg is downloaded in {ffmpegFolder} Directory");
        }

    }
}

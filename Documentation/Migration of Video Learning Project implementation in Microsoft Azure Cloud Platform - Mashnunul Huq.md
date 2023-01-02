## ML21/22-Video Learning Project Migration
## What is "Migration of Video Learning Project Implementation in Microsoft Azure Cloud Computing"
As cloud computing is the future of IT industry which offers accessibility, agility, scalability, and reliability in 
development and deployment of Applications in a cost effective way, migration of video learning project is implemented
in Microsoft's cloud computing platform called Azure. 

Containerization of an App is cost effective where no hardware configuration and software installations are required to
host a deployment can be done by using Docker. It is used to run multiple application in a single hardware inside their
isolated containers without interfering with files, memories and resources. 

In this project we used 6 features provided by Microsoft Azure platform.
 1. Azure Storage: It is the cloud storage with high security, scalability and accessability. It also has storage 
  abstractions including Blob Storage, File Storage, Queue Storage etc.

 2. Azure Blob Storage: It is an Object storage for cloud to store unstructured data which can provide images and documents
 directly to the browser, store files for distributed access and generate log files with backup and recovery options. Blob
 storage offers resources like storage account, container and blob.

 3. Azure Queue Storage: This offers the cloud messaging service between the application components for task management
 and process workflow management. The message Queue Storage stores can be accessable through HTTP or HTTPS calls and user
 can stack up multiple queue messages to run process sequentially and get results automatically without waiting for a 
 process to finish first.
 
 4. Azure Table Storage: It is also called previously as Cosmos DB Table API service which is a storage for structured data
 with schema less design. It can be utilized storing flixible datasheets for instant viewing the overview of a project via 
 metadata parsing as per service requirements.

 5. Azure Container Registry:  This is the hosting platform for docker images to store private docker container images and
 make it accessible for working publicly from anywhere. 

 6. Azure Container Instances: It is the virtual machine service that enables developer to deploy and run containers without 
 provisioning or managing docker image locally.

## Video Learning Project:
The HTM (Hierarchical Temporal Memory) is based on “Thousand Brains Theory” which explains how an object behaviors and 
high-level concepts gets tightly replicated across a cortical column but not only on the top layer and gets distributed 
throughout the neocortex. Here spatial pooler involves different computational principles of the cortex.It depends on 
competitive Hebbian learning, homeostatic excitability control, topology of connections in sensory cortices and structural 
plasticity. The HTM Spatial pooler is developed in such a way to achieve a set of computational properties which includes 
1. Preserving topology of the input space by mapping similar inputs to similar outputs 
2. Continuously adapting to changing statistics of the input stream 
3. Forming fixed sparsity representations 
4. Being robust to noise and 
5. Being fault tolerant that supports computations with SDRs (Sparse Distributed Representations). 
The output of the SP which is the integral component of HTM can be easily recognized by downstream neurons and contribute 
to improved performance in the end-to-end HTM system.
Sequence learning which indicates either generation, prediction or recognition is usually based on the models of legitimate 
sequences which can be developed through training with exemplars Hierarchical Temporal Memory proposed new computational 
learning models, Cortical Learning Algorithms (CLA), that is inspired from the neocortex which offer a better understanding 
of how our brains function. CLA mimics the procedure of human brain how to achieve pattern recognition and make intelligent 
predictions. The CLA processes the streams of information, classify them, learning to identify the differences and using 
time-based patterns to make predictions as like as performed by the neocortex in humans. But the place of time is significant 
in case of learning, inference and prediction. The temporal sequence is achieved from HTM algorithm from the stream of input 
data. Here Afterwards the result of the learning is tested by giving the trained model an arbitrary image, the model then 
attempts to recreate a video with proceeding frame after the input frame.

![nlunh](https://user-images.githubusercontent.com/59325753/185508893-2d4ceb44-57c0-4738-b8fa-535326bdeed6.PNG)
<p align="center"> Figure 1: Total Algorithm of Project</p>

## Creating Video Data Files
There are different ways to create training videos of object recognition but we chose to create our object videos using python 
OpenCV library. As we worked on the previous “neocortexapi-videolearning” project we had video data set for recognizing circle, 
triangle and rectangle. With the help of previous python codes we created training video-set for a line moving around the 120x120 
frame with a frame rate of 24 frames per second and the thickness of the object is 8. Videos can be found in the azure Blob 
Storage Container named "mashnunulinputcontainer" (https://portal.azure.com/#view/Microsoft_Azure_Storage/ContainerMenuBlade/~/overview/storageAccountId/%2Fsubscriptions%2F184881e3-59d0-4ddb-b7e5-1124023718e1%2Fresourcegroups%2Fmashnunulhuq%2Fproviders%2FMicrosoft.Storage%2FstorageAccounts%2Fvideolearningmigration/path/mashnunulinputcontainer/etag/%220x8DADBAEE468917E%22/defaultEncryptionScope/%24account-encryption-key/denyEncryptionScopeOverride~/false/defaultId//publicAccessVal/Container)

## Downloading Input Files
As the input files are in the Azure blob storage, the program needs to download the files. This is done with the help of class 
called AzureBlobStorageProvider inherited from abstract class named IFileStorageProvider. The methods for these input video files
is DownloadInputFileInFolder (as the run method requires data in different folders) and for configuration files is  DownloadInputFile.

~~~csharp
public async Task<string> DownloadInputFileInFolder(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("File name can not be null or empty");
            BlobServiceClient blobServiceClient = new BlobServiceClient(config.StorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(config.InputContainer);
            var blobClient = containerClient.GetBlobClient(fileName);
            string localStorageFileDirectory = Path.Combine(dataFolder, Path.GetFileNameWithoutExtension(fileName));
            if (!Directory.Exists(localStorageFileDirectory))
            {
                Directory.CreateDirectory(localStorageFileDirectory);
            }
            string localFilePath = Path.Combine(localStorageFileDirectory, new FileInfo(fileName).Name);
            using (var fileStream = File.Create(localFilePath))
            {
                await blobClient.DownloadToAsync(fileStream);
            }

            return localFilePath;
        }
~~~

~~~csharp
public async Task<string> DownloadInputFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("File name can not be null or empty");
            //string localStorageFilePath = Path.Combine(dataFolder, new FileInfo(fileName).Name);
            BlobServiceClient blobServiceClient = new BlobServiceClient(config.StorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(config.InputContainer);
            var blobClient = containerClient.GetBlobClient(fileName);
            string localStorageFilePath = Path.Combine(dataFolder, new FileInfo(fileName).Name);
            using (var fileStream = File.Create(localStorageFilePath))
            {
                await blobClient.DownloadToAsync(fileStream);
            }
            
            return localStorageFilePath;
        }
~~~

The project also needs input files for testing purpose and these files are downloaded from Azure blob container named "mashnunultestcontainer" (https://portal.azure.com/#view/Microsoft_Azure_Storage/ContainerMenuBlade/~/overview/storageAccountId/%2Fsubscriptions%2F184881e3-59d0-4ddb-b7e5-1124023718e1%2Fresourcegroups%2Fmashnunulhuq%2Fproviders%2FMicrosoft.Storage%2FstorageAccounts%2Fvideolearningmigration/path/mashnunultestcontainer/etag/%220x8DADBAF2CE54035%22/defaultEncryptionScope/%24account-encryption-key/denyEncryptionScopeOverride~/false/defaultId//publicAccessVal/Container)
with the help of method called DownloadInputTestFile of the same class. 

## Reading Video
To train a video to a machine learning program it has to be divided it into picture frames. As like brain, frames are the point of reference to the 
recognition program. As we are predicting future frames of an object or in plain language next move of the object according to it’s behavior we need 
to give as small distinctive data as possible to the program for reducing computational time. By VideoSet class from the supporting library of the 
project called SEProject we extract frames of each video.

As the previous videos of this training data set had the same frame rate as our line data the video configuration is not changed. We implemented a 
videoConfig.json file  for the ease of configuration changes to the videos implemented in the future for training. The training video rate is reduced 
to half of the original video frame rate(see Table 1)for making more frames and for computational ease of data to be introduced in the system the 
frame size is also reduced to 18x18 pixels. This can be changed by new users but the format must be the same mentioned below only values can be changed.

```json
{
  "frameWidth": 18,
  "frameHeight": 18,
  "frameRate": 12,
  "ColorMode": "BLACKWHITE"
}
```

As Previously we used EmguCV (openCV wrapper library for C#) for this purpose now we have changed the library to Xabe.ffmpeg (ffmpeg wrapper library for
for C#). Unlikely EmguCV, Xabe.ffmpeg does not have any in memory function (which means that it can not save data into computer's short term memory, it 
can only work directly and save as a file). So in this case we have to convert the video into frames and save as picture files. This is done in NVideo.cs

~~~csharp
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
~~~


## Converting Frames to Bitarrays
For learning in the spatial pooler and temporal memory each frame has to be binarized. By NFrame class we binarized each frame into binary array using 
BitmapToBinaryArray method. We used Black&White format to binarize each frame as our training videos are created based on Black and White color mode 
for ease of computational time required for processors. Pure or RGB color mode requires great amount of computational time as per our tests. When the 
predicted images are recreated IntArrayToBitmap method is used.

~~~csharp
private int[] BitmapToBinaryArray(SKBitmap image)
{
for (int heightCount = 0; heightCount < img.Height; heightCount++)
    {
     for (int widthCount = 0; widthCount < img.Width; widthCount++)
         {
          switch (colorMode)
              { imageBinary.Add((luminance > 255 / 2) ? 0 : 1);
                 imageBinary.AddRange(new List<int>() { (pixel.R > 255 / 2) ? 1 : 0, (pixel.G > 255 / 2) ? 1 : 0, (pixel.B > 255 / 2) ? 1 : 0 });
                 imageBinary.AddRange(ColorChannelToBinList(pixel.R));
                 imageBinary.AddRange(ColorChannelToBinList(pixel.G));
                 imageBinary.AddRange(ColorChannelToBinList(pixel.B));                              
               }
         }
    }
}
~~~

## Learning Stage
Now the binarized frames are sent to Spatial Pooler which operates on mini-columns to sensory inputs (for this case the movement of the object, 
it’s thickness, size, direction etc.) and learn spatial patterns by encoding the pattern into Sparse Distributed Representation (SDR). We can see 
from Figure the encoded bit array of circle 0 frame.



The created SDR which is the encoded spatial pattern of that object is used as the input to the Temporal Memory which learns about the patter 
when the spatial pooler is instable mode and removes the pattern when it is in unstable mode. SP oscillates between stable and unstable mode and 
the TM also learns and forgets about the pattern. But too much oscillation can cause permanent disruption to the program hence causing higher 
computational resources. To reduce this scenario we used homeostatic plasticity controller which influences excitation and inhibition balance of 
neurons. The functional stability of neural columns is achieved by SP and TM setting cells in active or predictive state. SP provides Global and 
Local inhibition which controls the number of cells must be activated in the currently processing area. To keep the stability of the Spatial Pooler 
and learning of TM a set of common parameters were selected while instantiating HTM ) and kept in the htmConfig.json file. Some of the configurations 
are manipulated while running the program in ModifyHtmFromCode method in the Main Program class.

```json
{
  "TemporalMemory": {},
  "SpatialPooler": {},
  "InhibitionRadius": 2,
  "InputTopology": null,
  "ColumnTopology": null,
  "NumInputs": 1,
  "NumColumns": 0,
  "PotentialRadius": 40,
  "PotentialPct": 0.5,
  "StimulusThreshold": 0.0,
  "SynPermBelowStimulusInc": 0.01,
  "SynPermInactiveDec": 0.01,
  "SynPermActiveInc": 0.1,
  "SynPermConnected": 0.1,
  "WrapAround": true,
  "GlobalInhibition": false,
  "LocalAreaDensity": 0.1,
  "SynPermTrimThreshold": 0.05,
  "SynPermMax": 1.0,
  "SynPermMin": 0.0,
  "InitialSynapseConnsPct": 0.5,
  "InputMatrix": null,
  "NumActiveColumnsPerInhArea": -1.0,
  "MinPctOverlapDutyCycles": 0.1,
  "MinPctActiveDutyCycles": 0.1,
  "PredictedSegmentDecrement": 0.1,
  "DutyCyclePeriod": 10,
  "MaxBoost": 30.0,
  "IsBumpUpWeakColumnsDisabled": false,
  "UpdatePeriod": 50,
  "OverlapDutyCycles": null,
  "ActiveDutyCycles": null,
  "MinOverlapDutyCycles": null,
  "MinActiveDutyCycles": null,
  "ColumnDimensions": [
    1024
  ],
  "CellsPerColumn": 16,
  "InputDimensions": [
    100,
    100
  ],
  "MaxNewSynapseCount": 20,
  "MaxSegmentsPerCell": 225,
  "MaxSynapsesPerSegment": 225,
  "PermanenceIncrement": 0.1,
  "PermanenceDecrement": 0.01,
  "ColumnModuleTopology": null,
  "InputModuleTopology": null,
  "Memory": null,
  "ActivationThreshold": 10,
  "LearningRadius": 10,
  "MinThreshold": 9,
  "InitialPermanence": 0.21,
  "ConnectedPermanence": 0.5,
  "RandomGenSeed": 42,
  "Name": null,
  "Random": {}
}
```

Now the boosting in spatial pooler makes sure that all columns are uniformly used across all seen patterns. As the mechanism remains active throughout 
the process the boosting of columns which already build learned SDRs is possible. Deactivation of boosting in homeostatic plasticity in the cortical 
layer can also be applied to SP. But the actual understanding to this is yet to be revealed. Till now in HTM this technique consists of boosting and 
inhibition algorithms which works on the minimum column level and not on the cell level in the minimum column. Because SP operates on the population of 
neural cells in minimum column rather than the individual cells. Therefore, the Spatial Pooler with the New-born Stage is used with the aim to send 
input pattern of SDR in each iteration to the homeostatic plasticity controller telling the program that SP has reached instable stage and program will 
disable the boosting. As the SP has entered to a stable state it will leave the new-born cycle and continue operating as usual without boosting which 
will help in reducing computational time.

For this project we used squential learning in the method called RunSoftwareEngineeringCode situated in the MyExperiment Library under the class called
Experiment.cs where we we put the frame key as HtmClassifier key while calling for the learn method. By the definition sequence learning takes more 
computational time as it learns by each frame.

~~~csharp
private List<string> RunSoftwareEngineeringCode(VideoConfig videoConfig, HtmConfig htmCfg, string[] videoPaths, string[] testFilePaths)
        {
            //New implementation of actual SE Project Code
            //String array to hold accuracy result files
            List<string> resultFilePaths = new List<string>();

            // Output folder initiation
            string outputFolder = nameof(Experiment.expectedProjectName) + "_" + GetCurrentTime();
            string convertedVideoDir, testOutputFolder;
            CreateTemporaryFolders(outputFolder, out convertedVideoDir, out testOutputFolder);
            HtmClassifier<string, ComputeCycle> cls = new();
            HomeostaticPlasticityController hpa = new(mem, maxNumOfElementsInSequence * 150 * 3, (isStable, numPatterns, actColAvg, seenInputs) =>{}, numOfCyclesToWaitOnChange: 50)
            SpatialPoolerMT sp = new(hpa);
            for (int i = 0; i < maxCycles; i++)
            {
                foreach (var currentFrame in nv.nFrames)
            {
                cls.Learn(currentFrame.FrameKey, actCells.ToArray());
                cls.Learn(key, actCells.ToArray()); //For TrainWithFrameKeys
            }
        }
~~~

## E.	Predicting Frames from inputfiles
After the learning we counted the accuracy of each learned video and put the output files named AccuracyLog_VidoeName.txt in the blob storage called
"mashnunuloutputcontainer" (https://portal.azure.com/#view/Microsoft_Azure_Storage/ContainerMenuBlade/~/overview/storageAccountId/%2Fsubscriptions%2F184881e3-59d0-4ddb-b7e5-1124023718e1%2Fresourcegroups%2Fmashnunulhuq%2Fproviders%2FMicrosoft.Storage%2FstorageAccounts%2Fvideolearningmigration/path/mashnunuloutputcontainer/etag/%220x8DADBAEFA943714%22/defaultEncryptionScope/%24account-encryption-key/denyEncryptionScopeOverride~/false/defaultId//publicAccessVal/Container)
Then we predicted from the images put in the blob storage container named "mashnunultestcontainer" (https://portal.azure.com/#view/Microsoft_Azure_Storage/ContainerMenuBlade/~/overview/storageAccountId/%2Fsubscriptions%2F184881e3-59d0-4ddb-b7e5-1124023718e1%2Fresourcegroups%2Fmashnunulhuq%2Fproviders%2FMicrosoft.Storage%2FstorageAccounts%2Fvideolearningmigration/path/mashnunultestcontainer/etag/%220x8DADBAF2CE54035%22/defaultEncryptionScope/%24account-encryption-key/denyEncryptionScopeOverride~/false/defaultId//publicAccessVal/Container)
We took the help of method called PredictImageInput in the Experiment.cs class to predict sequence and calculate prediction accuracy. This accuracies
can be found under the files named as "Predicted from InputImageFileName.txt". All these data is also persed to the table storage for better understanding 
and quick viewing in the table storage named as "codebreakersmashnunultable". To upload data in table we used the method called "InsertOrMergeEntityAsync"

~~~csharp
private async Task InsertOrMergeEntityAsync(ExperimentResult entity)
        {
            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(config.StorageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(config.ResultTable);
            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Created Table is called: {0}", config.ResultTable);
            }
            else
            {
                Console.WriteLine("Table name {0} already exists, returning the table", config.ResultTable);
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Insertion of row operation successful. Results uploaded in a table");
                }
            }
            catch (Exception storageException)
            {
                throw new Exception(storageException.Message);
            }
        }
~~~

As all the data is binarized, these images also needs to be binarized with NFrame class’s BitmapToBinaryArray method. While making the predicted future 
frames IntArrayToBitmap method of the same class is used and then combining all of these frames are done in NVideo class’s CreateVideoFromFrames method 
and saves those in a directory called convertedVideoDir but as these videos are so small that human can not understand without slowing 1000 times of a \
second, these files are not uploaded to the output folder. 

## How to run experiment
As the experiment is run firstly in the visual studio and found it's okay to run, a docker image is created and uploaded into the Azure container registry
called "codebreakersmashnunulregistry". One can download the docker image in a computer which has previously installed "docker for desktop" application
and run the command 

~~~csharp
docker run mycloudproject
~~~

Also there is a container instance created in the name of codebreakers which has a start and restart button. By pushing these buttons one can easily start
the experiment.
The application will stop for infinitive time to listen from the queuemessage and after puting a que message as mentioned bellow the program will read from 
the first queue message and deletes it after completing the whole run and again listens for new queue message.


```json
{
  "ExperimentId": "1",
  "InputFileCircle": "circle.mp4",
  "InputFileLine": "line.mp4",
  "InputFileRectangle": "rectangle.mp4",
  "InputFileTriangle": "triangle.mp4",
  "InputFileVideoConfig": "videoConfig.json",
  "InputFileHTMConfig": "htmConfig.json",
  "InputTestCircleFile": "Circle_circle_10.png",
  "InputTestLineFile": "Line_line_20.png",
  "InputTestRectangleFile": "Rectangle_rectangle_1.png",
  "InputTestTriangleFile": "Triangle_triangle_5.png",
  "Description": "Cloud Computing Implementation",
  "ProjectName": "ML21/22-Video Learning Project Migration",
  "GroupName": "codebreakersmashnunul",
  "Students": [ "mashnunul" ]
}
```
In the sample queue message one can change the inputFile names as uploaded in codebreakersmashnunulinpput blob storage before making the queue message but 
the keys at the left must be as it is. Also user can change the input test file names as changed files' names in the codebreakersmashnunultestdata blob 
storage. The program will verify the Project Name field if it matches with the actual experiment name (so it has to be same), otherwise the message will be
discarded as invalid. This will prevent the program from working on false queue messages.

In the appsettings.json file the storage connection string and name of the blob storages, queue storage and table storage is placed. If someone wants to modify
and add new storage account, must be a connection string of the new account be put in the field named StorageConnectionString and the corresponding blob storage
Queue storage and Tablestorage names should be filled accordingly.
```json
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },

  "MyConfig": {
    "GroupId": "codebreakersmashnunul",
    "StorageConnectionString": "DefaultEndpointsProtocol=https;AccountName=videolearningmigration;AccountKey=7bATDQ8EB4dbgfnwsJV+kqB4ef7EhM8WZZlnTY2l9XMrhPTarfzwmvp5mxMSlur3/74uAY9o6xrC+AStDtFy4A==;BlobEndpoint=https://videolearningmigration.blob.core.windows.net/;QueueEndpoint=https://videolearningmigration.queue.core.windows.net/;TableEndpoint=https://videolearningmigration.table.core.windows.net/;FileEndpoint=https://videolearningmigration.file.core.windows.net/;",
    "InputContainer": "mashnunulinputcontainer",
    "InputTestContainer": "mashnunultestcontainer",
    "OutputContainer": "mashnunuloutputcontainer",
    "ResultTable": "mashnunulresulttable",
    "Queue": "mashnunulinputque",
    "LocalPath": "mycloudproject-data"
  }
}
```
The information required to access the Azure container is given below:
|parameter||
|---------------	|--------------------------------------------------	|
|Resource Group| mashnunulhuq|
|Container Repository url |https://portal.azure.com/#@msdndaenet.onmicrosoft.com/resource/subscriptions/184881e3-59d0-4ddb-b7e5-1124023718e1/resourcegroups/mashnunulhuq/providers/Microsoft.Storage/storageAccounts/videolearningmigration/overview|
|connection string |DefaultEndpointsProtocol=https;AccountName=videolearningmigration;AccountKey=7bATDQ8EB4dbgfnwsJV+kqB4ef7EhM8WZZlnTY2l9XMrhPTarfzwmvp5mxMSlur3/74uAY9o6xrC+AStDtFy4A==;EndpointSuffix=core.windows.net|
|Blob Container Name of input file |mashnunulinputcontainer|
|URL of Container of input files  |https://portal.azure.com/#view/Microsoft_Azure_Storage/ContainerMenuBlade/~/overview/storageAccountId/%2Fsubscriptions%2F184881e3-59d0-4ddb-b7e5-1124023718e1%2Fresourcegroups%2Fmashnunulhuq%2Fproviders%2FMicrosoft.Storage%2FstorageAccounts%2Fvideolearningmigration/path/mashnunulinputcontainer/etag/%220x8DADBAEE468917E%22/defaultEncryptionScope/%24account-encryption-key/denyEncryptionScopeOverride~/false/defaultId//publicAccessVal/Container|
|Blob Container Name of input test files |mashnunultestcontainer|
|URL of Containr of input test files  |https://portal.azure.com/#view/Microsoft_Azure_Storage/ContainerMenuBlade/~/overview/storageAccountId/%2Fsubscriptions%2F184881e3-59d0-4ddb-b7e5-1124023718e1%2Fresourcegroups%2Fmashnunulhuq%2Fproviders%2FMicrosoft.Storage%2FstorageAccounts%2Fvideolearningmigration/path/mashnunultestcontainer/etag/%220x8DADBAF2CE54035%22/defaultEncryptionScope/%24account-encryption-key/denyEncryptionScopeOverride~/false/defaultId//publicAccessVal/Container|
|Blob Container Name of Output file |mashnunuloutputcontainer|
|URL of Container of output file  |https://portal.azure.com/#view/Microsoft_Azure_Storage/ContainerMenuBlade/~/overview/storageAccountId/%2Fsubscriptions%2F184881e3-59d0-4ddb-b7e5-1124023718e1%2Fresourcegroups%2Fmashnunulhuq%2Fproviders%2FMicrosoft.Storage%2FstorageAccounts%2Fvideolearningmigration/path/mashnunuloutputcontainer/etag/%220x8DADBAEFA943714%22/defaultEncryptionScope/%24account-encryption-key/denyEncryptionScopeOverride~/false/defaultId//publicAccessVal/Container|
|Container registry name |mashnunulregistry|
|URL of container registry ||
|Container instance name |mashnunulregistryinstance|
|URL of container instance ||
|Container Registry login url ||
|Container Registry Username ||
|Container Registry Password ||

## Result
The program runs with the input files of input blob container and gives output in the output blob storage as .txt files and in table storage as table entity.
But the whole project runs in the visual studio. When we create a docker image the files does not run and the same goes with the container registry and container
registry instance.

![InputContainer](https://user-images.githubusercontent.com/59325753/209949930-655b6350-dfad-4bc4-b953-66d585db6bd6.png)

<p align="center"> Figure 2: Input Blob Container "mashnunulinputcontainer" </p>

![TestContainer](https://user-images.githubusercontent.com/59325753/209949967-13e3d18d-104d-4b74-8bea-be591d2f67af.png)

<p align="center"> Figure 3: Input Test Blob Container "mashnunultestcontainer" </p>

![OutputContainer](https://user-images.githubusercontent.com/59325753/209950005-5fed4015-f474-4f01-9edc-df8736bb096c.png)

<p align="center"> Figure 4: Output Blob Container "mashnunuloutputcontainer" (output came of visual studio run)</p>

![Quemessage](https://user-images.githubusercontent.com/59325753/209950035-3fe9ff88-8bf1-4175-9ad1-d04b8e845698.png)

<p align="center"> Figure 5: Queue Storage Container "mashnunulinputque" </p>

![ResultTable](https://user-images.githubusercontent.com/59325753/209950052-f628996a-593c-4acf-9005-4782794d71ec.png)

<p align="center"> Figure 6: Output Table Storage Container "mashnunulresulttable" (output came of visual studio run) </p>

![2022-08-16 (10)](https://user-images.githubusercontent.com/59325753/185511294-57f66c7f-8abd-4da0-a7a3-f54c0c550060.png)

![2022-08-16 (11)](https://user-images.githubusercontent.com/59325753/185511037-aa6fd21c-3fbf-4f8c-81c2-591ae6d755c1.png)

<p align="center"> Figure 7: Successful run of code in visual studio with EmguCV </p>

This happens because Emgu team could not upload the Emgu.CV.windows.cuda library in nuget.org as it's size is 287mb and allowed size to be uploaded is 250mb (https://www.emgu.com/wiki/index.php/Download_And_Installation)
Now we tried to download the library manually from the provided link (https://github.com/emgucv/emgucv/releases/tag/4.5.3) and installed the nuget package in the project manually.

But while creating the docker iamge file the docker does not allow to install any packages intalled manually, only available packages in nuget.org get accepted. It also takes the
Emgu.CV.windows.cuda library of previous version which does not comply with new version of Emgu.CV Library. (Refer to issue number 238 of university repository)
![2022-08-16 (5)](https://user-images.githubusercontent.com/59325753/185511481-f45ca56f-8084-491f-bb62-f60ae99853d8.png)
<p align="center"> Figure 8: Emgu.CV error </p>

![2022-08-16 (4)](https://user-images.githubusercontent.com/59325753/185511824-46c41e4e-3a27-425c-b83a-95c6c86c1736.png)
<p align="center"> Figure 9: Docker image build fail with manual nuget package </p>

After this we have implemented ffmpeg wrapper class for C# called Xabe.ffmpeg. First we had to change in code a lot to work with this library into the previous algorithm. ImageMaker 
class had to be deleted, and in the main executing class has to add a new function to download ffmpeg executable files to make frames of the video.

~~~csharp
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
~~~

It runs locally in visual studio without any problem.

![Cloud Computing project works locally](https://user-images.githubusercontent.com/59325753/209950090-6fd0e8ee-2ea0-44e8-acc5-ea8beada79f6.png)

<p align="center"> Figure 10: Successful run of code in visual studio with Xabe.ffmpeg </p>

When we need to build the docker image first we needed to give the permission to run the files with execution permission of ffmpeg exe file in the DockerFile. (General convension 
of linux)

~~~csharp
COPY ["MyCloudProject/ffmpegexecutable/", "ffmpegexecutable"]

RUN chmod -R 777 /app
RUN chmod -R 777 /app/ffmpegexecutable
RUN chmod -R 777 /app/ffmpegexecutable/ffprobe.exe
~~~

In this case the docker file builds up successfully but does not run properly showing the error that Xabe.ffmpeg is unable to recognize .mp4 extension type. As Xabe.ffmpeg
was first built on dotnet5 when dotnet6 was implemented in full scale they could not yet update the documentation of the library (https://ffmpeg.xabe.net/docs.html).
Most of their given methods does not work accordingly. In their github issues everyone is finding issue on running Xabe.ffmpeg in linux based system and they are working
on it. In this case the ffmpeg executables are not coping up with the wrapper class Xabe.ffmpeg fully and giving error.

![ffmpeg problem at docker](https://user-images.githubusercontent.com/59325753/209950150-a797a559-d5ba-4b61-a8b7-7cb9842cd4a9.png)

<p align="center"> Figure 11: Error running Docker File with Xabe.ffmpeg </p>

Now if we put try catch error where we have used ffmpeg to make frames, the program circumvents that portion and goes to infinite loop.

~~~csharp
private static async Task ReadVideo(string videoPath, string imageDirectory, double framerate = 0)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }           
        }
~~~

If the problem within Xabe.ffmpeg solves for Linux system then this program will work on container registry. We have built container registry and container instance, but as it is currently not working in docker image because of linux build up issue we deleted the registry and its instance to save the subscription cost.

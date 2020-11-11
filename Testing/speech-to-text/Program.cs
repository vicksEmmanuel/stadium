using System;
using System.Threading;
using System.IO;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;
using System.Diagnostics;
// using Google.Cloud.Speech.V1;

namespace speech_to_text
{
    class Program
    {   
        async static Task FromMic(SpeechConfig speechConfig)
        {
            var checker = true;

            while(checker) {
                using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
                using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

                Console.WriteLine("Speak into your microphone.");
                var result = await recognizer.RecognizeOnceAsync();
                Console.WriteLine($"RECOGNIZED: Text={result.Text}");

                if(result.Text == "end." || result.Text == "End.") {
                    break;
                }
            }
        }

        async static Task FromFile(SpeechConfig speechConfig)
        {
            using var audioConfig = AudioConfig.FromWavFileInput(DEMO_FILE);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }

        async static Task FromStream(SpeechConfig speechConfig)
        {
            var reader = new BinaryReader(File.OpenRead(DEMO_FILE));
            Console.WriteLine(reader.ToString());
            using var audioInputStream = AudioInputStream.CreatePushStream();
            using var audioConfig = AudioConfig.FromStreamInput(audioInputStream);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            byte[] readBytes;
            do
            {
                readBytes = reader.ReadBytes(1024);
                audioInputStream.Write(readBytes, readBytes.Length);
            } while (readBytes.Length > 0);

            var result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }

        public static string DEMO_FILE = "";
        
        public static string ConvertMp3ToWav(String inputMp3FilePath, String outputWavFilePath)  
        {  
            var ffmpegLibWin = "ffmpeg\\bin\\ffmpeg.exe"; //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg\\lib", "ffmpeg.exe");  
            // var ffmpegLibLnx = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", "ffmpeg");  
            // String procPath = ffmpegLibWin;  
            // if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))  
            // {  
            //     procPath = ffmpegLibLnx;  
            // }  
            var process = Process.Start(ffmpegLibWin, $" -i {Path.GetFullPath(inputMp3FilePath)} -ac 1 -ar 22050 {Path.GetFullPath(outputWavFilePath)}");  
            process.WaitForExit();  
            Console.WriteLine(Path.GetFullPath(outputWavFilePath));
            return Path.GetFullPath(outputWavFilePath);
        }  
        
        public async static Task Main(string[] args)
        {
            // var speech = SpeechClient.Create();
            // var response = speech.Recognize(new RecognitionConfig()
            // {
            //     Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
            //     SampleRateHertz = 16000,
            //     LanguageCode = "en",
            // }, RecognitionAudio.FromFile(DEMO_FILE));
            // foreach (var result in response.Results)
            // {
            //     foreach (var alternative in result.Alternatives)
            //     {
            //         Console.WriteLine(alternative.Transcript);
            //     }
            // }

            DEMO_FILE = ConvertMp3ToWav("Recording.m4a","Recording2.wav");

            DotNetEnv.Env.Load(".env");
            var speechConfig = SpeechConfig.FromSubscription(DotNetEnv.Env.GetString("SPEECH_TO_TEXT_KEY"), DotNetEnv.Env.GetString("SPEECH_TO_TEXT_REGION_KEY"));
            await FromStream(speechConfig);
            
        }
    }
}

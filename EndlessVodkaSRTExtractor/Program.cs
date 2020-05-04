using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SRTExtract
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello and welcome to the most glorious SRT extractor the world has ever seen!");
            Console.WriteLine();

            Console.WriteLine("Behold, the mighty >>> Endless Vodka SRT Converter <<< ");
            Console.WriteLine();

            string currentDirectory = Directory.GetCurrentDirectory();
            string? ffmpegOrigin    = Directory.GetFiles(currentDirectory, searchPattern: "ffmpeg.exe", searchOption: SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (string.IsNullOrEmpty(ffmpegOrigin))
            {
                Console.WriteLine("ffmpeg.exe not found!");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Enter a directory that contains your MKV files, or leave blank to use the current directory:");
            Console.WriteLine();

            string inputDirectory = Console.ReadLine();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string workingDirectory;

            if (string.IsNullOrEmpty(inputDirectory))
            {
                workingDirectory = currentDirectory;
            }
            else
            {
                workingDirectory = inputDirectory;
            }

            if (Directory.Exists(workingDirectory) == false)
            {
                Console.WriteLine("Directory doesn't exist");
                Console.ReadLine();
                return;
            }

            if (workingDirectory != currentDirectory)
            {
                bool success = TryCopyFile(ffmpegOrigin, workingDirectory);
                if (success == false) { return; }
            }

            IEnumerable<string> files = Directory.GetFiles(path: workingDirectory,
                                                           searchPattern: "*.mkv",
                                                           searchOption: SearchOption.TopDirectoryOnly);

            Console.WriteLine($"Found {files.Count()} .mkv files");
            Console.WriteLine();


            foreach (string file in files)
            {
                Console.WriteLine($"Starting to process \"{Path.GetFileName(file)}\"");
                Console.WriteLine();

                string subtitleFile = $"{file.Substring(0, file.Length - 4)}.srt";
                Console.WriteLine($"Subtitle file name: \"{subtitleFile}\"");
                Console.WriteLine();

                string subtitleFilePath = Path.Combine(workingDirectory, subtitleFile);

                string command = $"/C ffmpeg -i \"{file}\" \"{subtitleFilePath}\"";
                Console.WriteLine($"Command: \"{command}\"");
                Console.WriteLine();

                Process process = Process.Start("CMD.exe", command);
                process.WaitForExit();
            }

            if (workingDirectory != currentDirectory)
            {
                bool success = TryDeleteCopiedFile(ffmpegOrigin, workingDirectory);
                if (success == false) { return; }
            }

            stopwatch.Stop();

            TimeSpan elapsedTime = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);

            string hours    = elapsedTime.Hours     >= 10 ? $"{elapsedTime.Hours}"      : $"0{elapsedTime.Hours}";
            string minutes  = elapsedTime.Minutes   >= 10 ? $"{elapsedTime.Minutes}"    : $"0{elapsedTime.Minutes}";
            string seconds  = elapsedTime.Seconds   >= 10 ? $"{elapsedTime.Seconds}"    : $"0{elapsedTime.Seconds}";

            Console.WriteLine($"Finished - elapsed time: {hours}:{minutes}:{seconds}");
            Console.WriteLine();

            Console.ReadLine();
        }


        private static bool TryCopyFile(string originFilePath, string destinationDirectory)
        {
            string file                 = Path.GetFileName(originFilePath);
            string destinationFilePath  = Path.Combine(destinationDirectory, file);

            try
            {
                File.Copy(originFilePath, destinationFilePath, overwrite: true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                return false;
            }

            Console.WriteLine($"Copied {file}");
            Console.WriteLine();
            return true;
        }

        private static bool TryDeleteCopiedFile(string originFilePath, string destinationDirectory)
        {
            string file                 = Path.GetFileName(originFilePath);
            string destinationFilePath  = Path.Combine(destinationDirectory, file);

            try
            {
                File.Delete(destinationFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                return false;
            }

            Console.WriteLine($"Deleted {file}");
            Console.WriteLine();
            return true;
        }
    }
}

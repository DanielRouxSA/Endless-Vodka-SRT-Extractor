using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EndlessVodkaSRTExtractor
{
    class Program
    {
        static void Main()
        {
            CustomConsole.WriteLineWithBreak("Hello and welcome to the most glorious SRT extractor the world has ever seen!");

            CustomConsole.WriteLineWithBreak("Behold, the mighty >>> Endless Vodka SRT Converter <<< ");

            string currentDirectory = Directory.GetCurrentDirectory();
            string? ffmpegOrigin    = Directory.GetFiles(currentDirectory, searchPattern: "ffmpeg.exe", searchOption: SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (string.IsNullOrEmpty(ffmpegOrigin))
            {
                CustomConsole.WriteLineWithBreak("ffmpeg.exe not found!");
                return;
            }

            CustomConsole.WriteLineWithBreak("Enter a directory that contains your MKV files, or leave blank to use the current directory:");

            string? inputDirectory = Console.ReadLine();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string workingDirectory;

            if (string.IsNullOrWhiteSpace(inputDirectory))
            {
                workingDirectory = currentDirectory;
            }
            else
            {
                workingDirectory = inputDirectory;
            }

            if (Directory.Exists(workingDirectory) == false)
            {
                CustomConsole.WriteLineWithBreak("Directory doesn't exist");
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

            CustomConsole.WriteLineWithBreak($"Found {files.Count()} .mkv files");


            foreach (string file in files)
            {
                CustomConsole.WriteLineWithBreak($"Starting to process \"{Path.GetFileName(file)}\"");

                string subtitleFile = $"{file.Substring(0, file.Length - 4)}.srt";
                CustomConsole.WriteLineWithBreak($"Subtitle file name: \"{subtitleFile}\"");

                string subtitleFilePath = Path.Combine(workingDirectory, subtitleFile);

                string command = $"/C ffmpeg -i \"{file}\" \"{subtitleFilePath}\"";
                CustomConsole.WriteLineWithBreak($"Command: \"{command}\"");

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

            CustomConsole.WriteLineWithBreak($"Finished - elapsed time: {hours}:{minutes}:{seconds}");

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
                CustomConsole.WriteLineWithBreak(e.Message);
                return false;
            }

            CustomConsole.WriteLineWithBreak($"Copied {file}");
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
                CustomConsole.WriteLineWithBreak(e.Message);
                return false;
            }

            CustomConsole.WriteLineWithBreak($"Deleted {file}");
            return true;
        }
    }
}

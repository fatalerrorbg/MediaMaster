using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster.Converter
{
    //TODO ADD EVENTS
    public class MediaConverter
    {
        public string FfmpegDelployPath { get; private set; }
        public string FfmpegFileName { get; private set; }

        public static void ClearAllExistingProcesses()
        {
            Process.GetProcessesByName("ffmpeg").ToList().ForEach(x => x.Kill());
        }

        public MediaConverter(string ffmpegDeployPath)
        {
            this.FfmpegDelployPath = ffmpegDeployPath;
            this.FfmpegFileName = "ffmpeg.exe";
            this.EnsureFfmpeg();
        }

        public MediaConverter()
            : this(Path.Combine(Path.GetTempPath(), "Ffmpeg"))
        {
        }

        public FileInfo ConvertSingleFile(string inputFile, string outputPath, MediaConverterMetadata metadata)
        {
            if (File.Exists(outputPath))
            {
                try
                {
                    File.Delete(outputPath);
                }
                catch(Exception ex)
                {
                    return new FileInfo(outputPath);
                }
            }

            string extension = Path.GetExtension(outputPath);
            Task<Process> conversionTask = Task.Factory.StartNew<Process>(() =>
                {
                    string parameters = null;
                    switch (extension)
                    {
                        case ".mp3":
                             parameters = string.Format("-i \"{0}\" -ab {1}k \"{2}\"", inputFile, (int)metadata.AudioBitrate, outputPath);
                            break;
                        default:
                            break;
                    }

                    return this.StartNewFfmpegInstance(parameters);
                });

            Task.WaitAll(conversionTask);

            return new FileInfo(outputPath);
        }

        protected virtual Process StartNewFfmpegInstance(string parameters)
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo(Path.Combine(this.FfmpegDelployPath, this.FfmpegFileName), parameters)
                {
                    CreateNoWindow = true,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                };

            process.Start();
            process.WaitForExit();

            return process;
        }

        public void EnsureFfmpeg()
        {
            string resourcePath = string.Format("{0}.{1}", this.GetType().Namespace, this.FfmpegFileName);
            string deployPath = Path.Combine(this.FfmpegDelployPath, this.FfmpegFileName);

            if (!Directory.Exists(this.FfmpegDelployPath))
            {
                Directory.CreateDirectory(FfmpegDelployPath);
            }

            if (File.Exists(deployPath))
            {
                return;
            }

            using (Stream exeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
            {
                byte[] buffer = new byte[exeStream.Length];
                exeStream.Read(buffer, 0, (int)exeStream.Length);

                File.WriteAllBytes(deployPath, buffer);
            }
        }
    }
}

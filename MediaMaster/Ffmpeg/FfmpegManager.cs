using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster.Ffmpeg
{
    public class FfmpegManager
    {
        public const string Ffmpeg = "ffmpeg";

        #region Singleton
        private static FfmpegManager instance;
        private static object syncRoot = new object();

        public static FfmpegManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new FfmpegManager();
                        }
                    }   
                }

                return instance;
            }
        }
        #endregion

        public string FfmpegDelployPath { get; private set; }
        public string FfmpegFileName { get; private set; }

        public void ClearAllExistingProcesses()
        {
            Process.GetProcessesByName(Ffmpeg).ToList().ForEach(x => x.Kill());
        }

        public FfmpegManager()
            : this(Path.Combine(Path.GetTempPath(), Ffmpeg))
        {
        }

        public FfmpegManager(string ffmpegDeployPath)
        {
            this.FfmpegDelployPath = ffmpegDeployPath;
            this.FfmpegFileName = Ffmpeg + ".exe";
            this.EnsureFfmpeg();
        }

        public virtual Process CreateNewFfmpegInstance(string parameters)
        {
            this.EnsureFfmpeg();

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

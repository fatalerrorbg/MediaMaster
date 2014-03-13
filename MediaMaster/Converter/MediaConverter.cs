using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MediaMaster.Converter;
using MediaMaster.Ffmpeg;

namespace MediaMaster
{
    public class MediaConverter
    {
        public virtual ConvertResult Convert(MediaFile inputFile, string inputFilePath, string outputFolder, MediaConverterMetadata metadata)
        {
            ConvertResult result = new ConvertResult(inputFile);
            if (metadata == MediaConverterMetadata.Default)
            {
                result.IsConverted = false;
                return result;
            }

            result.IsConverted = true;
            string destinationPath = Path.Combine(outputFolder, metadata.FileName + metadata.Extension);
            result.ConvertedPath = destinationPath;
            if (File.Exists(destinationPath))
            {
                try
                {
                    File.Delete(destinationPath);
                }
                catch (Exception ex)
                {
                    result.IsConverted = false;
                    result.Exceptions.Add(ex);
                    return result;
                }
            }

            string extension = metadata.Extension.Value;
            if (!this.OnMediaFileConversionStarting(inputFile, metadata))
            {
                result.IsConverted = false;
                return result;
            }

            string parameters = null;
            SupportedConversionFormats format = SupportedConversionFormats.Parse(extension);
            if (format == SupportedConversionFormats.Mp3)
            {
                parameters = string.Format("-i \"{0}\" -ab {1}k \"{2}\"", inputFilePath, (int)metadata.AudioBitrate, destinationPath);
            }

            Process instance = FfmpegManager.Instance.CreateNewFfmpegInstance(parameters);
            instance.Start();

            this.ProcessOutputStream(instance.StandardError);

            instance.WaitForExit();
            if (!instance.HasExited)
            {
                instance.Kill();
            }

            this.OnMediaFileConvertionComplete(inputFile, metadata);

            return result;
        }

        private void ProcessOutputStream(StreamReader reader)
        {
            //TODO: 
            /*{
                        while (!reader.EndOfStream)
                        {
                            string line = instance.StandardError.ReadLine() + Environment.NewLine;
                        }   
                    }*/
        }

        #region Events
        public event EventHandler<MediaFileConversionEventArgs> MediaFileConversionStarting;

        protected virtual bool OnMediaFileConversionStarting(MediaFile mediaFile, MediaConverterMetadata outputMetadata)
        {
            if (this.MediaFileConversionStarting != null)
            {
                var args = new MediaFileConversionEventArgs(mediaFile, outputMetadata);
                this.MediaFileConversionStarting(this, args);
                return !args.Cancel;
            }

            return true;
        }

        public event EventHandler<MediaFileConversionEventArgs> MediaFileConvertionCompelete;

        protected virtual void OnMediaFileConvertionComplete(MediaFile mediaFile, MediaConverterMetadata outputMetadata)
        {
            if (this.MediaFileConvertionCompelete != null)
            {
                this.MediaFileConvertionCompelete(this, new MediaFileConversionEventArgs(mediaFile, outputMetadata));
            }
        }
        #endregion
    }
}

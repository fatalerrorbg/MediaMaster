using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MediaMaster
{
    public class MediaFile
    {
        private MediaFileMetadata metadata;

        public FileOrigin FileOrigin { get; private set; }

        public string Url { get; private set; }

        public virtual bool IsValid { get { return true; } }

        public bool InitializeMetadataOnDemand { get; private set; }

        public MediaFileMetadata Metadata
        {
            get
            {
                if (this.metadata == null)
                {
                    this.metadata = this.InitializeMetadata();
                }

                return this.metadata;
            }
        }

        public MediaFile(string url, FileOrigin fileOrigin, bool initializeMetadataOnDemand = true)
        {
            this.Url = url;
            this.FileOrigin = fileOrigin;
            this.InitializeMetadataOnDemand = initializeMetadataOnDemand;
            if (!this.InitializeMetadataOnDemand)
            {
                this.metadata = this.InitializeMetadata();
            }
        }

        protected virtual MediaFileMetadata InitializeMetadata()
        {
            return MediaFileMetadata.DefaultMetadata;
        }

        //public virtual MediaFileMetadata Metadata
        //{
        //    switch (this.FileOrigin)
        //    {
        //        case FileOrigin.Vbox7:
        //            return (this as VboxFile).Metadata;
        //        default:
        //            return null;
        //    }
        //}

        public static MediaFile CreateNew(string url)
        {
            if (url == null)
            {
                return null;
            }

            FileOrigin urlOrigin = ParseFileOrigin(url);
            switch (urlOrigin)
            {
                case FileOrigin.Vbox7:
                    return new VboxFile(url);
                default:
                    throw new NotSupportedException("Url " + url + "Is not supported");
            }
        }

        public static FileOrigin ParseFileOrigin(string url)
        {
            if (url.Contains(Constants.Vbox7))
            {
                return FileOrigin.Vbox7;
            }

            return FileOrigin.NotSupported;
        }
    }

    //public abstract class MediaFile<TMetadata> : 
    //    MediaFile,
    //    IMediaFileWithMetadata<TMetadata>
    //    where TMetadata : MediaFileMetadata
    //{
    //    private TMetadata metadata;

    //    public MediaFile(string url, FileOrigin fileOrigin, bool initializeMetadataOnDemand = true)
    //        :base(url, fileOrigin)
    //    {
    //        this.InitializeMetadataOnDemand = initializeMetadataOnDemand;
    //        if (!this.InitializeMetadataOnDemand)
    //        {
    //            this.ReInitializeMetadata();
    //        }
    //    }

    //    public TMetadata Metadata
    //    {
    //        get
    //        {
    //            if (this.metadata == null)
    //            {
    //                this.metadata = this.InitializeMetadata();
    //            }

    //            return this.metadata;
    //        }
    //    }


    //    protected abstract TMetadata InitializeMetadata();

    //    public void ReInitializeMetadata()
    //    {
    //        this.metadata = this.InitializeMetadata();
    //    }
    //}
}
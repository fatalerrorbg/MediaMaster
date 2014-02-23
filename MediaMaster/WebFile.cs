using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MediaMaster
{
    public abstract class WebFile
    {
        public FileOrigin FileOrigin { get; private set; }

        public string Url { get; private set; }

        public virtual bool IsValid
        {
            get
            {
                return true;
            }
        }
        public WebFile(string url, FileOrigin fileOrigin)
        {
            this.Url = url;
            this.FileOrigin = fileOrigin;
        }

        public virtual WebFileMetadata GetMetadata()
        {
            switch (this.FileOrigin)
            {
                case FileOrigin.Vbox7:
                    return (this as VboxFile).Metadata;
                default:
                    return null;
            }
        }

        public static WebFile CreateNew(string url)
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
                    return null;
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

    public abstract class WebFile<TMetadata> : 
        WebFile,
        IWebFileWithMetadata<TMetadata>
        where TMetadata : WebFileMetadata
    {
        private TMetadata metadata;

        public WebFile(string url, FileOrigin fileOrigin, bool initializeMetadataOnDemand = true)
            :base(url, fileOrigin)
        {
            this.InitializeMetadataOnDemand = initializeMetadataOnDemand;
            if (!this.InitializeMetadataOnDemand)
            {
                this.ReInitializeMetadata();
            }
        }

        public TMetadata Metadata
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

        public bool InitializeMetadataOnDemand { get; private set; }

        protected abstract TMetadata InitializeMetadata();

        public void ReInitializeMetadata()
        {
            this.metadata = this.InitializeMetadata();
        }
    }
}
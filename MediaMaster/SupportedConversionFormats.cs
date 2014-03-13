using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public sealed class SupportedConversionFormat
    {
        public static readonly SupportedConversionFormat None = new SupportedConversionFormat("", "");
        public static readonly SupportedConversionFormat Mp3 = new SupportedConversionFormat("Mp3", ".mp3");
        public static readonly SupportedConversionFormat Mp4 = new SupportedConversionFormat("Mp4", ".mp4");
        public static readonly SupportedConversionFormat Flv = new SupportedConversionFormat("Flv", ".flv");
        public static readonly SupportedConversionFormat Avi = new SupportedConversionFormat("Avi", ".avi");

        public string Name { get; private set; }

        public string Value { get; private set; }
        
        private SupportedConversionFormat (string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        private SupportedConversionFormat(string nameValue)
            : this(nameValue, nameValue)
        {
        }

        public override string ToString()
        {
            return this.Value;
        }

        public static SupportedConversionFormat Parse(string extension)
        {
            FieldInfo[] fields = typeof(SupportedConversionFormat).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                var format = field.GetValue(null) as SupportedConversionFormat;
                string value = format.Value;
                if (string.Equals(value, extension, StringComparison.InvariantCultureIgnoreCase))
                {
                    return format;
                }
            }

            throw new InvalidCastException("Not supported conversion format " + extension);
        }
    }
}

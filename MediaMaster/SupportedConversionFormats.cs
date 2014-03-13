using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster
{
    public sealed class SupportedConversionFormats
    {
        public static readonly SupportedConversionFormats None = new SupportedConversionFormats("", "");
        public static readonly SupportedConversionFormats Mp3 = new SupportedConversionFormats("Mp3", ".mp3");
        public static readonly SupportedConversionFormats Mp4 = new SupportedConversionFormats("Mp4", ".mp4");
        public static readonly SupportedConversionFormats Flv = new SupportedConversionFormats("Flv", ".flv");
        public static readonly SupportedConversionFormats Avi = new SupportedConversionFormats("Avi", ".avi");

        public string Name { get; private set; }

        public string Value { get; private set; }
        
        private SupportedConversionFormats (string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        private SupportedConversionFormats(string nameValue)
            : this(nameValue, nameValue)
        {
        }

        public override string ToString()
        {
            return this.Value;
        }

        public static SupportedConversionFormats Parse(string extension)
        {
            FieldInfo[] fields = typeof(SupportedConversionFormats).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                var format = field.GetValue(null) as SupportedConversionFormats;
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

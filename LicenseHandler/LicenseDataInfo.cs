using System;

namespace Bitboxx.License
{
    public class LicenseDataInfo
    {
        public LicenseDataInfo(string tag, string prefix, int modules, byte workstations, byte version, DateTime? validUntil)
        {
            Prefix = prefix;
            Modules = modules;
            Workstations = workstations;
            Version = version;
            ValidUntil = validUntil;
            Tag = tag;
        }
        public LicenseDataInfo()
        {
            Prefix = "";
            Modules = 0;
            Workstations = 0;
            Version = 1;
            ValidUntil = null;
            Tag = "";
        }
        public string Prefix { get; set; }
        public int Modules { get; set; }
        public byte Workstations { get; set; }
        public byte Version { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string Tag { get; set; }
    }
}

using System.Collections.Generic;
using System;
using System.Linq;
using Shared.Enums;
using Ui.Pocos;
using Ui.Enums;

namespace Ui.Static
{
    public static class ScanSpells
    {
        public static OrbitalScan SimpleScan = new() {
            ScanType = ScanType.SimpleScan,
            Title = "Simple Scan",
            Endpoint = "Scans",
            Description = "Scan a payload with a single antivirus",
            Image = "images/simple_scan.png",
            SupportedPayloads = Enum.GetValues(typeof(PayloadType)).Cast<PayloadType>().ToList(),
            ConfigurationNeeded = new List<SpellConfiguration> { SpellConfiguration.AntivirusToUse }
        };

        public static OrbitalScan ScanWithAll = new() {
            ScanType = ScanType.ScanWithAll,
            Title = "Scan all",
            Endpoint = "Scans",
            Description = "Scan a payload with every supported antiviruses",
            Image = "images/offline_virustotal.png",
            SupportedPayloads = Enum.GetValues(typeof(PayloadType)).Cast<PayloadType>().ToList()
        };

        public static OrbitalScan Dissection = new() {
            ScanType = ScanType.Dissection,
            Title = "Dissection",
            Endpoint = "Dissections",
            Description = "Statically analyze every functions of a payload separately, and returns which ones are flagged, and which ones are healthy",
            Image = "images/dissect.png",
            SupportedPayloads = new List<PayloadType> { PayloadType.NativeExecutable, PayloadType.NativeLibrary },
            ConfigurationNeeded = new List<SpellConfiguration> 
                { SpellConfiguration.AntivirusToUse, SpellConfiguration.FunctionsToScan, SpellConfiguration.NumberOfDockers }
        };

        public static List<OrbitalScan> AllScanSpells => new List<OrbitalScan>() { ScanWithAll, SimpleScan, Dissection };
    }

}
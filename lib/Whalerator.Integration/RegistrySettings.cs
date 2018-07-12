﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Whalerator.Integration
{
    internal class RegistrySettings
    {
        public static RegistrySettings FromFile(string filename) =>
            new YamlDotNet.Serialization.Deserializer().Deserialize<RegistrySettings>(System.IO.File.ReadAllText(filename));

        [YamlDotNet.Serialization.YamlMember(Alias = "registry")]
        public string Registry { get; set; }
        [YamlDotNet.Serialization.YamlMember(Alias = "user")]
        public string User { get; set; }
        [YamlDotNet.Serialization.YamlMember(Alias = "password")]
        public string Password { get; set; }
        [YamlDotNet.Serialization.YamlMember(Alias = "repository")]
        public string Repository { get; set; }
    }
}

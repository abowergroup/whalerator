﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MobyInspector.Data
{
    public class ImageConfig
    {
        public string Architecture { get; set; }
        public ContainerConfig Config { get; set; }
        public string Container { get; set; }
        public ContainerConfig Container_config { get; set; }
        public DateTime Created { get; set; }
        public string Docker_Version { get; set; }
        public IEnumerable<History> History { get; set; }
        public string OS { get; set; }
        [JsonProperty("os.version")]
        public string OSVersion { get; set; }
        /*"rootfs": {
            "type": "layers",
            "diff_ids": [
                "sha256:e1df5dc88d2cc2cd9a1b1680ec3cb92a2dc924a0205125d85da0c61083b4e87d",
                "sha256:9c2a08e1e47a6c26845a5d16322cee940e52b47e35b0624098212c7aced10869",
                "sha256:f9d40f369825282cf6a599c5f348a0e3784008b401f307cff35a7db48c4c0e52",
                "sha256:b196c7c99e82f2eb35e69cfa5cd1effdee931c59f2966a1d7cbfeab7a1abe1b4"
            ]
        }*/

    }
}

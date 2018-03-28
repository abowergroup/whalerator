﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MobyInspector.Model
{
    public class ImageSet
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public IEnumerable<Image> Images { get; set; }
    }
}

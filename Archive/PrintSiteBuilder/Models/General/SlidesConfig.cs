using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.Models.General
{
    public class SlidesConfig
    {
        public SlideConfig CoverSlideConfig { get; set; }
        public SlideConfig AmazonSlideConfig { get; set; }
        public SlideConfig PrintSlideConfig { get; set; }
    }
}

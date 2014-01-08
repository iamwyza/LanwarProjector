using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanwarProjector.Common.DTO;

namespace LanwarProjector.Queue
{
    public class Config
    {
        public List<VideoDTO> Videos { get; set; }
        public List<ConnectioninfoDTO> ConnectionInfos { get; set; }
    }
}
using System;
using System.Text;
using System.Collections.Generic;


namespace LanwarProjector.Common.DTO
{

    public class MediaindexDTO
    {
        public Guid Id { get; set; }
        public ConnectioninfoDTO Connectioninfo { get; set; }
        public string Path { get; set; }
    }
}

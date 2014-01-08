using System;
using System.Text;
using System.Collections.Generic;


namespace LanwarProjector.Common.DTO
{

    public class VoteDTO
    {
        public Guid Id { get; set; }
        public VideoDTO Video { get; set; }
        public string IpAddress { get; set; }
        public short? VoteValue { get; set; }
        public DateTime? VoteTime { get; set; }
    }
}

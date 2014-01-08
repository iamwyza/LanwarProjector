using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanwarProjector.Common.DTO
{
    public class VideoDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public short? Rank { get; set; }
        public string Path { get; set; }
        public DateTime? WatchTime { get; set; }
        public string Url { get; set; }
        public DateTime? AddTime { get; set; }
        public string Message { get; set; }
        public Status Status { get; set; }
        public VideoType VideoType { get; set; }
        [JsonIgnore]
        public IList<VoteDTO> Votes { get; set; }
    }
}

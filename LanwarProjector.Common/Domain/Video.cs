using System;
using System.Text;
using System.Collections.Generic;


namespace LanwarProjector.Common.Domain {
    
    public class Video {
        public Video() {
			Votes = new List<Vote>();
        }
        public virtual Guid Id { get; set; }
        public virtual string Title { get; set; }
        public virtual short? Rank { get; set; }
        public virtual string Path { get; set; }
        public virtual DateTime? WatchTime { get; set; }
        public virtual string Url { get; set; }
        public virtual DateTime? AddTime { get; set; }
        public virtual string Message { get; set; }
        public virtual Status Status { get; set; }
        public virtual VideoType VideoType { get; set; }
        public virtual IList<Vote> Votes { get; set; }
    }
}

using System;
using System.Text;
using System.Collections.Generic;


namespace LanwarProjector.Common.Domain {
    
    public class Vote {
        public virtual Guid Id { get; set; }
        public virtual Video Video { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual short? VoteValue { get; set; }
        public virtual DateTime? VoteTime { get; set; }
    }
}

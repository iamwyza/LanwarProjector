using System;
using System.Text;
using System.Collections.Generic;


namespace LanwarProjector.Common.Domain {
    
    public class MediaIndex {
        public virtual Guid Id { get; set; }
        public virtual ConnectionInfo ConnectionInfo { get; set; }
        public virtual string Path { get; set; }
    }
}

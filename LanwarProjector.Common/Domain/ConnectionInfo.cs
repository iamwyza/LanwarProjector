using System;
using System.Text;
using System.Collections.Generic;


namespace LanwarProjector.Common.Domain {
    
    public class ConnectionInfo {
        public ConnectionInfo() {
			MediaIndex = new List<MediaIndex>();
        }
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Path { get; set; }
        public virtual string Status { get; set; }
        public virtual DateTime? LastindexTime { get; set; }
        public virtual byte[] Password { get; set; }
        public virtual string Username { get; set; }
        public virtual DateTime? Statustime { get; set; }
        public virtual IList<MediaIndex> MediaIndex { get; set; }
        public virtual bool Queueable { get; set; }
    }
}

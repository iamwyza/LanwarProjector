using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using LanwarProjector.Common.Domain;


namespace LanwarProjector.Common.Maps {
    
    
    public class ConnectioninfoMap : ClassMapping<ConnectionInfo> {
        
        public ConnectioninfoMap() {
			Lazy(true);
			Id(x => x.Id, map => map.Generator(Generators.Assigned));
			Property(x => x.Name);
			Property(x => x.Path);
			Property(x => x.Status);
			Property(x => x.LastindexTime);
			Property(x => x.Password);
			Property(x => x.Username);
			Property(x => x.Statustime);
			Bag(x => x.MediaIndex, colmap =>  { colmap.Key(x => x.Column("ConnectionId")); colmap.Inverse(true); }, map => { map.OneToMany(); });
        }
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using LanwarProjector.Common.Domain;


namespace LanwarProjector.Common.Maps {
    
    
    public class MediaindexMap : ClassMapping<MediaIndex> {
        
        public MediaindexMap() {
			Lazy(true);
			Id(x => x.Id, map => map.Generator(Generators.Assigned));
			Property(x => x.Path);
			ManyToOne(x => x.ConnectionInfo, map => 
			{
				map.Column("ConnectionId");
                //map.PropertyRef("Id");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

        }
    }
}

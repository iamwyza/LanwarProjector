using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using LanwarProjector.Common.Domain;


namespace LanwarProjector.Common.Maps {
    
    
    public class VotesMap : ClassMapping<Vote> {
        
        public VotesMap() {
            Table("Votes");
			Lazy(true);
			Id(x => x.Id, map => map.Generator(Generators.Assigned));
			Property(x => x.IpAddress);
			Property(x => x.VoteValue);
			Property(x => x.VoteTime);
			ManyToOne(x => x.Video, map => 
			{
				map.Column("VideoId");
                //map.PropertyRef("Id");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

        }
    }
}

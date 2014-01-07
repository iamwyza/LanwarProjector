using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using LanwarProjector.Common.Domain;


namespace LanwarProjector.Common.Maps
{


    public class VideosMap : ClassMapping<Video>
    {

        public VideosMap()
        {
            Table("Videos");
            Lazy(true);
            Id(x => x.Id, map => map.Generator(Generators.Assigned));
            Property(x => x.Title);
            Property(x => x.Rank);
            Property(x => x.Path);
            Property(x => x.WatchTime);
            Property(x => x.Url);
            Property(x => x.AddTime);
            Property(x => x.Message);
            Property(x => x.Status);
            Property(x => x.VideoType);
            Bag(x => x.Votes, colmap =>
                {
                    colmap.Key(x => x.Column("VideoId"));
                    colmap.Inverse(true);
                    colmap.Cascade(Cascade.All);
                },
                map => { map.OneToMany(); });
        }
    }
}

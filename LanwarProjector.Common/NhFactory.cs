using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Cfg;
using System.Reflection;
using NHibernate.Mapping;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace LanwarProjector.Common
{
    public class NhFactory
    {
        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    var configuration = new Configuration();
                    configuration.Configure();
                    AddMappings(configuration);

                    _sessionFactory = configuration.BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }

        private static void AddMappings(Configuration cfg)
        {
            cfg.SetProperty("hbm2ddl.keywords", "auto-quote");
            ModelMapper mapper = new ModelMapper();
            mapper.AddMapping<Maps.VideosMap>();
            mapper.AddMapping<Maps.VotesMap>();
            mapper.AddMapping<Maps.ConnectioninfoMap>();
            mapper.AddMapping<Maps.MediaindexMap>();
            cfg.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
        }

        



        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
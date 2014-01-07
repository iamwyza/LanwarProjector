using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace LanwarProjector.Common
{
    public static class AutoMapperSetup
    {
        public static void Setup()
        {
            Mapper.CreateMap<Domain.Vote, DTO.VoteDTO>();
            Mapper.CreateMap<Domain.Video, DTO.VideoDTO>();
            Mapper.CreateMap<Domain.ConnectionInfo, DTO.ConnectioninfoDTO>();
            Mapper.CreateMap<Domain.MediaIndex, DTO.MediaindexDTO>();
            Mapper.AssertConfigurationIsValid();
        }
    }
}

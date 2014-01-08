using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanwarProjector.Common.DTO
{

    public class ConnectioninfoDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Status { get; set; }
        public DateTime? Lastindextime { get; set; }
        [JsonIgnore]
        public byte[] Password { get; set; }
        public string Username { get; set; }
        public DateTime? Statustime { get; set; }
        [JsonIgnore]
        public IList<MediaindexDTO> Mediaindex { get; set; }
        public bool Queable { get; set; }
    }
}

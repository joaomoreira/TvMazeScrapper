using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TvMazeScrapper.Models
{
    [Serializable()]
    public class TvShowModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CastMemberModel> Cast { get; set; }

    }
}

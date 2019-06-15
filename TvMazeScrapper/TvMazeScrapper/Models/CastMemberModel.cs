using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TvMazeScrapper.Models
{
    [Serializable()]
    public class CastMemberModel
    {
        public PersonModel Person { get; set; }
        public CharacterModel Character { get; set; }
    }

    [Serializable()]
    public class PersonModel
    {
        //public int id { get; set; }
        public string Name { get; set; }
        public DateTime? Birthday { get ; set; }
    }

    [Serializable()]
    public class CharacterModel
    {
        //public int id { get; set; }
        public string Name { get; set; }
    }
}

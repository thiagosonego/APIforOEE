using Dapper.FluentMap.Mapping;
using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OEE.Infrastructure.Dapper.Mapper
{
    public class DowntimeMap : EntityMap<DowntimeDBModel>
    {
        public DowntimeMap()
        {
            Map(x => x.justification).ToColumn("JUSTIFICATIVA");
            Map(x => x.stopTime).ToColumn("TEMPO_PARADA");
            Map(x => x.level1).ToColumn("NIVEL_1");
            Map(x => x.level2).ToColumn("NIVEL_2");
        }
    }
}

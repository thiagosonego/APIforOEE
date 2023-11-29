using Dapper.FluentMap.Mapping;
using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OEE.Infrastructure.Dapper.Mapper
{
    public class DataProcessAndTurnForLastTurnsMap : EntityMap<DateProcessAndTurnForLastTurnsModel>
    {
        public DataProcessAndTurnForLastTurnsMap()
        {
            Map(x => x.dateProcess).ToColumn("DATA_PROCESSO");
            Map(x => x.turn).ToColumn("TURNO");
            Map(x => x.dateEnd).ToColumn("DATA_FINAL");
        }
    }
}

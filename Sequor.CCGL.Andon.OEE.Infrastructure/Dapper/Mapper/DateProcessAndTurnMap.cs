using Dapper.FluentMap.Mapping;
using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OEE.Infrastructure.Mapper
{
    public class DateProcessAndTurnMap : EntityMap<DateProcessAndTurnModel>
    {
        public DateProcessAndTurnMap()
        {
            Map(x => x.dateProcess).ToColumn("DATA_PROCESSO");
            Map(x => x.turn).ToColumn("TURNO");
        }
    }
}

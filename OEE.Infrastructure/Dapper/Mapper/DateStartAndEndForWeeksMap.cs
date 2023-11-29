using Dapper.FluentMap.Mapping;
using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OEE.Infrastructure.Dapper.Mapper
{
    public class DateStartAndEndForWeeksMap : EntityMap<DateStartAndEndForWeeksModel>
    {
        public DateStartAndEndForWeeksMap()
        {
            Map(x => x.dateStart).ToColumn("dateStart");
            Map(x => x.dateEnd).ToColumn("dateEnd");
            Map(x => x.weeks).ToColumn("weeks");
        }
    }
}

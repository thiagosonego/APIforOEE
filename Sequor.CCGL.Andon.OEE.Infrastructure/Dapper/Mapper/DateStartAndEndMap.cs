using Dapper.FluentMap.Mapping;
using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OEE.Infrastructure.Mapper
{
    public class DateStartAndEndMap : EntityMap<DateStartAndEndModel>
    {
        public DateStartAndEndMap()
        {
            Map(x => x.dateStart).ToColumn("dateStart");
            Map(x => x.dateEnd).ToColumn("dateEnd");
        }
    }
}

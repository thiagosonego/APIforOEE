using Dapper.FluentMap.Mapping;
using Sequor.CCGL.Andon.OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sequor.CCGL.Andon.OEE.Infrastructure.Mapper
{
    public class GetCalendarsMap : EntityMap<CalendarsModel>
    {
        public GetCalendarsMap()
        {
            Map(x => x.ESQUEMA).ToColumn("ESQUEMA");
            Map(x => x.DATA_PROCESSO).ToColumn("DATA_PROCESSO");
            Map(x => x.DATA_INICIAL).ToColumn("DATA_INICIAL");
            Map(x => x.DATA_FINAL).ToColumn("DATA_FINAL");
            Map(x => x.TURNO).ToColumn("TURNO");
            Map(x => x.SEMANA).ToColumn("SEMANA");
        }
    }
}

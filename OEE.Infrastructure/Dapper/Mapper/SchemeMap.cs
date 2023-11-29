using Dapper.FluentMap.Mapping;
using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OEE.Infrastructure.Dapper.Mapper
{
    public class SchemeMap : EntityMap<SchemeDBModel>
    {
        public SchemeMap()
        {
            Map(x => x.scheme).ToColumn("ESQUEMA_CALENDARIO");
        }
    }
}

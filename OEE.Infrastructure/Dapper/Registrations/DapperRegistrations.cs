using Dapper;
using Dapper.FluentMap;
using OEE.Infrastructure.Dapper.Mapper;

namespace OEE.Infrastructure.Dapper.Registrations
{
    public static class DapperRegistrations
    {
        public static void RegisterMapper()
        {
            FluentMapper.Initialize(mapper =>
            {
                mapper.AddMap(new DataProcessAndTurnForLastTurnsMap());
                mapper.AddMap(new DateProcessAndTurnMap());
                mapper.AddMap(new DateStartAndEndForWeeksMap());
                mapper.AddMap(new DateStartAndEndMap());
                mapper.AddMap(new DowntimeMap());
                mapper.AddMap(new OEEMap());
                mapper.AddMap(new SchemeMap());
            });
        }
    }
}

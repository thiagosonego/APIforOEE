using Dapper.FluentMap.Mapping;
using OEE.Domain.Models;

namespace OEE.Infrastructure.Dapper.Mapper
{
    public class OEEMap : EntityMap<OEEDBModel>
    {
        public OEEMap()
        {
            Map(x => x.station).ToColumn("ESTACAO");
            Map(x => x.partsOk).ToColumn("PECAS_OK");
            Map(x => x.partsRefused).ToColumn("PECAS_REFUGO");
            Map(x => x.partsReworked).ToColumn("PECAS_RETRABALHADAS");
            Map(x => x.timeAvailableTurn).ToColumn("TEMPO_DISPONIVEL_TURNO");
            Map(x => x.timeStopWithoutDescription).ToColumn("TEMPO_PARADA_SEM_APONTAMENTO");
            Map(x => x.timeStop).ToColumn("TEMPO_PARADA");
            Map(x => x.timeBroken).ToColumn("TEMPO_QUEBRADO");
            Map(x => x.goalProduction).ToColumn("META_PRODUCAO");
        }
    }
}

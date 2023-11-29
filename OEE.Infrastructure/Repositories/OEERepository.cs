using OEE.Domain.Interfaces;
using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OEE.Infrastructure.Repositories
{
    public class OEERepository : IOEERepository
    {
        private readonly IOEEQueries OEEQueries;

        public OEERepository(IOEEQueries oeeQueries)
        {
            this.OEEQueries = oeeQueries;
        }

        public async Task<List<OEEDBModel>> GetOEEHour(string station, string assemblyLine, DateTime dateHour)
        {
            return (await OEEQueries.GetActualOEEsByHour(station, assemblyLine, dateHour));
        }

        public Task<List<OEEDBModel>> GetOEETurn(string station, string assemblyLine, DateTime dateProcess, string turn)
        {
            return (OEEQueries.GetActualOEEsByTurns(station, assemblyLine, dateProcess, turn));
        }

        public Task<List<OEEDBModel>> GetOEEDay(string station, string assemblyLine, DateTime dateProcess)
        {
            return (OEEQueries.GetActualOEEsByDay(station, assemblyLine, dateProcess));
        }

        public Task<List<OEEDBModel>> GetOEEDays(string station, string assemblyLine, DateTime dateStart, DateTime dateEnd)
        {
            return (OEEQueries.GetActualOEEsByDays(station, assemblyLine, dateStart, dateEnd));
        }
    }
}

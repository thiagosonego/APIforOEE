using OEE.Domain.Interfaces;
using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OEE.Infrastructure.Repositories
{
    public class DowntimeRepository : IDowntimeRepository
    {
        private readonly IDowntimeQueries DowntimeQueries;

        public DowntimeRepository(IDowntimeQueries downtimeQueries)
        {
            this.DowntimeQueries = downtimeQueries;
        }
        public async Task<List<DowntimeDBModel>> GetTop3Downtime(string station, string assemblyLine, DateTime dateHour)
        {
            return await DowntimeQueries.GetTop3Downtime(station, assemblyLine, dateHour);
        }
    }
}

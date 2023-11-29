using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OEE.Domain.Interfaces
{
    public interface IDowntimeQueries
    {
        public Task<List<DowntimeDBModel>> GetTop3Downtime(string station, string assemblyLine, DateTime dateHour);
    }
}

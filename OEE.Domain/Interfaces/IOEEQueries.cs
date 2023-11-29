using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OEE.Domain.Interfaces
{
    public interface IOEEQueries
    {
        public Task<List<OEEDBModel>> GetActualOEEsByHour(string station, string assemblyLine, DateTime dateHour);
        public Task<List<OEEDBModel>> GetActualOEEsByTurns(string station, string assemblyLine, DateTime dateProcess, string turn);
        public Task<List<OEEDBModel>> GetActualOEEsByDay(string station, string assemblyLine, DateTime dateProcess);
        public Task<List<OEEDBModel>> GetActualOEEsByDays(string station, string assemblyLine, DateTime dateStart, DateTime dateEnd);
    }
}

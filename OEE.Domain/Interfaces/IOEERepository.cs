using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OEE.Domain.Interfaces
{
    public interface IOEERepository
    {
        Task<List<OEEDBModel>> GetOEEHour(string station, string assemblyLine, DateTime dateHour);
        Task<List<OEEDBModel>> GetOEETurn(string station, string assemblyLine, DateTime dateProcess, string turn);
        Task<List<OEEDBModel>> GetOEEDay(string station, string assemblyLine, DateTime dateProcess);
        Task<List<OEEDBModel>> GetOEEDays(string station, string assemblyLine, DateTime dateStart, DateTime dateEnd);
    }
}

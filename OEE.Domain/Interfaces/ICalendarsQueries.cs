using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OEE.Domain.Interfaces
{
    public interface ICalendarsQueries
    {
        public Task<string> GetEsquema(string station, string assemblyLine);
        public Task<DateProcessAndTurnModel> GetDateProcessAndTurn(string esquema);
        public Task<List<DateProcessAndTurnModel>> GetCalendarByWeekForActualOEE(string esquema, DateTime dateProcess);
        public Task<List<DateProcessAndTurnModel>> GetCalendarByMonthForActualOEE(string esquema, DateTime dateProcess);
        public Task<List<DateProcessAndTurnModel>> GetCalendarByTurnForLastOEE(string esquema);
        public Task<List<DateProcessAndTurnModel>> GetCalendarByDayForLastOEE(string esquema, DateTime dateProcess);
        public Task<List<DateStartAndEndModel>> GetCalendarByTurnsForDowntime(string esquema, int Quantity);
        public Task<List<DateStartAndEndModel>> GetCalendarByDayForDowntime(string esquema, DateTime dateProcess, int Quantity);
        public Task<List<DateStartAndEndForWeeksModel>> GetCalendarByWeekForLastOEEAndDowntime(string esquema, DateTime dateProcess, int Quantity = 2);
        public Task<List<DateStartAndEndModel>> GetCalendarByMonthForLastOEEAndDowntime(string esquema, DateTime dateProcess, int Quantity = 2);
    }
}
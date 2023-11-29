using OEE.Domain.Interfaces;
using OEE.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OEE.Infrastructure.Repositories
{
    public class CalendarsRepository : ICalendarsRepository
    {
        private readonly ICalendarsQueries CalendarsQueries;

        public CalendarsRepository(ICalendarsQueries calendarsQueries)
        {
            this.CalendarsQueries = calendarsQueries;
        }

        public async Task<string> GetScheme(string station, string assemblyLine)
        {
            return await CalendarsQueries.GetEsquema(station, assemblyLine);
        }

        public async Task<DateProcessAndTurnModel> GetDateProcessAndTurn(string scheme)
        {
            return await CalendarsQueries.GetDateProcessAndTurn(scheme);
        }

        public async Task<DateStartAndEndModel> GetCalendarByWeekForActualOEE(string esquema, DateTime dateProcess)
        {
            var result = await CalendarsQueries.GetCalendarByWeekForActualOEE(esquema, dateProcess);
            return SetCalendarForWeekAndMonthForActualOEE(result);
        }

        public async Task<DateStartAndEndModel> GetCalendarByMonthForActualOEE(string esquema, DateTime dateProcess)
        {
            var result = await CalendarsQueries.GetCalendarByMonthForActualOEE(esquema, dateProcess);
            return SetCalendarForWeekAndMonthForActualOEE(result);
        }

        public async Task<List<DateProcessAndTurnModel>> GetCalendarByTurnForLastOEE(string esquema)
        {
            return await CalendarsQueries.GetCalendarByTurnForLastOEE(esquema);
        }

        public async Task<List<DateProcessAndTurnModel>> GetCalendarByDayForLastOEE(string esquema, DateTime dateProcess)
        {

            return await CalendarsQueries.GetCalendarByDayForLastOEE(esquema, dateProcess);
        }

        public async Task<List<DateStartAndEndModel>> GetCalendarByTurnsForDowntime(string esquema, int Quantity)
        {
            return await CalendarsQueries.GetCalendarByTurnsForDowntime(esquema, Quantity);
        }

        public async Task<List<DateStartAndEndModel>> GetCalendarByDayForDowntime(string esquema, DateTime dateProcess, int Quantity)
        {
            return await CalendarsQueries.GetCalendarByDayForDowntime(esquema, dateProcess, Quantity);
        }

        public async Task<List<DateStartAndEndForWeeksModel>> GetCalendarByWeekForLastOEEAndDowntime(string esquema, DateTime dateProcess, int Quantity = 2)
        {
            return await CalendarsQueries.GetCalendarByWeekForLastOEEAndDowntime(esquema, dateProcess, Quantity);
        }

        public async Task<List<DateStartAndEndModel>> GetCalendarByMonthForLastOEEAndDowntime(string esquema, DateTime dateProcess, int Quantity = 2)
        {
            return await CalendarsQueries.GetCalendarByMonthForLastOEEAndDowntime(esquema, dateProcess, Quantity);
        }

        public DateStartAndEndModel SetCalendarForWeekAndMonthForActualOEE(List<DateProcessAndTurnModel> result)
        {
            DateStartAndEndModel calendarsEntity = new DateStartAndEndModel();
            calendarsEntity.dateStart = result.Min(x => x.dateProcess);
            calendarsEntity.dateEnd = result.Max(x => x.dateProcess);

            return calendarsEntity;
        }
    }
}

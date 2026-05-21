using Quartz;
using Schedule.Services;

namespace Schedule.Jobs
{
    public class SyncHolidaysJob : IJob
    {
        private readonly HolidayService _holidayService;

        public SyncHolidaysJob(HolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            int year = DateTime.Now.Year;

            // 1. Busca os nacionais na BrasilAPI
            await _holidayService.SyncNationalHolidaysAsync(year);

            // 2. Pega os regionais (IsRecurring) do ano anterior e joga para o ano atual
            await _holidayService.ReplicateRecurringHolidaysAsync(year);
        }
    }
}
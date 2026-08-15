using LibraryManagementSystem.Application.DTOs.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Application.IServices.IStatistics
{
    public interface IStatisticsService
    {
        Task<StatisticsDto> GetDashboardStatsAsync();
    }
}

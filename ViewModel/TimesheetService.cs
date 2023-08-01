using Microsoft.EntityFrameworkCore;
using Timesheet_Project.Models;

namespace Timesheet_Project.ViewModel
{
    public class TimesheetService
    {

        private readonly TimesheetDBContext context = new TimesheetDBContext();

        //IQueryable<Timesheet> GetTimesheets();
        //Task<List<Timesheet>> GetTimesheets(int emp_number);
        //Task<Timesheet> GetTimesheet(int id);
        //Task<Timesheet> GetEmployeeLastTimesheet(int emp_number);
        //Task<Timesheet> GetEmployeeTimesheetByDates(DateTime start_date, DateTime end_date, int emp_number);
        //Task<int> AddTimeSheet(Timesheet timesheet);
        //Task<List<TimesheetProjectActivityLnk>> GetTimesheetActivities();
        //void AddTimesheetItem(List<TimesheetItem> timesheetItem);
        //void AddTimesheetLog(TimesheetActionLog actionLog);
        //IQueryable<TimesheetItem> GetTimesheetItems(int id);
        //void UpdateTimesheet(Timesheet timesheet);
        //IQueryable<Timesheet> GetMonthlyAudit(int year, int month);
        //IQueryable<TimesheetItem> GetTimesheetItems();

        private readonly TimesheetDBContext _context = new TimesheetDBContext();

        public async Task<List<Timesheet>> GetTimesheets(int emp_number)
        {
            return await _context.Timesheets.Where(m => m.EmpId == 1).ToListAsync();
        }
        public async Task<int> AddTimeSheet(Timesheet timesheet)
        {
            _context.Timesheets.Add(timesheet);
            return await _context.SaveChangesAsync();
     }
//>>>>>>> Stashed changes
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Timesheet_Project.Models;
using Timesheet_Project.ViewModel;
using Timesheet_Project.Utils;

namespace Timesheet_Project.Controllers
{
    public class TimesheetsController : Controller
    {
        private readonly TimesheetDBContext _context;
        //private readonly ITimesheetService _timesheetService;
        public TimesheetsController(TimesheetDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var timesheetDBContext = _context.Timesheets.Include(t => t.Emp);
            return View(await timesheetDBContext.ToListAsync());
        }

        //myTimeSheet
        public async Task<IActionResult> Create(int? timesheet_id)
        //public async Task<IActionResult> Create(int? timesheet_id)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var emp = _context.Employees.FirstOrDefault(m => m.Id == 4);

            if (emp != null)
            {
                if (timesheet_id.HasValue && timesheet_id < 1)
                {
                    ViewBag.Message = TempData["Message"];
                    return View(null);
                }
                else
                {
                    //ViewBag.emp_number = emp.EmpNumber;
                    var timesheet = _context.Timesheets.Include(m => m.TimesheetItems).OrderByDescending(m => m.TimesheetId).FirstOrDefault(m => m.EmpId == emp.Id);
                    var timesheetModel = new TimesheetModel
                    {
                        Timesheet = timesheet
                    };
                    if (timesheet == null)
                    {
                        timesheetModel.month = DateTime.Now.Month;
                        timesheetModel.year = DateTime.Now.Year;
                    }
                    else
                    {
                        timesheetModel.month = timesheet.StartDate.Month;
                        timesheetModel.year = timesheet.StartDate.Year;
                    }
                    ViewBag.Message = TempData["Message"];
                    return View(timesheetModel);
                }
            }
            return NotFound();
        }

        [HttpPost] //HttpPost for create
        //[PermissionFilter(permission = "timesheet.mytimesheet")]
        public async Task<ActionResult> Create(IFormCollection form_collection)
        {
            var year = Convert.ToInt32(form_collection["year"]);
            var month = Convert.ToInt32(form_collection["month"]);
            var start_date = new DateTime(year, month, 1);
            var end_date = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var timesheet = await _context.Timesheets.Include(m => m.TimesheetItems)
            .OrderByDescending(m => m.TimesheetId)
             .FirstOrDefaultAsync(m => m.EmpId == 1 && m.StartDate == start_date && m.EndDate == end_date);

            //var emp = _hrmUtilService.GetEmployeeByUserId(userId);

            //var timesheet = await _timesheetService.GetEmployeeTimesheetByDates(start_date, end_date, emp.EmpNumber);
            var timesheetModel = new TimesheetModel
            {
                Timesheet = timesheet
            };
            if (timesheet == null)
            {
                timesheetModel.month = month;
                timesheetModel.year = year;
            }
            else
            {
                timesheetModel.month = timesheet.StartDate.Month;
                timesheetModel.year = timesheet.StartDate.Year;
            }
            return View(timesheetModel);
        }


        // GET: Timesheets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Timesheets == null)
            {
                return NotFound();
            }

            var timesheet = await _context.Timesheets
                .Include(t => t.Emp)
                .FirstOrDefaultAsync(m => m.TimesheetId == id);
            if (timesheet == null)
            {
                return NotFound();
            }

            return View(timesheet);
        }
  
        // GET: Previous Timesheet 
        public async Task<IActionResult> NewTimeSheet(int year, int month)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var emp = _hrmUtilService.GetEmployeeByUserId(userId);
            var emp = _context.Employees.FirstOrDefault(m => m.Id == 1);

            //check for existing timesheets
            var start_date = new DateTime(year, month, 1);
            var end_date = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            var timesheet = await _context.Timesheets.Include(m => m.TimesheetItems)
            .OrderByDescending(m => m.TimesheetId)
            .FirstOrDefaultAsync(m => m.EmpId == emp.Id && m.StartDate == start_date && m.EndDate == end_date);

            if (timesheet != null)
            {
                return RedirectToAction("Edit", new { id = timesheet.TimesheetId });
            }

            timesheet = new Timesheet
            {
                StartDate = start_date,
                EndDate = end_date,
                EmpId = emp.Id,
                State = "NOTSUBMITTED",
                CreatedAt = DateTime.Now
            };
            _context.Timesheets.Add(timesheet);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = timesheet.TimesheetId });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            // if its empty return not found 
            if (id == null)
            {
                return NotFound();
            }

            var timesheet = await _context.Timesheets
                .Include(m => m.TimesheetItems).ThenInclude(m => m.Project)
                .FirstOrDefaultAsync(m => m.TimesheetId == id);
            if (timesheet == null)
            {
                return NotFound();
            }

            var view_model = new TimesheetModel();
            var items = new List<TimesheetItemViewModel>();
            foreach (var item in timesheet.TimesheetItems)
            {
                items.Add(new TimesheetItemViewModel(item));
            }
            view_model.year = timesheet.StartDate.Year;
            view_model.month = timesheet.EndDate.Month;
            var timeSheetDays = DateUtil.indexMonth(view_model.year, view_model.month);
            view_model.TimesheetDays = timeSheetDays;
            view_model.FirstWeek = timeSheetDays.FirstOrDefault().WeekOfMonth;
            view_model.LastWeek = timeSheetDays.LastOrDefault().WeekOfMonth;
            view_model.Timesheet = timesheet;
            view_model.TimesheetItem = items;// timesheet.TimesheetItem.ToList();
        

            var timesheet_items = timesheet.TimesheetItems;
            //ViewData["Projects"] = new SelectList(_context.Projects.Where(m => m.ProjectId != 1).ToListAsync().Result, "Id", "Name");
            //ViewData["Activities"] = new SelectList(_hrmUtilService.GetActivities().Result, "Id", "Name");
            ViewData["Projects"] = await _context.Projects.ToListAsync();
            //ViewData["Activities"] = _hrmUtilService.GetActivities().Result;
            ViewBag.timesheet_items = timesheet_items;

            ViewBag.EmpId = timesheet.EmpId;
            return View(view_model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Timesheets == null)
            {
                return NotFound();
            }

            var timesheet = await _context.Timesheets
                .Include(t => t.Emp)
                .FirstOrDefaultAsync(m => m.TimesheetId == id);
            if (timesheet == null)
            {
                return NotFound();
            }

            return View(timesheet);
        }

        // POST: Timesheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Timesheets == null)
            {
                return Problem("Entity set 'TimesheetDBContext.Timesheets'  is null.");
            }
            var timesheet = await _context.Timesheets.FindAsync(id);
            if (timesheet != null)
            {
                _context.Timesheets.Remove(timesheet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimesheetExists(int id)
        {
            return (_context.Timesheets?.Any(e => e.TimesheetId == id)).GetValueOrDefault();
        }
    }
}

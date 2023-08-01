using System;
using System.Collections.Generic;

namespace Timesheet_Project.Models
{
    public partial class Employee
    {
        public Employee()
        {
            TimesheetItems = new HashSet<TimesheetItem>();
            Timesheets = new HashSet<Timesheet>();
        }

        public int Id { get; set; }
        public string Firstname { get; set; } = null!;
        public string Department { get; set; } = null!;

        public virtual ICollection<TimesheetItem> TimesheetItems { get; set; }
        public virtual ICollection<Timesheet> Timesheets { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Timesheet_Project.Models
{
    public partial class TimesheetItem
    {
        public int TimesheetItemId { get; set; }
        public int EmpId { get; set; }
        public int TimesheetId { get; set; }
        public int ProjectId { get; set; }
        public DateTime Date { get; set; }
        public int WkDuration { get; set; }
        public string Summary { get; set; } = null!;
        public byte[]? Signature { get; set; }

        public virtual Employee Emp { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
        public virtual Timesheet Timesheet { get; set; } = null!;
    }
}

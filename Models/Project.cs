using System;
using System.Collections.Generic;

namespace Timesheet_Project.Models
{
    public partial class Project
    {
        public Project()
        {
            TimesheetItems = new HashSet<TimesheetItem>();
        }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = null!;

        public virtual ICollection<TimesheetItem> TimesheetItems { get; set; }
    }
}

using HUECL.alpha._6_0.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace HUECL.alpha._6_0.Models.Projects
{
    public class Reminder
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public string Description { get; set; } = string.Empty;
        public DateTime ReminderDate { get; set; }
        public bool Completed { get; set; } = false;

        public string AssignedUserId { get; set; } = string.Empty;
        public ApplicationUser? AssignedUser { get; set; }

    }
}

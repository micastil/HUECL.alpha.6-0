using HUECL.alpha._6_0.Models.Projects;
using HUECL.alpha._6_0.Models.ViewModels;

namespace HUECL.alpha._6_0.Models.Repositories
{
    public interface IProjectRepository
    {
        Task<DataTablesViewModel<ProjectViewModel>> GetDataTablesProject(
            string? draw,
            int skip,
            int pageSize,
            string? searchValue,
            int sortColumnIndex,
            string? sortColumnName,
            string? sortDirection,
            int selectedYear);

        Task<int> AddProject(Project project);

        Task<Project?> GetProjectById(int id);

        Task<bool> UpdateStatus(int status, int projectId);

        Task<int> AddProjectInteraction(Interaction interaction);
    }
}

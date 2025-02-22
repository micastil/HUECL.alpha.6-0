using HUECL.alpha._6_0.Interfaces;
using HUECL.alpha._6_0.Models.CustomExceptions;
using HUECL.alpha._6_0.Models.Projects;
using HUECL.alpha._6_0.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace HUECL.alpha._6_0.Models.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<SaleRepository> _logger;
        private readonly ICustomDataProtector _customDataProtector;

        public ProjectRepository(AppDbContext appDbContext, ILogger<SaleRepository> logger, ICustomDataProtector customDataProtector)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _customDataProtector = customDataProtector;
        }

        public async Task<int> AddProject(Project project)
        {
            try 
            {
                project.Active = true;
                project.CreationDate = DateTime.Now;
                project.LastUpdate = DateTime.Now;
                // TODO 2025.01.30: Estos campos se deben ingresar en formulario
                project.Priority = 1;
                project.Probaility = 1;

                _appDbContext.Projects.Add(project);
                return await _appDbContext.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "AddProject Db Exception: {mensaje}", ex.Message);
                throw new ProjectRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "AddProject Exception: {mensaje}", ex.Message);
                throw new ProjectRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }
        }

        public async Task<int> AddProjectInteraction(Interaction interaction)
        {
            try 
            {
                _appDbContext.Interactions.Add(interaction);
                return await _appDbContext.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "AddProject Db Exception: {mensaje}", ex.Message);
                throw new ProjectRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "AddProject Exception: {mensaje}", ex.Message);
                throw new ProjectRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }
        }

        public async Task<DataTablesViewModel<ProjectViewModel>> GetDataTablesProject(string? draw, int skip, int pageSize, string? searchValue, int sortColumnIndex, string? sortColumnName, string? sortDirection, int selectedYear)
        {
            try
            {
                DataTablesViewModel<ProjectViewModel> dataTablesResult = new DataTablesViewModel<ProjectViewModel>();

                var query = _appDbContext.Projects.
                    Include(p => p.Customer).
                    Include(q => q.Owner).
                    Include(r => r.ProjectStatus).
                    Include(s => s.ProjectSector).
                    AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    query = query.Where(p =>
                        (
                        p.Customer != null && p.Customer.Name.Contains(searchValue)) ||
                        p.Name.Contains(searchValue
                        )
                    );
                }

                dataTablesResult.recordsFiltered = await query.CountAsync();

                switch (sortColumnName)
                {
                    case "customer":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.Customer != null ? p.Customer.Name : "")
                            : query.OrderByDescending(p => p.Customer != null ? p.Customer.Name : "");
                        break;
                    case "name":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.Name)
                            : query.OrderByDescending(p => p.Name);
                        break;
                    case "lastupdate":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.LastUpdate)
                            : query.OrderByDescending(p => p.LastUpdate);
                        break;
                    case "total":
                        query = sortDirection == "asc"
                            ? query.OrderBy(p => p.Total)
                            : query.OrderByDescending(p => p.Total);
                        break;
                        // Add cases for other sortable columns as needed
                }

                query = query.Include(p => p.Owner);

                dataTablesResult.Data = await query.Where(p => p.Active == true)
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(p => new ProjectViewModel
                    {
                        Id = _customDataProtector.Protect(p.Id.ToString()),
                        Name = p.Name,
                        Customer = p.Customer != null ? p.Customer.Name : "No Customer",
                        Sector = p.ProjectSector != null ? p.ProjectSector.Name : "No Sector",
                        Status = p.ProjectStatus != null ? p.ProjectStatus.Name : "No Status",
                        Total = p.Total,
                        Currency = p.Currency != null ? p.Currency.Name : "No Currency",
                        LastUpdate = p.LastUpdate,
                        Owner = p.Owner.UserName != null ? p.Owner.UserName : "No Owner"
                    })
                    .ToListAsync();

                dataTablesResult.recordsTotal = await _appDbContext.Projects.Where(p => p.Active == true).CountAsync();

                return dataTablesResult;
            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "Project Db Exception: {mensaje}", ex.Message);
                throw new ProjectRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Project Exception: {mensaje}", ex.Message);
                throw new ProjectRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }
        }

        public async Task<Project?> GetProjectById(int id)
        {
            try 
            {
                return await _appDbContext
                    .Projects
                    .Include(c => c.Customer)
                    .Include(s => s.ProjectSector)
                    .Include(r => r.ProjectStatus)
                    .Include(i => i.Currency)
                    .Include(j => j.Interactions).ThenInclude(t => t.InteractionType)
                    .Include(u => u.Owner)
                    .Where(i => i.Id == id && i.Active)
                    .FirstOrDefaultAsync();
            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "AddProject Db Exception: {mensaje}", ex.Message);
                throw new ProjectRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "AddProject Exception: {mensaje}", ex.Message);
                throw new ProjectRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }
        }

        public async Task<bool> UpdateStatus(int status, int projectId)
        {
            try 
            {
                Project? _project = await _appDbContext.Projects.Where(i => i.Id == projectId).FirstOrDefaultAsync();

                if (_project == null) { return false; }

                _project.ProjectStatusId = status;
                _project.LastUpdate = DateTime.Now;

                if (await _appDbContext.SaveChangesAsync() > 0) { return true; }

                return false;
            }
            catch (DbException ex)
            {
                _logger.LogInformation(ex, "Project Db Exception: {mensaje}", ex.Message);
                throw new ProjectRepositoryCustomException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Project Exception: {mensaje}", ex.Message);
                throw new ProjectRepositoryCustomException("Ha ocurrido un error en la aplicacion.", ex);
            }   
        }
    }
}

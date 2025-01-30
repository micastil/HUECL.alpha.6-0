using HUECL.alpha._6_0.Interfaces;
using HUECL.alpha._6_0.Models;
using HUECL.alpha._6_0.Models.CustomExceptions;
using HUECL.alpha._6_0.Models.Repositories;
using HUECL.alpha._6_0.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HUECL.alpha._6_0.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly AppDbContext _appDbcontext;
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<ProjectsController> _logger;
        private readonly ICustomDataProtector _customDataProtector;

        public ProjectsController(AppDbContext appDbcontext, IProjectRepository projectRepository, ILogger<ProjectsController> logger, ICustomDataProtector customDataProtector)
        {
            _appDbcontext = appDbcontext;
            _projectRepository = projectRepository;
            _logger = logger;
            _customDataProtector = customDataProtector;
        }

        [Authorize(Policy = "CanRead")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "CanRead")]
        [HttpPost]
        public async Task<IActionResult> GetProjects()
        {
            try
            {
                var _draw = Request.Form["draw"].FirstOrDefault();
                var _start = Request.Form["start"].FirstOrDefault();
                var _length = Request.Form["length"].FirstOrDefault();
                var _searchValue = Request.Form["search[value]"].FirstOrDefault();
                var _selectedYear = int.Parse(Request.Form["currentYear"].FirstOrDefault());

                if (!int.TryParse(_start, out int _skip))
                    return StatusCode(500, "Ha ocurrido un error en la aplicacion");
                if (!int.TryParse(_length, out int _pageSize))
                    return StatusCode(500, "Ha ocurrido un error en la aplicacion");

                // Sort the data based on the selected column and direction
                var _sortColumnIndex = int.Parse(Request.Form["order[0][column]"].FirstOrDefault());
                var _sortColumnName = Request.Form[$"columns[{_sortColumnIndex}][data]"].FirstOrDefault();
                var _sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                DataTablesViewModel<ProjectViewModel> result = await _projectRepository.GetDataTablesProject(
                    _draw,
                    _skip,
                    _pageSize,
                    _searchValue,
                    _sortColumnIndex,
                    _sortColumnName,
                    _sortDirection,
                    _selectedYear
                    );

                return Json(new
                {
                    draw = _draw,
                    recordsFiltered = result.recordsFiltered,
                    recordsTotal = result.recordsTotal,
                    data = result.Data
                });
            }
            catch (ProjectRepositoryCustomException ex)
            {
                _logger.LogInformation("Error on ProjectsController/Index: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error on ProjectsController/Index: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }
    }
}

﻿using DocumentFormat.OpenXml.Office2010.Excel;
using HUECL.alpha._6_0.Areas.Identity.Data;
using HUECL.alpha._6_0.Interfaces;
using HUECL.alpha._6_0.Models;
using HUECL.alpha._6_0.Models.CustomExceptions;
using HUECL.alpha._6_0.Models.Projects;
using HUECL.alpha._6_0.Models.Repositories;
using HUECL.alpha._6_0.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace HUECL.alpha._6_0.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly AppDbContext _appDbcontext;
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<ProjectsController> _logger;
        private readonly ICustomDataProtector _customDataProtector;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(AppDbContext appDbcontext, IProjectRepository projectRepository, ILogger<ProjectsController> logger, ICustomDataProtector customDataProtector, UserManager<ApplicationUser> userManager)
        {
            _appDbcontext = appDbcontext;
            _projectRepository = projectRepository;
            _logger = logger;
            _customDataProtector = customDataProtector;
            _userManager = userManager;
        }

        [HttpGet]
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


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["CustomerId"] = new SelectList(await _appDbcontext.Customers.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
            ViewData["ProjectStatusId"] = new SelectList(await _appDbcontext.ProjectStatuses.ToListAsync(), "Id", "Name");
            ViewData["ProjectSectorId"] = new SelectList(await _appDbcontext.ProjectSectors.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
            ViewData["CurrencyId"] = new SelectList(await _appDbcontext.Currencies.ToListAsync(), "Id", "Name");

            return View();
        }

        [Authorize(Policy = "CanWrite")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            try
            {
                ModelState.Remove(nameof(project.OwnerId));

                if (!ModelState.IsValid)
                {
                    return View(project);
                }

                string? userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError(string.Empty, "Error: Unable to identify the user. Please log in and try again.");
                    return View(project);
                }

                project.OwnerId = userId;

                var result = await _projectRepository.AddProject(project);

                if (result <= 0)
                {
                    ModelState.AddModelError(string.Empty, "Error: There was no possible to Create the New Project.");
                    return View(project);
                }

                return RedirectToAction("Index");
            }
            catch (ProjectRepositoryCustomException ex)
            {
                _logger.LogInformation("Error on ProjectsController/Create-POST: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error on ProjectsController/Create-POST: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }

        [Authorize(Policy = "CanRead")]
        public async Task<IActionResult> Details(string id)
        {
            try 
            {
                if (id.IsNullOrEmpty()) { return NotFound(); }

                var unprotected = _customDataProtector.Unprotect(id);
                int Id = int.Parse(unprotected);

                Project? _project = await _projectRepository.GetProjectById(Id);

                if (_project == null) { return NotFound(); }

                ViewData["ProjectStatusId"] = new SelectList(await _appDbcontext.ProjectStatuses.ToListAsync(), "Id", "Name");
                return View(_project);
            }
            catch (ProjectRepositoryCustomException ex)
            {
                _logger.LogInformation("Error on ProjectsController/Create-POST: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error on ProjectsController/Create-POST: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(string id, int status)
        {
            try 
            {
                if(id.IsNullOrEmpty()) { return NotFound(); }

                int Id = int.Parse(_customDataProtector.Unprotect(id));

                if (await _projectRepository.UpdateStatus(status, Id)) 
                {
                    return Json(new { success = true, message = "Project status updated successfully!", update = DateTime.Now.ToString("dd-MM-yyyy | HH:mm") });
                }

                return Json(new { success = false, message = "Error updating project status"});
            }
            catch (ProjectRepositoryCustomException ex)
            {
                _logger.LogInformation("Error on ProjectsController/Create-POST: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error on ProjectsController/Create-POST: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddInteraction(string Id)
        {
            try 
            {
                int project_Id = int.Parse(_customDataProtector.Unprotect(Id));

                Interaction _model = new Interaction 
                {
                    ProjectId = project_Id,
                    Date = DateTime.Now
                };
                ViewData["InteractionType"] = new SelectList(await _appDbcontext.InteractionTypes.Where(i => i.Active == true).ToListAsync(), "Id", "Name");

                var partialViewString = await this.RenderViewToStringAsync("_ProjectInteractionCreate", _model);
                return new JsonResult(new { status = 1, partialView = partialViewString });
            }
            catch (ProjectRepositoryCustomException ex)
            {
                _logger.LogInformation("Error on ProjectsController/Create-POST: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error on ProjectsController/Create-POST: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInteraction([FromBody] Interaction interaction, [FromQuery] string Id)
        {
            try 
            {
                if (Id.IsNullOrEmpty()) { return BadRequest(); }

                int projectId = int.Parse(_customDataProtector.Unprotect(Id));

                Project? _project = await _projectRepository.GetProjectById(projectId);

                if ( _project == null) { return NotFound(); }

                string? userId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest();
                }

                interaction.ProjectId = projectId;
                interaction.Date = DateTime.Now;
                interaction.CreatedByUserId = userId;


                if (await _projectRepository.AddProjectInteraction(interaction) > 0) 
                {
                    _project = await _projectRepository.GetProjectById(projectId);

                    if (_project == null) { return NotFound(); }

                    var partialViewString = await this.RenderViewToStringAsync("/Views/Projects/_ProjectInteractions.cshtml", _project);

                    return new JsonResult(new { partialView = partialViewString });
                }

                return BadRequest();
            }
            catch (ProjectRepositoryCustomException ex)
            {
                _logger.LogInformation("Error on ProjectsController/Create-POST: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error on ProjectsController/Create-POST: {mensaje}", ex.Message);
                return StatusCode(500, "Ha ocurrido un error en la aplicacion");
            }
        }
    }
}

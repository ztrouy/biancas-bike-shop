using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BiancasBikes.Data;
using BiancasBikes.Models;

namespace BiancasBikes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkOrderController : ControllerBase
{
    private BiancasBikesDbContext _dbContext;

    public WorkOrderController(BiancasBikesDbContext context)
    {
        _dbContext = context;
    }

    [HttpGet("incomplete")]
    [Authorize]
    public IActionResult GetIncompleteWorkOrders()
    {
        List<WorkOrder> workOrders = _dbContext.WorkOrders
            .Include(wo => wo.Bike)
            .ThenInclude(b => b.Owner)
            .Include(wo => wo.Bike)
            .ThenInclude(b => b.BikeType)
            .Include(wo => wo.UserProfile)
            .Where(wo => wo.DateCompleted == null)
            .OrderBy(wo => wo.DateInitiated)
            .ThenByDescending(wo => wo.UserProfileId == null)
            .ToList();
        
        return Ok(workOrders);
    }
}
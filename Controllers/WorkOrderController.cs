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

    [HttpPost]
    [Authorize]
    public IActionResult CreateWorkOrder(WorkOrder workOrder)
    {
        workOrder.DateInitiated = DateTime.Now;

        _dbContext.WorkOrders.Add(workOrder);
        _dbContext.SaveChanges();

        return Created($"/api/workorder/{workOrder.Id}", workOrder);
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult UpdateWorkOrder(WorkOrder workOrder, int id)
    {
        WorkOrder workOrderToUpdate = _dbContext.WorkOrders.SingleOrDefault(wo => wo.Id == id);
        if (workOrderToUpdate == null)
        {
            return NotFound();
        }
        else if (id != workOrder.Id)
        {
            return BadRequest();
        }

        // These are the only properties that we want to make editable
        workOrderToUpdate.Description = workOrder.Description;
        workOrderToUpdate.UserProfileId = workOrder.UserProfileId;
        workOrderToUpdate.BikeId = workOrder.BikeId;

        _dbContext.SaveChanges();

        return NoContent();
    }

    [HttpPut("{id}/complete")]
    [Authorize]
    public IActionResult CompleteWorkOrder(int id)
    {
        WorkOrder workOrderToComplete = _dbContext.WorkOrders.SingleOrDefault(wo => wo.Id == id);
        if (workOrderToComplete == null)
        {
            return NotFound();
        }
        if (workOrderToComplete.DateCompleted != null)
        {
            return BadRequest();
        }

        workOrderToComplete.DateCompleted = DateTime.Now;
        _dbContext.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult DeleteIncompleteWorkOrder(int id)
    {
        WorkOrder workOrderToDelete = _dbContext.WorkOrders.SingleOrDefault(wo => wo.Id == id);
        if (workOrderToDelete == null)
        {
            return NotFound();
        }
        if (workOrderToDelete.DateCompleted != null)
        {
            return BadRequest();
        }

        _dbContext.Remove(workOrderToDelete);
        _dbContext.SaveChanges();

        return NoContent();
    }
}
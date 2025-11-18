using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assignment3_EquipmentRental_API_Group12.Models;
using Assignment3_EquipmentRental_API_Group12.UnitOfWork;

namespace Assignment3_EquipmentRental_API_Group12.Controllers
{
    /// <summary>
    /// Equipment Management Controller
    /// Handles all equipment CRUD operations with role-based authorization
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
    }
}

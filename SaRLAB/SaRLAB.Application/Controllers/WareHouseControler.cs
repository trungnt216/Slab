using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.SubjectDto;
using SaRLAB.DataAccess.Service.WareHouseService;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class WareHouseControler : Controller
    {
        private readonly IWareHouseService _wareHouseService;

        public WareHouseControler(IWareHouseService wareHouseService)
        {
            _wareHouseService = wareHouseService;
        }
        [HttpPost]
        [Route("Insert")]
        // [Authorize(Roles = "Admin,Owner,Technical")]
        public IActionResult Insert(WareHouse wareHouse)
        {
            if (wareHouse == null)
            {
                return BadRequest("not have wareHouse");
            }
            else
            {
                return Ok(_wareHouseService.InsertWareHouse(wareHouse));
            }
        }

        [HttpPost]
        [Route("Update/{id}")]
        // [Authorize(Roles = "Admin,Owner,Technical")]
        public IActionResult Update(int id, WareHouse wareHouse)
        {
            if (id == 0)
            {
                return BadRequest("not have id");
            }
            else
            {
                return Ok(_wareHouseService.UpdateWareHouseById(id, wareHouse));
            }
        }

        [HttpPost]
        [Route("Delete/{id}")]
        // [Authorize(Roles = "Admin,Owner,Technical")]
        public IActionResult DeleteById(int id)
        {
            return Ok(_wareHouseService.DeleteWareHouseById(id));
        }

        [HttpGet]
        [Route("GetWareHouseByEquipmentId/{equipmentId}")]
        // [Authorize(Roles = "Admin,Owner,Technical")]
        public IActionResult GetAllEquipmentByType(int equipmentId)
        {
            return Ok(_wareHouseService.GetWareHousesByEquipmentId(equipmentId));
        }
    }
}

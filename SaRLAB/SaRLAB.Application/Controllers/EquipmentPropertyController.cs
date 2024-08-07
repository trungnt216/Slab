using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.EquipmentPropertyService;
using SaRLAB.DataAccess.Service.ManageTitleService;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentPropertyController : Controller
    {
        private readonly IEquipmentPropertyService _equipmentPropertyService;

        public EquipmentPropertyController(IEquipmentPropertyService equipmentPropertyService)
        {
            _equipmentPropertyService = equipmentPropertyService;
        }

        [HttpPost]
        [Route("Insert")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult Insert(EquipmentProperty equipmentProperty)
        {
            if (equipmentProperty == null)
            {
                return BadRequest("not have object");
            }
            else
            {
                return Ok(_equipmentPropertyService.InsertEquipmentProperty(equipmentProperty));
            }
        }

        [HttpPost]
        [Route("Update/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult Update(int id, EquipmentProperty equipmentProperty)
        {
            if (id == 0)
            {
                return BadRequest("not have id");
            }
            else
            {
                return Ok(_equipmentPropertyService.UpdateEquipmentPropertyById(id, equipmentProperty));
            }
        }


        [HttpPost]
        [Route("Delete/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult DeletelById(int id)
        {
            return Ok(_equipmentPropertyService.DeleteEquipmentPropertyById(id));
        }

        [HttpGet]
        [Route("Get/{equipmentId}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetEquipmentPropertiesAccordingEquipment(int equipmentId)
        {
            return Ok(_equipmentPropertyService.GetEquipmentPropertiesByEquipmentId(equipmentId));
        }

    }
}

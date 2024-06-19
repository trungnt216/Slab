using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.EquipmentService;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : Controller
    {
        private readonly IEquipmentService _equipmentService;

        public EquipmentController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public ActionResult Delete(int id) 
        {
            if(id == 0)
            {
                return BadRequest("not have id");
            }
            else
            {
                return Ok(_equipmentService.DeleteEquipmentById(id));
            }
        }

        [HttpGet]
        [Route("GetBySubject/{id}")]
        public IActionResult GetAllBySubject(int id) 
        {
            return Ok(_equipmentService.GetEquipmentsBySubjectId(id));
        }

        [HttpGet]
        [Route("GetByType/{subjectId}/{type}")]
        public IActionResult GetEquipmentsByType(int subjectId, string type)
        {
            return Ok(_equipmentService.GetEquipmentsByType(subjectId, type));
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_equipmentService.GetEquipmentById(id));
        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult Insert(Equipment equipment)
        {
            if (equipment == null)
            {
                return BadRequest("not have equipment");
            }
            else
            {
                return Ok(_equipmentService.InsertEquipment(equipment));
            }
        }

        [HttpPost]
        [Route("Update/{id}")]
        public IActionResult Update(int id, Equipment equipment)
        {
            if(id == 0)
            {
                return BadRequest("not have id");
            }
            else
            {
                return Ok(_equipmentService.UpdateEquipmentById(id,equipment));
            }
        }

        [HttpGet]
        [Route("GetAll/{schoolId}/{subjectId}/{type}")]
        public IActionResult GetAllEquipmentByType(int schoolId, int subjectId, string type)
        {
            return Ok(_equipmentService.GetEquipmentsByType(schoolId, subjectId, type));
        }
    }
}

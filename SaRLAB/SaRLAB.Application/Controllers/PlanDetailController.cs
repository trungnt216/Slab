﻿using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.EquipmentService;
using SaRLAB.DataAccess.Service.PlanDetailService;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanDetailController : Controller
    {
        private readonly IPlanDetailService _planDetailService;
        private readonly IEquipmentService _equipmentService;

        public PlanDetailController(IPlanDetailService planDetailService)
        {
            _planDetailService = planDetailService;
        }

        [HttpGet]
        [Route("GetByPracticePlanId")]
        public IActionResult GetPlanDetailListByPracticePlanId([FromBody] PlanDetail planDetail)
        {
            return Ok(_planDetailService.GetPlanDetailListByPracticePlanId(planDetail));
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetPlanDetailById(int id)
        {
            return Ok(_planDetailService.GetPlanDetailById(id));
        }

        [HttpPost]
        [Route("Update/{id}")]
        public IActionResult UpdatePlanDetailById(int id, [FromBody] PlanDetail planDetail)
        {
            return Ok(_planDetailService.UpdatePlanDetailById(id, planDetail));
        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult InsertPlanDetailById([FromBody] PlanDetail planDetail)
        {
  
            Equipment equipment = _equipmentService.GetEquipmentById(planDetail.EquipmentId.Value);
            Equipment equipmentUpdate = new Equipment
            {
                ID = planDetail.EquipmentId,
                EquipmentQuantity = equipment.EquipmentQuantity - planDetail.EquipmentQuantity,
                UpdateBy = planDetail.CreateBy,
                UpdateTime = planDetail.CreateTime
            };
            _equipmentService.UpdateEquipmentById(planDetail.EquipmentId.Value, equipmentUpdate);
            return Ok(_planDetailService.InsertlanDetail(planDetail));
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult DeletePlanDetailById(int id)
        {
            return Ok(_planDetailService.DeletePlanDetailById(id));
        }
    }
}
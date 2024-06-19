using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Dto.LoginService;
using SaRLAB.DataAccess.Service.BannerService;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : Controller
    {
        private readonly IBannerService bannerService;

        public BannerController(IBannerService _bannerService)
        {
            bannerService = _bannerService;
        }

        [HttpGet]
        [Route("GetALL")]
        public IActionResult GetAll()
        {
            return Ok(bannerService.GetAll());
        }

        [HttpGet]
        [Route("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var banner = bannerService.GetById(id);
            if (banner == null)
            {
                return BadRequest("cannot find the banner");
            }
            else
            {
                return Ok(banner);
            }
        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult Insert(Banner banner)
        {
            var _banner= bannerService.Insert(banner);
            if (_banner == null)
            {
                return BadRequest("cannot find the banner");
            }
            else
            {
                return Ok(_banner);
            }
        }

        [HttpPost]
        [Route("Update")]
        public IActionResult Update(Banner banner)
        {
            var _banner = bannerService.Update(banner);
            if (_banner == null)
            {
                return BadRequest("cannot find the banner");
            }
            else
            {
                return Ok(_banner);
            }
        }

        [HttpPost]
        [Route("DeleteById/{id}")]
        public IActionResult DeleteById(int id)
        {
            bannerService.DeleteById(id);
            return Ok("Banner deleted successfully.");
        }

        [HttpPost]
        [Route("DeleteByIds")]
        public IActionResult DeleteByIds([FromBody] string bannerIds)
        {
            if (string.IsNullOrEmpty(bannerIds))
            {
                return BadRequest("Banner IDs are required.");
            }

            bannerService.DeleteByIds(bannerIds);
            return Ok("Banners deleted successfully.");
        }

    }
}

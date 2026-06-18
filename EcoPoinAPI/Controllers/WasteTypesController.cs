using EcoPoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcoPoinAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WasteTypesController : ExtBaseController
    {
        private readonly EcoPoinContext dbc;
        public WasteTypesController(EcoPoinContext dbc) { this.dbc = dbc; }

        [HttpGet]
        [Authorize]
        public ActionResult GetAll()
        {
            var recs = dbc.WasteTypes.Where(w => w.IsActive).ToList().Select(w => new
            {
                id = w.Id,
                name = w.Name,
                pointTariff = w.PointTariff,
                co2Factor = w.Co2factor,
            });
            return json(recs, "Waste Types fetched successfully");
        }
    }
}

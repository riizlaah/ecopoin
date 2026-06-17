using EcopoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcopoinAPI.Controllers
{
    [Route("ecopoin-v1/[controller]")]
    [ApiController]
    public class WasteTypesController : ControllerBase
    {
        private readonly EcoPoinContext dbc;

        public WasteTypesController(EcoPoinContext dbc)
        {
            this.dbc = dbc;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAll(int page = 1, int size = 20)
        {
            var query = dbc.WasteTypes.Where(w => w.IsActive).AsQueryable();
            var (err, totalPage, items, data) = Helper.PaginateData(query, page, size, v => new
            {
                id = v.Id,
                name = v.Name,
                pointTariff = v.PointTariff,
                co2Factor = v.Co2factor,
                isActive = v.IsActive
            });
            if (err != "") return Helper.err(err);
            return Helper.PaginateRes(data, page, totalPage, items, "WasteTypes fetched successfully");
        }
    }
}

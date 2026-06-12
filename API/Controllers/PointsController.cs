using EcoPoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcoPoinAPI.Controllers
{
    [Route("ecopoin-api-v1/[controller]")]
    [ApiController]
    public class PointsController : ControllerBase
    {
        private readonly EcoPoinContext dbc;

        public PointsController(EcoPoinContext dbc)
        {
            this.dbc = dbc;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Get()
        {
            var totalPoints = dbc.DepositPoints.Sum(dp => dp.Amount);
            var redeemedPoints = dbc.RedemptionPoints.Sum(rp => rp.Amount);
            var pointOutstanding = totalPoints - redeemedPoints;
            return Helper.json(new
            {
                totalPoints,
                redeemedPoints,
                pointOutstanding,
            }, "Points report fetched successfully");
        }
    }
}

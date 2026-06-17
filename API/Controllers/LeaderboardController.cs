using EcopoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcopoinAPI.Controllers
{
    [Route("ecopoin-v1/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly EcoPoinContext dbc;

        public LeaderboardController(EcoPoinContext dbc)
        {
            this.dbc = dbc;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAll(int page = 1, int size = 20, string range = "alltime")
        {
            range = range.ToLower().Replace(" ", "");
            var query = dbc.Users.Where(u => u.Role == "resident").Include(u => u.DepositPoints).Include(u => u.DepositResidents).Include(u => u.RedemptionPoints).AsQueryable();
            if (range == "thismonth") query = query.OrderByDescending(u => u.ThisMonthPoints);
            else query = query.OrderByDescending(u => u.TotalPoints);
            var (err, totalPage, items, data) = Helper.PaginateData(query, page, size, (rec, idx) => new
            {
                rank = idx + 1,
                fullName = rec.FullName,
                points = range == "alltime" ? rec.TotalPoints : rec.ThisMonthPoints,
                balance = range == "alltime" ? rec.TotalBalance : rec.ThisMonthBalance,
                envImpact = range == "alltime" ? rec.TotalEnvImpact : rec.ThisMonthEnvImpact,
                submittedWeight = range == "alltime" ? rec.TotalSubmittedWeight : rec.ThisMonthSubmittedWeight
            });
            if (err != "") return Helper.err(err);
            return Helper.PaginateRes(data, page, totalPage, items, "Leaderboard fetched successfully");
        }

        [HttpGet("my-rank")]
        [Authorize]
        public ActionResult MyRank(int page = 1, int size = 20, string range = "alltime")
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            range = range.ToLower().Replace(" ", "");
            var query = dbc.Users.Where(u => u.Role == "resident").Include(u => u.DepositPoints).Include(u => u.DepositResidents).Include(u => u.RedemptionPoints).AsQueryable();
            if (range == "thismonth") query = query.OrderByDescending(u => u.ThisMonthPoints);
            else query = query.OrderByDescending(u => u.TotalPoints);
            var rec = query.Select((rec, idx) => new
            {
                rank = idx + 1,
                id = rec.Id,
                fullName = rec.FullName,
                points = range == "alltime" ? rec.TotalPoints : rec.ThisMonthPoints,
                balance = range == "alltime" ? rec.TotalBalance : rec.ThisMonthBalance,
                envImpact = range == "alltime" ? rec.TotalEnvImpact : rec.ThisMonthEnvImpact,
                submittedWeight = range == "alltime" ? rec.TotalSubmittedWeight : rec.ThisMonthSubmittedWeight
            }).FirstOrDefault(u => u.id == userId);
            if (rec == null) return Helper.err("User not found");
            return Helper.json(rec, "User rank fetched successfully");
        }
    }
}

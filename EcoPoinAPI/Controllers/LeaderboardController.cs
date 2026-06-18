using EcoPoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoPoinAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ExtBaseController
    {
        private readonly EcoPoinContext dbc;

        public LeaderboardController(EcoPoinContext dbc)
        {
            this.dbc = dbc;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAll(int page = 1, int size = 10, string range = "alltime")
        {
            range = range.ToLower().Replace(" ", "");
            var query = dbc.Users.Include(u => u.DepositResidents).Include(u => u.RedemptionPoints).Include(u => u.DepositResidents).ThenInclude(d => d.WasteType).AsQueryable();
            if (range == "thismonth") query = query.OrderByDescending(u => u.ThisMonthPoints);
            else query = query.OrderByDescending(u => u.TotalPoints);
            return PaginateQuery(query, page, size, (u, i) => new
            {
                rank = i + 1,
                fullName = u.FullName,
                balance = range == "alltime" ? u.TotalBalance : u.ThisMonthBalance,
                points = range == "alltime" ? u.TotalPoints : u.ThisMonthPoints,
                submittedWeight = range == "alltime" ? u.TotalSubmittedWeight : u.ThisMonthSubmittedWeight,
                envImpact = range == "alltime" ? u.TotalEnvImpact : u.ThisMonthEnvImpact,
            }, "Leaderboard fetched successfully");
        }

        [HttpGet("my-rank")]
        [Authorize]
        public ActionResult MyRank(int page = 1, int size = 10, string range = "alltime")
        {
            var userId = getUserId();
            range = range.ToLower().Replace(" ", "");
            var query = dbc.Users.Include(u => u.DepositResidents).Include(u => u.RedemptionPoints).Include(u => u.DepositResidents).ThenInclude(d => d.WasteType).AsQueryable();
            if (range == "thismonth") query = query.OrderByDescending(u => u.ThisMonthPoints);
            else query = query.OrderByDescending(u => u.TotalPoints);
            var rec = query.Select((u, i) => new
            {
                rank = i + 1,
                id = u.Id,
                fullName = u.FullName,
                balance = range == "alltime" ? u.TotalBalance : u.ThisMonthBalance,
                points = range == "alltime" ? u.TotalPoints : u.ThisMonthPoints,
                submittedWeight = range == "alltime" ? u.TotalSubmittedWeight : u.ThisMonthSubmittedWeight,
                envImpact = range == "alltime" ? u.TotalEnvImpact : u.ThisMonthEnvImpact,
            }).FirstOrDefault(u => u.id == userId);
            if (rec == null) return err("User not found");
            return json(rec, "Rank fetched successfully");
        }
    }
}

using EcoPoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcoPoinAPI.Controllers
{
    [Route("ecopoin-api-v1/[controller]")]
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
        public ActionResult GetAll(int page = 1, int size = 10, string range = "alltime")
        {
            range = range.Replace(" ", "").ToLower();
            var allowed = new[] { "alltime", "thismonth" };
            if (!allowed.Contains(range)) return Helper.err("Range not valid.");
            var query = dbc.Users.Where(u => u.Role == "resident").Include(d => d.RedemptionPoints).Include(d => d.DepositPoints)
                .ThenInclude(d => d.Deposit).ThenInclude(d => d.WasteType).AsQueryable();
            if (range == "alltime") query = query.OrderByDescending(u => u.DepositPoints.Sum(d => d.Amount));
            else query = query.OrderByDescending(u => u.DepositPoints.Where(dp => dp.CreatedAt.Month == DateTime.Today.Month).Sum(d => d.Amount));
            var (error, result, paging) = Helper.Paginate(query, (rec, idx) => new
            {
                rank = idx + 1,
                fullName = rec.FullName,
                totalPoints = range == "alltime" ? rec.TotalPoints : rec.ThisMonthTotalPoints,
                currentBalance = range == "alltime" ? rec.Balance : rec.ThisMonthBalance,
                environmentalImpact = range == "alltime" ? rec.EnvironmentalImpact : rec.ThisMonthEnvImpact
            }, page, size); 
            return Helper.paginate(result, page, paging?.items ?? 0, paging?.totalPage ?? 1, "Leaderboard fetched successfully");
        }

        [HttpGet("my-rank")]
        [Authorize]
        public ActionResult MyRank()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var totalUsers = dbc.Users.Where(u => u.Role == "resident").Count();
            var data = dbc.Users.Where(u => u.Role == "resident").Include(d => d.RedemptionPoints).Include(d => d.DepositPoints).ThenInclude(d => d.Deposit)
                .ThenInclude(d => d.WasteType).OrderByDescending(u => u.DepositPoints.Sum(d => d.Amount)).AsEnumerable().Select((rec, idx) => new
                {
                    rank = idx + 1,
                    id = rec.Id,
                    fullName = rec.FullName,
                    currentBalance = rec.Balance,
                    totalPoints = rec.TotalPoints,
                    environmentalImpact = rec.EnvironmentalImpact,
                    fromTotal = totalUsers
                }).FirstOrDefault(u => u.id == userId);
            return Helper.json(data, "Leaderboard fetched successfully");
        }
    }
}

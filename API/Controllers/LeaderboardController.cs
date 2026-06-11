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
        public ActionResult GetAll(int page = 1, int size = 10)
        {
            var query = dbc.Users.Where(u => u.Role == "resident").Include(d => d.DepositPoints).ThenInclude(d => d.Deposit).ThenInclude(d => d.WasteType).OrderByDescending(u => u.DepositPoints.Sum(d => d.Amount));
            var (error, result, paging) = Helper.Paginate(query, (rec, idx) => new
            {
                rank = idx + 1,
                fullName = rec.FullName,
                totalPoints = rec.TotalPoints,
                currentBalance = rec.Balance,
                environmentalImpact = rec.EnvironmentalImpact
            }, page, size);
            return Helper.paginate(result, page, paging?.items ?? 0, paging?.totalPage ?? 1, "Leaderboard fetched successfully");
        }

        [HttpGet("my-rank")]
        [Authorize]
        public ActionResult MyRank()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var data = dbc.Users.Where(u => u.Role == "resident").Include(d => d.DepositPoints).ThenInclude(d => d.Deposit)
                .ThenInclude(d => d.WasteType).OrderByDescending(u => u.DepositPoints.Sum(d => d.Amount)).AsEnumerable().Select((rec, idx) => new
                {
                    rank = idx + 1,
                    id = rec.Id,
                    fullName = rec.FullName,
                    totalPoints = rec.TotalPoints,
                    environmentalImpact = rec.EnvironmentalImpact
                }).FirstOrDefault(u => u.id == userId);
            return Helper.json(data, "Leaderboard fetched successfully");
        }
    }
}

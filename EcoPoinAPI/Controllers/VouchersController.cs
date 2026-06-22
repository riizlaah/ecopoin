using EcoPoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoPoinAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VouchersController : ExtBaseController
    {
        private readonly EcoPoinContext dbc;

        public VouchersController(EcoPoinContext dbc) { this.dbc = dbc; }

        [HttpGet]
        [Authorize]
        public ActionResult GetAll(int page = 1, int size = 10)
        {
            var query = dbc.Vouchers.Where(v => v.IsActive).AsQueryable();
            return PaginateQuery(query, page, size, v => new
            {
                id = v.Id,
                name = v.Name,
                pointCost = v.PointCost,
            }, "Vouchers fetched successfully");
        }

        [HttpGet("history")]
        [Authorize(Roles = "resident")]
        public ActionResult History(int page = 1, int size = 10, string status = "all")
        {
            var allowed = new[] { "unused", "used" };
            status = status.ToLower().Replace(" ", "");
            var userId = getUserId();
            var query = dbc.RedemptionPoints.Include(rp => rp.Voucher).AsQueryable();
            if(status != "all")
            {
                var target = status == "used";
                query = query.Where(rp => rp.IsUsed == target);
            }
            return PaginateQuery(query, page, size, v => new
            {
                id = v.Id,
                voucher = new {
                    id = v.VoucherId,
                    name = v.Voucher.Name,
                    pointCost = v.PointCost,
                    isActive = v.Voucher.IsActive
                },
                isUsed = v.IsUsed,
                code = v.Code,
                amount = v.Amount,
                createdAt = v.CreatedAt,
                updatedAt = v.UpdatedAt
            }, "Vouchers fetched successfully");
        }

        [HttpPost("{id}")]
        [Authorize]
        public ActionResult Redeem(int id)
        {
            var userId = getUserId();
            var voucher = dbc.Vouchers.Find(id);
            if (voucher == null) return err("Voucher not found", 404);
            var user = dbc.Users.Include(u => u.DepositPoints).Include(u => u.RedemptionPoints).FirstOrDefault(u => u.Id == userId);
            if (user == null) return err("User not found", 404);
            if (user.TotalBalance <= voucher.PointCost) return err($"Insufficient points. Required: {voucher.PointCost}, available: {user.TotalBalance}");
            dbc.RedemptionPoints.Add(new RedemptionPoint
            {
                ResidentId = userId,
                VoucherId = id,
                Amount = voucher.PointCost,
                PointCost = voucher.PointCost,
                Code = $"{voucher.Code}-{RandStr(10)}",
                IsUsed = false
            });
            dbc.SaveChanges();
            return msg("Voucher redeemed successfully");
        }

        [HttpGet("{code}/check")]
        [Authorize]
        public ActionResult Detail(string code)
        {
            var rp = dbc.RedemptionPoints.Include(rp => rp.Voucher).Include(rp => rp.Resident).FirstOrDefault(rp => rp.Code == code);
            if (rp == null) return err("Redeemed Voucher not found");
            return json(new
            {
                id = rp.Id,
                resident = new
                {
                    id = rp.ResidentId,
                    name = rp.Resident.FullName,
                    email = rp.Resident.Email
                },
                voucher = new
                {
                    id = rp.VoucherId,
                    name = rp.Voucher.Name,
                    pointCost = rp.Voucher.PointCost,
                    isActive = rp.Voucher.IsActive
                },
                isUsed = rp.IsUsed,
                code,
                amount = rp.Amount,
                createdAt = rp.CreatedAt,
                updatedAt = rp.UpdatedAt
            }, "Redeemed voucher detail fetched");
        }

        [HttpPatch("{id}/use")]
        [Authorize]
        public ActionResult Use(int id)
        {
            var rp = dbc.RedemptionPoints.Include(rp => rp.Voucher).Include(rp => rp.Resident).FirstOrDefault(rp => rp.Id == id);
            if (rp == null) return err("Redeemed Voucher not found", 404);
            if (rp.IsUsed) return err("Voucher has been used");
            rp.IsUsed = true;
            rp.UpdatedAt = DateTime.Now;
            dbc.SaveChanges();
            return msg("Voucher used successfully");
        }
    }
}

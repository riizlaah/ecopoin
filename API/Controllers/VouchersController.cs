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
    public class VouchersController : ControllerBase
    {
        private readonly EcoPoinContext dbc;

        public VouchersController(EcoPoinContext dbc)
        {
            this.dbc = dbc;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAll(int page = 1, int size = 20)
        {
            var query = dbc.Vouchers.Where(v => v.IsActive).AsQueryable();
            var (err, totalPage, items, data) = Helper.PaginateData(query, page, size, v => new
            {
                id = v.Id,
                name = v.Name,
                pointCost = v.PointCost,
                isActive = v.IsActive
            });
            if (err != "") return Helper.err(err);
            return Helper.PaginateRes(data, page, totalPage, items, "Vouchers fetched successfully");
        }

        [HttpPost("{id}/redeem")]
        [Authorize(Roles = "resident")]
        public ActionResult Redeem(int id)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var voucher = dbc.Vouchers.Find(id);
            if (voucher == null) return Helper.err("Voucher not found", 404);
            var user = dbc.Users.Include(u => u.DepositPoints).Include(u => u.RedemptionPoints).FirstOrDefault(u => u.Id == id);
            if (user == null) return Helper.err("User not found", 404);
            if (user.TotalBalance < voucher.PointCost) return Helper.err($"Insufficient points. Required: {voucher.PointCost}, available: {user.TotalBalance}");
            dbc.RedemptionPoints.Add(new RedemptionPoint
            {
                ResidentId = userId,
                VoucherId = voucher.Id,
                Amount = voucher.PointCost,
                Code = $"{voucher.Code}-{Helper.RandStr(10)}",
                PointCost = voucher.PointCost,
                IsUsed = false
            });
            dbc.SaveChanges();
            return Helper.msg("Voucher redeemed successfully");
        }

        [HttpGet("redeemed")]
        [Authorize(Roles = "resident")]
        public ActionResult GetAllRedeemedVoucher(int page = 1, int size = 10, string status = "all")
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = dbc.RedemptionPoints.Include(v => v.Voucher).Where(v => v.ResidentId == userId).OrderByDescending(v => v.IsUsed ? 1 : 0).ThenByDescending(v => v.UpdatedAt).AsQueryable();
            var allowed = new[] { "used", "unused" };
            if(allowed.Contains(status.ToLower()))
            {
                var target = status.ToLower() == "used";
                query = query.Where(v => v.IsUsed == target);
            }
            var (err, totalPage, items, data) = Helper.PaginateData(query, page, size, rec => new
            {
                id = rec.Id,
                voucher = new
                {
                    id = rec.VoucherId,
                    name = rec.Voucher.Name,
                    pointCost = rec.PointCost,
                    isActive = rec.Voucher.IsActive
                },
                pointCost = rec.PointCost,
                amount = rec.Amount,
                isUsed = rec.IsUsed,
                createdAt = rec.CreatedAt,
                updatedAt = rec.UpdatedAt,
            });
            if (err != "") return Helper.err(err);
            return Helper.PaginateRes(data, page, totalPage, items, "Redeemed vouchers fetched successfully");
        }

        [HttpGet("{id}/detail")]
        [Authorize]
        public ActionResult CheckRedeemedVoucher(int id)
        {
            var rec = dbc.RedemptionPoints.Include(v => v.Voucher).FirstOrDefault(v => v.Id == id);
            if (rec == null) return Helper.err("Voucher not found", 404);
            return Helper.json(new
            {
                id = rec.Id,
                voucher = new
                {
                    id = rec.VoucherId,
                    name = rec.Voucher.Name,
                    pointCost = rec.PointCost,
                    isActive = rec.Voucher.IsActive
                },
                pointCost = rec.PointCost,
                amount = rec.Amount,
                isUsed = rec.IsUsed,
                createdAt = rec.CreatedAt,
                updatedAt = rec.UpdatedAt,
            }, "Redemed Voucher fetched successfully");
        }

        [HttpPost("{id}/use")]
        [Authorize(Roles = "admin")]
        public ActionResult Use(int id)
        {
            var rec = dbc.RedemptionPoints.Find(id);
            if (rec == null) return Helper.err("Voucher not found", 404);
            rec.IsUsed = true;
            dbc.SaveChanges();
            return Helper.msg("Redeemed Voucher used");
        }
    }
}

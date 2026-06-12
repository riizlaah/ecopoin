using EcoPoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace EcoPoinAPI.Controllers
{
    [Route("ecopoin-api-v1/[controller]")]
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
        public ActionResult GetAll(int page = 1, int size = 20, string search = "")
        {
            var query = dbc.Vouchers.AsQueryable();
            if (search.Trim() != "")
            {
                query = query.Where(w => EF.Functions.Like(w.Name, $"%{search}%") || EF.Functions.Like(w.Code, $"%{search}%"));
            }
            var (error, result, paging) = Helper.Paginate(query, rec => new
            {
                id = rec.Id,
                name = rec.Name,
                code = rec.Code,
                pointCost = rec.PointCost,
                isActive = rec.IsActive
            }, page, size);
            if (error != "") return Helper.err(error);
            return Helper.paginate(result, page, paging?.items ?? 0, paging?.totalPage ?? 1, "Vouchers fetched successfully");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Create(VoucherDTO input)
        {
            if (!input.code.All(Char.IsLetterOrDigit)) return Helper.err("Voucher code can only contain letters/digits");
            if (input.pointCost <= 0m) return Helper.err("Point cost must be greater than zero");
            if (dbc.Vouchers.Any(rec => rec.Code == input.code)) return Helper.err("Code has been taken");
            if (dbc.Vouchers.Any(rec => rec.Name == input.name)) return Helper.err("Name has been taken");
            dbc.Vouchers.Add(new Voucher
            {
                Name = input.name,
                Code = input.code,
                PointCost = input.pointCost,
                IsActive = input.isActive,
            });
            dbc.SaveChanges();
            return Helper.msg("Voucher created successfully");
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult Get(int id)
        {
            var rec = dbc.Vouchers.AsNoTrackingWithIdentityResolution().FirstOrDefault(rec => rec.Id == id);
            if (rec == null) return Helper.err("Voucher not found", 404);
            return Helper.json(new
            {
                id = rec.Id,
                name = rec.Name,
                code = rec.Code,
                pointCost = rec.PointCost,
                isActive = rec.IsActive
            }, "Voucher fetched successfully");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public ActionResult Update(int id, VoucherDTO input)
        {
            if (!input.code.All(Char.IsLetterOrDigit)) return Helper.err("Voucher code can only contain letters/digits");
            if (input.pointCost <= 0m) return Helper.err("Point cost must be greater than zero");
            var rec = dbc.Vouchers.FirstOrDefault(rec => rec.Id == id);
            if (rec == null) return Helper.err("Voucher not found", 404);
            if (dbc.Vouchers.Any(rec2 => rec2.Code == input.code && rec2.Id != rec.Id)) return Helper.err("Code has been taken");
            if (dbc.Vouchers.Any(rec2 => rec2.Name == input.name && rec2.Id != rec.Id)) return Helper.err("Name has been taken");
            rec.Name = input.name;
            rec.Code = input.code;
            rec.PointCost = input.pointCost;
            rec.IsActive = input.isActive;
            dbc.SaveChanges();
            return Helper.msg("Voucher updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var rec = dbc.Vouchers.FirstOrDefault(rec => rec.Id == id);
            if (rec == null) return Helper.err("Voucher not found", 404);
            dbc.Vouchers.Remove(rec);
            dbc.SaveChanges();
            return Helper.msg("Voucher removed successfully");
        }

        [HttpGet("top")]
        [Authorize(Roles = "admin")]
        public ActionResult GetTops(int page = 1, int size = 20)
        {
            var query = dbc.Vouchers.Include(rec => rec.RedemptionPoints).OrderByDescending(rec => rec.RedemptionPoints.Sum(rp => rp.Amount)).AsQueryable();
            var (error, result, paging) = Helper.Paginate(query, rec => new
            {
                id = rec.Id,
                name = rec.Name,
                totalRedemption = rec.RedemptionPoints.Count(),
                totalPointsRedeemed = rec.RedemptionPoints.Sum(rp => rp.Amount),
                isActive = rec.IsActive
            }, page, size);
            return Helper.paginate(result, page, paging?.items ?? 0, paging?.totalPage ?? 1, "Voucher reports fetched successfully");
        }

        [HttpGet("history")]
        [Authorize(Roles = "resident")]
        public ActionResult History(int page = 1, int size = 20)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = dbc.RedemptionPoints.Include(rec => rec.Voucher).OrderByDescending(rec => rec.UpdatedAt).Where(rec => rec.ResidentId == userId).AsQueryable();
            var (error, result, paging) = Helper.Paginate(query, rec => new
            {
                id = rec.Id,
                voucher = new
                {
                    id = rec.VoucherId,
                    name = rec.Voucher.Name,
                    code = rec.Voucher.Code,
                    pointCost = rec.Voucher.PointCost,
                    isActive = rec.Voucher.IsActive
                },
                code = rec.Code,
                amount = rec.Amount,
                isUsed = rec.IsUsed,
                createdAt = rec.CreatedAt,
                updatedAt = rec.UpdatedAt
            }, page, size);
            return Helper.paginate(result, page, paging?.items ?? 0, paging?.totalPage ?? 1, "Voucher usage history fetched successfully");
        }

        [HttpGet("{code}/detail")]
        [Authorize(Roles = "admin")]
        public ActionResult Detail(string code)
        {
            var rec = dbc.RedemptionPoints.Include(rp => rp.Voucher).Include(rp => rp.Resident).FirstOrDefault(rp => rp.Code == code);
            if (rec == null) return Helper.err("Voucher not found", 404);
            return Helper.json(new
            {
                id = rec.Id,
                voucher = new
                {
                    id = rec.VoucherId,
                    name = rec.Voucher.Name,
                    code = rec.Voucher.Code,
                    pointCost = rec.Voucher.PointCost,
                    isActive = rec.Voucher.IsActive
                },
                resident = new
                {
                    id = rec.ResidentId,
                    fullName = rec.Resident.FullName,
                    email = rec.Resident.Email,
                },
                code = rec.Code,
                amount = rec.Amount,
                isUsed = rec.IsUsed,
                createdAt = rec.CreatedAt,
                updatedAt = rec.UpdatedAt
            }, "Voucher fetched successfully");
        }

        [HttpPost("{id}/redeem")]
        [Authorize(Roles = "resident")]
        public ActionResult Redeem(int id)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var voucher = dbc.Vouchers.Find(id);
            if (voucher == null) return Helper.err("Voucher not found", 404);
            var user = dbc.Users.Include(u => u.RedeemedPoints).Include(u => u.DepositPoints).FirstOrDefault(u => u.Id == userId);
            if (user == null) return Helper.err("User not found", 404);
            if (user.Balance < voucher.PointCost) return Helper.err($"Insufficient points. Required: {voucher.PointCost}, available: {user.Balance}");
            dbc.RedemptionPoints.Add(new RedemptionPoint
            {
                ResidentId = userId,
                Amount = voucher.PointCost,
                PointCost = voucher.PointCost,
                IsUsed = false,
                Code = voucher.Code + DateTime.Now.ToString("yyyyMMdd") + Helper.RandStr(),
                VoucherId = voucher.Id,
            });
            dbc.SaveChanges();
            return Helper.msg("Voucher redeemed successfully");
        }

        [HttpPost("{id}/use")]
        [Authorize(Roles = "admin")]
        public ActionResult Use(int id)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var rec = dbc.RedemptionPoints.Include(rp => rp.Voucher).FirstOrDefault(rp => rp.Id == id);
            if (rec == null) return Helper.err("Voucher not found", 404);
            if (rec.IsUsed) return Helper.err("Voucher has been used");
            rec.IsUsed = true;
            rec.UpdatedAt = DateTime.Now;
            dbc.SaveChanges();
            return Helper.msg("Voucher used");
        }



    }

    public class VoucherDTO
    {
        [Required] public string name { get; set; } = null!;
        [Required] public string code { get; set; } = null!;
        [Required] public int pointCost { get; set; }
        [Required] public bool isActive { get; set; }
    }
}

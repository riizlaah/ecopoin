using EcoPoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EcoPoinAPI.Controllers
{
    [Route("api/[controller]")]
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
            return Helper.paginate(result, page, size, paging?.total ?? 1, "WasteTypes fetched successfully");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Create(VoucherDTO input)
        {
            if (input.pointCost < 0m) return Helper.err("Point cost must be greater than zero");
            if (dbc.WasteTypes.Any(rec => rec.Code == input.code)) return Helper.err("Code has been taken");
            if (dbc.WasteTypes.Any(rec => rec.Name == input.name)) return Helper.err("Name has been taken");
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
        [Authorize(Roles = "admin")]
        public ActionResult Get(int id)
        {
            var rec = dbc.Vouchers.AsNoTrackingWithIdentityResolution().FirstOrDefault(rec => rec.Id == id);
            if (rec == null) return Helper.err("Voucher not found");
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
            if (input.pointCost < 0m) return Helper.err("Point cost must be greater than zero");
            var rec = dbc.Vouchers.AsNoTrackingWithIdentityResolution().FirstOrDefault(rec => rec.Id == id);
            if (rec == null) return Helper.err("WasteType not found");
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
            if (rec == null) return Helper.err("Voucher not found");
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
            return Helper.paginate(new
            {
                data = result
            }, page, size, paging?.total ?? 1, "Voucher reports fetched successfully");
        }

    }

    public class VoucherDTO
    {
        [Required] public string name { get; set; } = null!;
        [Required] public string code { get; set; } = null!;
        [Required] public decimal pointCost { get; set; }
        [Required] public bool isActive { get; set; }
    }
}

using EcoPoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace EcoPoinAPI.Controllers
{
    [Route("ecopoin-api-v1/[controller]")]
    [ApiController]
    public class WasteTypesController : ControllerBase
    {
        private readonly EcoPoinContext dbc;

        public WasteTypesController(EcoPoinContext dbc)
        {
            this.dbc = dbc;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult GetAll(int page = 1, int size = 20, string search = "")
        {
            var query = dbc.WasteTypes.AsQueryable();
            if (search.Trim() != "")
            {
                query = query.Where(w => EF.Functions.Like(w.Name, $"%{search}%") || EF.Functions.Like(w.Code, $"%{search}%"));
            }
            var (error, result, paging) = Helper.Paginate(query, rec => new
            {
                id = rec.Id,
                name = rec.Name,
                code = rec.Code,
                pointTariff = rec.PointTariff,
                CO2Factor = rec.Co2factor,
                isActive = rec.IsActive
            }, page, size);
            if (error != "") return Helper.err(error);
            return Helper.paginate(result, page, paging?.items ?? 0, paging?.totalPage ?? 1, "WasteTypes fetched successfully");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Create(WasteTypeDTO input)
        {
            if (input.pointTariff <= 0m) return Helper.err("Point tariff must be greater than zero");
            if (input.CO2factor <= 0m) return Helper.err("CO2 Factor must be greater than zero");
            if (dbc.WasteTypes.Any(rec => rec.Code == input.code)) return Helper.err("Code has been taken");
            if (dbc.WasteTypes.Any(rec => rec.Name == input.name)) return Helper.err("Name has been taken");
            dbc.WasteTypes.Add(new WasteType
            {
                Name = input.name,
                Code = input.code,
                PointTariff = input.pointTariff,
                Co2factor = input.CO2factor,
                IsActive = input.isActive,
            });
            dbc.SaveChanges();
            return Helper.msg("WasteType created successfully");
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public ActionResult Get(int id)
        {
            var rec = dbc.WasteTypes.AsNoTrackingWithIdentityResolution().FirstOrDefault(rec => rec.Id == id);
            if (rec == null) return Helper.err("WasteType not found", 404);
            return Helper.json(new
            {
                id = rec.Id,
                name = rec.Name,
                code = rec.Code,
                pointTariff = rec.PointTariff,
                CO2Factor = rec.Co2factor,
                isActive = rec.IsActive
            }, "WasteType fetched successfully");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public ActionResult Update(int id, WasteTypeDTO input)
        {
            if (input.pointTariff <= 0m) return Helper.err("Point tariff must be greater than zero");
            if (input.CO2factor <= 0m) return Helper.err("CO2 Factor must be greater than zero");
            var rec = dbc.WasteTypes.FirstOrDefault(rec => rec.Id == id);
            if (rec == null) return Helper.err("WasteType not found", 404);
            if (dbc.WasteTypes.Any(rec2 => rec2.Code == input.code && rec2.Id != rec.Id)) return Helper.err("Code has been taken");
            if (dbc.WasteTypes.Any(rec2 => rec2.Name == input.name && rec2.Id != rec.Id)) return Helper.err("Name has been taken");
            rec.Name = input.name;
            rec.Code = input.code;
            rec.PointTariff = input.pointTariff;
            rec.Co2factor = input.CO2factor;
            rec.IsActive = input.isActive;
            dbc.SaveChanges();
            return Helper.msg("WasteType updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var rec = dbc.WasteTypes.FirstOrDefault(rec => rec.Id == id);
            if (rec == null) return Helper.err("Voucher not found", 404);
            dbc.WasteTypes.Remove(rec);
            dbc.SaveChanges();
            return Helper.msg("WasteType removed successfully");
        }

        [HttpGet("top")]
        [Authorize(Roles = "admin")]
        public ActionResult GetTops(int page = 1, int size = 20)
        {
            var query = dbc.WasteTypes.Include(rec => rec.Deposits).OrderByDescending(rec => rec.Deposits.Where(d => d.Status == "Verified").Sum(d => d.ActualWeight ?? d.EstimatedWeight)).AsQueryable();
            var (error, result, paging) = Helper.Paginate(query, rec => new
            {
                id = rec.Id,
                name = rec.Name,
                totalWeight = rec.Deposits.Where(d => d.Status == "Verified").Sum(d => d.ActualWeight ?? d.EstimatedWeight),
                totalPoints = rec.Deposits.Where(d => d.Status == "Verified").Sum(d => d.DepositPoint?.Amount ?? 0m)
            }, page, size);
            return Helper.paginate(result, page, paging?.items ?? 0, paging?.totalPage ?? 1, "WasteType reports fetched successfully");
        }
    }

    public class WasteTypeDTO
    {
        [Required] public string name { get; set; } = null!;
        [Required] public string code { get; set; } = null!;
        [Required] public decimal pointTariff { get; set; }
        [Required] public decimal CO2factor { get; set; }
        [Required] public bool isActive { get; set; }
    }
}

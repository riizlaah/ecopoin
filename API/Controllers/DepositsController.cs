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
    public class DepositsController : ControllerBase
    {
        private readonly EcoPoinContext dbc;
        private readonly string uploadPath;

        public DepositsController(EcoPoinContext dbc, IWebHostEnvironment env)
        {
            this.dbc = dbc;
            uploadPath = Path.Combine(env.ContentRootPath, "wwwroot/uploads");
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAll(int page = 1, int size = 10, string status = "")
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "resident";
            var query = dbc.Deposits.Include(d => d.Resident).Include(d => d.WasteType).OrderByDescending(d => d.UpdatedAt).AsQueryable();
            var allowed = new[] { "Pending", "Rejected", "Verified" };
            if(allowed.Contains(status))
            {
                query = query.Where(d => d.Status == status);
            }
            var (err, totalPage, items, data) = Helper.PaginateData(query, page, size, d => new
            {
                id = d.Id,
                resident = new
                {
                    id = d.ResidentId,
                    name = d.Resident.FullName,
                    email = d.Resident.Email
                },
                wasteType = new
                {
                    id = d.WasteTypeId,
                    name = d.WasteType.Name,
                    pointTariff = d.WasteType.PointTariff,
                    co2Factor = d.WasteType.Co2factor,
                    isActive = d.WasteType.IsActive
                },
                estWeight = d.EstimatedWeight,
                estPoints = (int)Math.Round(d.EstimatedWeight * d.WasteType.PointTariff),
                actualWeight = d.ActualWeight ?? d.EstimatedWeight,
                actualPoints = (int)Math.Round((d.ActualWeight ?? 0m) * d.WasteType.PointTariff),
                status = d.Status,
                updatedAt = d.UpdatedAt
            });
            if (err != "") return Helper.err(err);
            return Helper.PaginateRes(data, page, totalPage, items, "Deposits fetched successfuly");
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult GetAll(int id)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "resident";
            var d = dbc.Deposits.Include(d => d.Resident).Include(d => d.Officer).Include(d => d.DepositPoint).Include(d => d.WasteType).OrderByDescending(d => d.UpdatedAt).Select(d => new
            {
                id = d.Id,
                resident = new
                {
                    id = d.ResidentId,
                    name = d.Resident.FullName,
                    email = d.Resident.Email
                },
                officer = d.Officer == null ? null : new
                {
                    id = d.OfficerId,
                    name = d.Officer.FullName,
                    email = d.Officer.Email
                },
                wasteType = new
                {
                    id = d.WasteTypeId,
                    name = d.WasteType.Name,
                    pointTariff = d.WasteType.PointTariff,
                    co2Factor = d.WasteType.Co2factor,
                    isActive = d.WasteType.IsActive
                },
                estWeight = d.EstimatedWeight,
                estPoints = (int)Math.Round(d.EstimatedWeight * d.WasteType.PointTariff),
                actualWeight = d.ActualWeight ?? d.EstimatedWeight,
                actualPoints = (int)Math.Round((d.ActualWeight ?? 0m) * d.WasteType.PointTariff),
                status = d.Status,
                updatedAt = d.UpdatedAt,
                createdAt = d.CreatedAt,
                notes = d.Notes,
                rejectionReason = d.RejectionReason,
                photoPath = d.PhotoPath
            }).FirstOrDefault(d => d.id == id);
            if (d == null) return Helper.err("Deposit not found", 404);
            return Helper.json(d, "Deposit fetched successfully");
        }

        [HttpPost]
        [Authorize(Roles = "resident")]
        public async Task<ActionResult> Create([FromForm] int wasteTypeId, [FromForm] decimal estWeight, IFormFile photo, [FromForm] string? notes = null)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (photo == null || photo.Length == 0) return Helper.err("Photo required");
            var allowed = new[] { "image/jpeg", "image/png" };
            if (!allowed.Contains(photo.ContentType)) return Helper.err("Only JPG/PNG");
            if (estWeight <= 0m) return Helper.err("Estimated weight must be more than zero");
            if (notes != null && notes.Trim() == "") return Helper.err("Notes can't be empty");
            if (!dbc.WasteTypes.Any(w => w.Id == wasteTypeId)) return Helper.err("WasteType not found", 404);
            await dbc.Deposits.AddAsync(new Deposit
            {
                ResidentId = userId,
                WasteTypeId = wasteTypeId,
                EstimatedWeight = estWeight,
                Notes = notes,
                PhotoPath = await Helper.UploadFile(uploadPath, photo),
                Status = "Pending"
            });
            await dbc.SaveChangesAsync();
            return Helper.msg("Deposit submitted successfully");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "resident")]
        public async Task<ActionResult> Update(int id, [FromForm] int wasteTypeId, [FromForm] decimal estWeight, IFormFile photo, [FromForm] string? notes = null)
        {
            if (photo != null && photo.Length > 0)
            {
                var allowed = new[] { "image/jpeg", "image/png" };
                if (!allowed.Contains(photo.ContentType)) return Helper.err("Only JPG/PNG");
            }
            if (estWeight <= 0m) return Helper.err("Estimated weight must be more than zero");
            if (notes != null && notes.Trim() == "") return Helper.err("Notes can't be empty");
            var rec = dbc.Deposits.FirstOrDefault(d => d.Id == id && d.Status == "Pending");
            if (rec == null) return Helper.err("Deposit not found", 404);
            if (!dbc.WasteTypes.Any(w => w.Id == wasteTypeId)) return Helper.err("WasteType not found", 404);
            rec.WasteTypeId = wasteTypeId;
            rec.EstimatedWeight = estWeight;
            rec.Notes = notes;
            rec.UpdatedAt = DateTime.Now;
            if (photo != null) rec.PhotoPath = await Helper.UploadFile(uploadPath, photo, rec.PhotoPath);
            await dbc.SaveChangesAsync();
            return Helper.msg("Deposit updated successfully");
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "resident")]
        public async Task<ActionResult> Verify(int id, [FromForm] int wasteTypeId, [FromForm] decimal actWeight, [FromForm] bool verify, IFormFile photo, [FromForm] string? rejectionReason = null)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (photo != null && photo.Length > 0)
            {
                var allowed = new[] { "image/jpeg", "image/png" };
                if (!allowed.Contains(photo.ContentType)) return Helper.err("Only JPG/PNG");
            }
            if (actWeight <= 0m) return Helper.err("Actual weight must be more than zero");
            if(!verify)
            {
                if (rejectionReason != null && rejectionReason.Trim() == "") return Helper.err("Rejection reason can't be empty");
            }
            var rec = dbc.Deposits.FirstOrDefault(d => d.Id == id && d.Status == "Pending");
            if (rec == null) return Helper.err("Deposit not found", 404);
            if (!dbc.WasteTypes.Any(w => w.Id == id)) return Helper.err("WasteType not found", 404);
            rec.WasteTypeId = wasteTypeId;
            rec.ActualWeight = actWeight;
            rec.OfficerId = userId;
            rec.UpdatedAt = DateTime.Now;
            rec.Status = verify ? "Verified" : "Rejected";
            if(rejectionReason != null && rejectionReason.Trim() != "") rec.RejectionReason = rejectionReason;
            if (photo != null) rec.PhotoPath = await Helper.UploadFile(uploadPath, photo, rec.PhotoPath);
            await dbc.SaveChangesAsync();
            return Helper.msg("Deposit updated successfully");
        }
    }
}

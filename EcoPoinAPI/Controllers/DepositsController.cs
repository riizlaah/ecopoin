using EcoPoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcoPoinAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositsController : ExtBaseController
    {
        private readonly EcoPoinContext dbc;
        private readonly string uploadDir;
        public DepositsController(EcoPoinContext dbc, IWebHostEnvironment env) { 
            this.dbc = dbc;
            uploadDir = Path.Combine(env.ContentRootPath, "wwwroot/uploads");
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAll(int page = 1, int size = 10, string status = "All")
        {
            var userId = getUserId();
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "resident";
            var query = dbc.Deposits.Include(d => d.Resident).Include(d => d.WasteType).Include(d => d.DepositPoint).OrderByDescending(d => d.UpdatedAt).AsQueryable();
            if(role == "officer")
            {
                query = query.Where(d => d.OfficerId == userId || d.OfficerId == null);
                if (status != "All") query = query.Where(d => d.Status == status);
                else query = query.OrderByDescending(d => d.Status == "Pending" ? 1 : 0).ThenByDescending(d => d.UpdatedAt);
            }
            if(role == "resident")
            {
                query = query.Where(d => d.ResidentId == userId);
                if (status != "All") query = query.Where(d => d.Status == status);
            }
            return PaginateQuery(query, page, size, d => new
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
                estPoints = d.EstimatedWeight * d.WasteType.PointTariff,
                actWeight = d.ActualWeight,
                actPoints = d.DepositPoint?.Amount,
                status = d.Status,
                updatedAt = d.UpdatedAt
            }, "Deposits fetched successfully");
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult GetAll(int id)
        {
            var userId = getUserId();
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "resident";
            var rec = dbc.Deposits.Include(d => d.Resident).Include(d => d.WasteType).Include(d => d.DepositPoint).Where(d => d.Id == id).ToList().Select(d => new
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
                estPoints = d.EstimatedWeight * d.WasteType.PointTariff,
                actWeight = d.ActualWeight,
                actPoints = d.DepositPoint?.Amount,
                status = d.Status,
                updatedAt = d.UpdatedAt,
                createdAt = d.CreatedAt,
                notes = d.Notes,
                rejectionReason = d.RejectionReason,
                photoPath = d.PhotoPath
            }).FirstOrDefault();
            if (rec == null) return err("Deposit not found", 404);
            if (role == "resident" && rec.resident.id != userId) return err("Forbidden", 403);
            return json(rec, "Deposit fetched successfully");
        }

        [HttpPost]
        [Authorize(Roles = "resident")]
        async public Task<ActionResult> Submit([FromForm] int wasteTypeId, [FromForm] decimal estWeight, IFormFile photo, [FromForm] string? notes = null)
        {
            if (photo == null || photo.Length == 0) return err("Photo required");
            var allowed = new[] { "image/jpeg", "image/png" };
            if (!allowed.Contains(photo.ContentType)) return err("Photo type must be JPG/PNG");
            if (wasteTypeId < 1) return err("Waste Type not valid");
            if (estWeight <= 0m) return err("Estimated weight not valid");
            if (!dbc.WasteTypes.Any(w => w.Id == wasteTypeId)) return err("Waste Type not found", 404);
            var userId = getUserId();
            await dbc.Deposits.AddAsync(new Deposit
            {
                ResidentId = userId,
                WasteTypeId = wasteTypeId,
                EstimatedWeight = estWeight,
                PhotoPath = await Upload(uploadDir, photo),
                Status = "Pending"
            });
            await dbc.SaveChangesAsync();
            return msg("Deposit submitted");
        }

        [HttpPut("{id}")]
        [Authorize]
        async public Task<ActionResult> Update(int id, [FromForm] int wasteTypeId, [FromForm] decimal estWeight, IFormFile? photo, [FromForm] string? notes = null)
        {
            if (photo != null)
            {
                if(photo.Length == 0) err("Photo required");
                var allowed = new[] { "image/jpeg", "image/png" };
                if (!allowed.Contains(photo.ContentType)) return err("Photo type must be JPG/PNG");
            }
            if (wasteTypeId < 1) return err("Waste Type not valid");
            if (estWeight <= 0m) return err("Estimated weight not valid");
            if (!dbc.WasteTypes.Any(w => w.Id == wasteTypeId)) return err("Waste Type not found", 404);
            var rec = await dbc.Deposits.FindAsync(id);
            if (rec == null) return err("Deposit not found");
            rec.WasteTypeId = wasteTypeId;
            rec.EstimatedWeight = estWeight;
            rec.Notes = notes;
            rec.UpdatedAt = DateTime.Now;
            if(photo != null) rec.PhotoPath = await Upload(uploadDir, photo, rec.PhotoPath);
            await dbc.SaveChangesAsync();
            return msg("Deposit updated");
        }

        [HttpPatch("{id}/verify")]
        [Authorize(Roles = "officer")]
        async public Task<ActionResult> Verify(int id, [FromForm] int wasteTypeId, [FromForm] decimal actWeight, IFormFile? photo, [FromForm] bool verify, [FromForm] string? rejectionReason = null)
        {
            if (photo != null)
            {
                if (photo.Length == 0) err("Photo required");
                var allowed = new[] { "image/jpge", "image/png" };
                if (!allowed.Contains(photo.ContentType)) return err("Photo type must be JPG/PNG");
            }
            if (wasteTypeId < 1) return err("Waste Type not valid");
            if (actWeight <= 0m) return err("Actual weight not valid");
            if (!dbc.WasteTypes.Any(w => w.Id == wasteTypeId)) return err("Waste Type not found", 404);
            var rec = await dbc.Deposits.Include(d => d.WasteType).FirstOrDefaultAsync(d => d.Id == id);
            if (rec == null) return err("Deposit not found");
            rec.OfficerId = getUserId();
            rec.WasteTypeId = wasteTypeId;
            rec.ActualWeight = actWeight;
            rec.UpdatedAt = DateTime.Now;
            rec.Status = verify ? "Verified" : "Rejected";
            if (photo != null) rec.PhotoPath = await Upload(uploadDir, photo, rec.PhotoPath);
            if(verify)
            {
                await dbc.DepositPoints.AddAsync(new DepositPoint
                {
                    DepositId = rec.Id,
                    Amount = (int)Math.Round(rec.WasteType.PointTariff * rec.ActualWeight ?? 0),
                    PointTariff = rec.WasteType.PointTariff,
                    ResidentId = rec.ResidentId,
                });
            } else
            {
                rec.RejectionReason = rejectionReason;
            }
            await dbc.SaveChangesAsync();
            return msg("Deposit updated");
        }
    }
}

using EcoPoinAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace EcoPoinAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepositsController : ControllerBase
    {
        private readonly EcoPoinContext dbc;
        private readonly string uploadDir;

        public DepositsController(EcoPoinContext dbc, IWebHostEnvironment env)
        {
            uploadDir = env.ContentRootPath;
            this.dbc = dbc;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAll(int page = 1, int size = 20)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "resident";
            var query = dbc.Deposits.Include(rec => rec.Resident).Include(rec => rec.WasteType).Include(rec => rec.DepositPoint)
                .Include(rec => rec.Officer).OrderByDescending(rec => rec.UpdatedAt).AsQueryable();
            if(role == "resident")
            {
                query = query.Where(rec => rec.ResidentId == userId);
            }
            if (role == "officer")
            {
                query = query.Where(rec => rec.Status == "Pending");
            }
            var (error, result, paging) = Helper.Paginate(query, rec => new
            {
                id = rec.Id,
                resident = new
                {
                    id = rec.ResidentId,
                    name = rec.Resident.FullName,
                    email = rec.Resident.Email
                },
                wasteType = new
                {
                    id = rec.WasteTypeId,
                    name = rec.WasteType.Name,
                    code = rec.WasteType.Code,
                    pointTariff = rec.WasteType.PointTariff,
                    isActive = rec.WasteType.IsActive
                },
                estimatedWeight = rec.EstimatedWeight,
                estimatedPoints = rec.EstimatedWeight * rec.WasteType.PointTariff,
                actualWeight = rec.ActualWeight,
                actualPoints = rec.ActualWeight == null ? null : rec.ActualWeight * rec.WasteType.PointTariff,
                status = rec.Status,
                updatedAt = rec.UpdatedAt
            }, page, size);
            if (error != "") return Helper.err(error);
            return Helper.paginate(result, page, size, paging?.total ?? 1, "Deposits fetched successfully");
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult Get(int id)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "resident";
            var rec = dbc.Deposits.AsNoTrackingWithIdentityResolution().FirstOrDefault(u => u.Id == id);
            if (rec == null) return Helper.err("Deposit not found", 404);
            if (role == "resident" && userId != rec.ResidentId) return Helper.err("Forbidden", 403);
            return Helper.json(new
            {
                id = rec.Id,
                resident = new
                {
                    id = rec.ResidentId,
                    name = rec.Resident.FullName,
                    email = rec.Resident.Email
                },
                wasteType = new
                {
                    id = rec.WasteTypeId,
                    name = rec.WasteType.Name,
                    code = rec.WasteType.Code,
                    pointTariff = rec.WasteType.PointTariff,
                    isActive = rec.WasteType.IsActive
                },
                estimatedWeight = rec.EstimatedWeight,
                estimatedPoints = rec.EstimatedWeight * rec.WasteType.PointTariff,
                actualWeight = rec.ActualWeight,
                actualPoints = rec.ActualWeight == null ? null : rec.ActualWeight * rec.WasteType.PointTariff,
                status = rec.Status,
                notes = rec.Notes,
                rejectionReason = rec.RejectionReason,
                photoPath = rec.PhotoPath,
                createdAt = rec.CreatedAt,
                updatedAt = rec.UpdatedAt
            }, "Deposit fetched successfully");
        }

        [HttpPost]
        [Authorize(Roles = "resident")]
        public async Task<ActionResult> Create([FromForm] int wasteTypeId, [FromForm] decimal estimatedWeight, IFormFile photo, [FromForm] string? notes = null)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (photo == null || photo.Length == 0) return Helper.err("Photo required");
            var allowed = new[] { "image/png", "image/jpeg" };
            if (!allowed.Contains(photo.ContentType)) return Helper.err("Photo must be png/jpg file");
            if (estimatedWeight <= 0m) return Helper.err("Estimated weight not valid");
            if (await dbc.WasteTypes.AnyAsync(w => w.Id == wasteTypeId)) return Helper.err("Waste Type not found", 404);
            dbc.Deposits.Add(new Deposit
            {
                ResidentId = userId,
                WasteTypeId = wasteTypeId,
                EstimatedWeight = estimatedWeight,
                Notes = notes,
                PhotoPath = await Helper.UploadFile(photo, uploadDir),
                Status = "Pending"
            });
            await dbc.SaveChangesAsync();
            return Helper.msg("Deposit submitted successfully");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "resident")]
        public async Task<ActionResult> Update(int id, [FromForm] int wasteTypeId, [FromForm] decimal estimatedWeight, IFormFile photo, [FromForm] string? notes = null)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (photo == null || photo.Length == 0) return Helper.err("Photo required");
            var allowed = new[] { "image/png", "image/jpeg" };
            if (!allowed.Contains(photo.ContentType)) return Helper.err("Photo must be png/jpg file");
            if (estimatedWeight <= 0m) return Helper.err("Estimated weight not valid");
            if (await dbc.WasteTypes.AnyAsync(w => w.Id == wasteTypeId)) return Helper.err("Waste Type not found", 404);
            var rec = await dbc.Deposits.FindAsync(id);
            if (rec == null) return Helper.err("Deposit not found", 404);
            if (rec.Status != "Pending") return Helper.err("Reviewed Deposit can't be updated");
            rec.WasteTypeId = wasteTypeId;
            rec.EstimatedWeight = estimatedWeight;
            rec.PhotoPath = await Helper.UploadFile(photo, uploadDir, rec.PhotoPath);
            rec.Notes = notes;
            rec.UpdatedAt = DateTime.Now;
            await dbc.SaveChangesAsync();
            return Helper.msg("Deposit updated successfully");
        }

        [HttpPut("{id}/verify")]
        [Authorize(Roles = "officer")]
        public async Task<ActionResult> Verify(int id, [FromForm] int wasteTypeId, [FromForm] decimal actualWeight, [FromForm] bool verified, IFormFile photo, [FromForm] string? rejectionReason = null)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (photo != null)
            {
                var allowed = new[] { "image/png", "image/jpeg" };
                if (photo.Length == 0) return Helper.err("Photo invalid");
                if (!allowed.Contains(photo.ContentType)) return Helper.err("Photo must be png/jpg file");
            }
            if(!verified)
            {
                if (rejectionReason == null || rejectionReason.Trim() == "") return Helper.err("Rejection reason can't be empty");
            }
            if (actualWeight <= 0m) return Helper.err("Actual weight not valid");
            if (await dbc.WasteTypes.AnyAsync(w => w.Id == wasteTypeId)) return Helper.err("Waste Type not found", 404);
            var rec = await dbc.Deposits.FindAsync(id);
            if (rec == null) return Helper.err("Deposit not found", 404);
            if (rec.Status != "Pending") return Helper.err("Reviewed Deposit can't be updated");
            rec.WasteTypeId = wasteTypeId;
            rec.ActualWeight = actualWeight;
            rec.UpdatedAt = DateTime.Now;
            if(photo != null)
            {
                rec.PhotoPath = await Helper.UploadFile(photo, uploadDir, rec.PhotoPath);
            }
            rec.Status = verified ? "Verified" : "Rejected";
            if(!verified) rec.RejectionReason = rejectionReason;
            else
            {
                dbc.DepositPoints.Add(new DepositPoint
                {
                    ResidentId = rec.ResidentId,
                    Amount = actualWeight * rec.WasteType.PointTariff,
                    PointTariff = rec.WasteType.PointTariff,
                    DepositId = rec.Id,
                });
            }
            await dbc.SaveChangesAsync();
            return Helper.msg("Deposit updated successfully");
        }
    }
}

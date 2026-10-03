using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermitSystem.Web.Data;
using PermitSystem.Web.Entities;

namespace PermitSystem.Web.Controllers.Api;

[Authorize]
[Route("api/branches")]
[ApiController]
public class BranchesApiController : ControllerBase
{
    private readonly AppDbContext _db;
    public BranchesApiController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Branches.Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .Select(b => new { b.Id, b.Name, b.Code, b.City, b.Manager, b.Phone, b.IsActive })
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var b = await _db.Branches.FindAsync(id);
        return b == null ? NotFound() : Ok(b);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Branch dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Code))
            return BadRequest(new { error = "الاسم والكود مطلوبان" });

        if (await _db.Branches.AnyAsync(b => b.Code == dto.Code))
            return BadRequest(new { error = "الكود مستخدم مسبقاً" });

        dto.Id = 0;
        dto.CreatedAt = DateTime.UtcNow;
        _db.Branches.Add(dto);
        await _db.SaveChangesAsync();
        return Ok(dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Branch dto)
    {
        var b = await _db.Branches.FindAsync(id);
        if (b == null) return NotFound();

        b.Name = dto.Name;
        b.Code = dto.Code;
        b.City = dto.City;
        b.Manager = dto.Manager;
        b.Phone = dto.Phone;
        b.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(b);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var b = await _db.Branches.FindAsync(id);
        if (b == null) return NotFound();

        if (await _db.Buildings.AnyAsync(x => x.BranchId == id))
            return BadRequest(new { error = "لا يمكن الحذف — يوجد مباني مرتبطة" });

        b.IsDeleted = true;
        await _db.SaveChangesAsync();
        return Ok();
    }
}
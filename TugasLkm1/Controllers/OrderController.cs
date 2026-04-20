using Microsoft.AspNetCore.Mvc;
using TugasLkm1.Models;
using TugasLkm1.Repositories;

namespace TugasLkm1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderRepository _repo;
        public OrdersController(OrderRepository repo) { _repo = repo; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _repo.GetAllAsync();
                return Ok(new { status = "success", meta = new { total = data.Count }, data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _repo.GetByIdAsync(id);
                if (data is null)
                    return NotFound(new { status = "error", message = "Order tidak ditemukan" });
                return Ok(new { status = "success", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderRequest req)
        {
            try
            {
                var data = await _repo.CreateAsync(req);
                return CreatedAtAction(nameof(GetById), new { id = data.Id },
                    new { status = "success", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderRequest req)
        {
            try
            {
                var data = await _repo.UpdateAsync(id, req);
                if (data is null)
                    return NotFound(new { status = "error", message = "Order tidak ditemukan" });
                return Ok(new { status = "success", data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _repo.DeleteAsync(id);
                if (!result)
                    return NotFound(new { status = "error", message = "Order tidak ditemukan" });
                return Ok(new { status = "success", message = "Order berhasil dihapus" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }
    }
}
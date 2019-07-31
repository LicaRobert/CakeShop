using AutoMapper;
using CakeShop.BindingModels;
using CakeShop.Data;
using CakeShop.Models;
using CakeShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReviewController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Review
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            return await _context.Reviews.ToListAsync();
        }

        // POST: api/review
        [HttpPost]
        [Route("PostReview")]
        public async Task<ActionResult<Review>> PostReview([FromForm] ReviewBindingModel reviewBindingModel)
        {

            try
            {
                var review = _mapper.Map<Review>(reviewBindingModel);
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetReviews), new { id = review.ReviewId }, review);
            }
            catch (Exception ex)
            {

            }

            return BadRequest();
        }

        // GET: api/Review/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewViewModel>> GetReview(int id)
        {
            try
            {
                var review = _context.Reviews.Include(c => c.CakeId).ToListAsync().Result.FirstOrDefault(c => c.CakeId == id);
                var reviewViewmodel = _mapper.Map<ReviewViewModel>(review);

                if (review == null)
                {
                    return NotFound();
                }

                return reviewViewmodel;
            }
            catch (Exception ex)
            {

            }

            return NotFound();
        }

        // DELETE: api/Review/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}

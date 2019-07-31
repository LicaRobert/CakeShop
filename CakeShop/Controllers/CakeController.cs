using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CakeShop.BindingModels;
using CakeShop.Data;
using CakeShop.Models;
using CakeShop.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CakeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CakeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        private const string SampleImage1 = "sample_image1.jpg";
        private const string SampleImage2 = "sample_image2.jpg";
        private const string SampleImage3 = "sample_image3.jpg";

        public CakeController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Cake
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CakeViewModel>>> GetCakes(string searchText = "", int categoryId = 0)
        {
            var cakes = new List<Cake>();
            var cakesViewModels =  new List<CakeViewModel>();
            try
            {
                if(string.IsNullOrEmpty(searchText))
                {
                    searchText = string.Empty;
                }

                if (categoryId == 0)
                {
                    cakes = await _context.Cakes.Include(cake => cake.Category).Where(c => c.Name.Contains(searchText)).ToListAsync();
                }
                else
                {
                    cakes = await _context.Cakes.Include(cake => cake.Category).Where(c => c.Name.Contains(searchText) && c.CategoryId == categoryId).ToListAsync();
                }

                cakesViewModels = _mapper.Map<List<CakeViewModel>>(cakes);

                // should move this in a service
                foreach (var cakeViewModel in cakesViewModels)
                {
                    if (cakeViewModel.ImageName != null)
                    {
                        cakeViewModel.Base64Image = ConvertImageToBase64String(cakeViewModel.ImageName);
                    }

                }
            }
            catch(Exception ex)
            {

            }

            return cakesViewModels;
        }

        // GET: api/GetSampleCakesImages
        [HttpGet]
        [Route("GetSampleCakesImages")]
        public async Task<ActionResult<SampleImagesViewModel>> GetSampleCakesImages()
        {
            try
            {
                var sampleImagesViewModels = new SampleImagesViewModel
                {
                    Base64Image1 = ConvertImageToBase64String(SampleImage1),
                    Base64Image2 = ConvertImageToBase64String(SampleImage2),
                    Base64Image3 = ConvertImageToBase64String(SampleImage3)
                };

                return sampleImagesViewModels;
            }
            catch (Exception ex)
            {

            }

            return NotFound();
        }

        // GET: api/Cake/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CakeViewModel>> GetCake(int id)
        {
            try
            {
                var cake = _context.Cakes.Include(c => c.Category).ToListAsync().Result.FirstOrDefault(c => c.CakeId == id);
                var cakeViewModel = _mapper.Map<CakeViewModel>(cake);
                cakeViewModel.Base64Image = ConvertImageToBase64String(cakeViewModel.ImageName);

                if (cake == null)
                {
                    return NotFound();
                }

                return cakeViewModel;
            }
            catch (Exception ex)
            {

            }

            return NotFound();
        }

        // POST: api/Cake
        [HttpPost]
        [Route("PostCake")]
        public async Task<ActionResult<Cake>> PostCake(IFormFile image, [FromForm] CakeBindingModel cakeBindingModel)
        {
            try
            {
                var cake = _mapper.Map<Cake>(cakeBindingModel);

                if (image != null && image.Length > 0)
                {
                    string fileName = DateTime.Now.ToString("ddMMyyhhmmss") + image.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                        cake.ImageName = fileName;
                    }
                }

                _context.Cakes.Add(cake);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCake), new { id = cake.CakeId }, cake);
            }
            catch (Exception ex)
            {

            }

            return BadRequest();
        }


        // PUT: api/Cake/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCake(long id, Cake cake)
        {
            if (id != cake.CakeId)
            {
                return BadRequest();
            }

            _context.Entry(cake).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Cake/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCake(int id)
        {
            var cake = await _context.Cakes.FindAsync(id);

            if (cake == null)
            {
                return NotFound();
            }

            _context.Cakes.Remove(cake);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private string ConvertImageToBase64String(string imageName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", imageName);
            if (System.IO.File.Exists(path))
            {
                byte[] b = System.IO.File.ReadAllBytes(path);
                return "data:image/jpg;base64," + Convert.ToBase64String(b);
            }

            return string.Empty;
        }

    }
}
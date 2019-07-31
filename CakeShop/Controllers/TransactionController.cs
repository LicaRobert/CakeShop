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
    public class TransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TransactionController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("PostTransaction")]
        public async Task<ActionResult<Transaction>> PostTransaction([FromForm] TransactionBindingModel transactionBindingModel)
        {
            try
            {
                var transaction = _mapper.Map<Transaction>(transactionBindingModel);

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

               return CreatedAtAction(nameof(GetTransaction), new { id = transaction.CakeId }, transaction);
            }
            catch (Exception ex)
            {

            }

            return BadRequest();
        }
        // GET: api/Transaction/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionViewModel>> GetTransaction(int id)
        {
            try
            {
                var cake = _context.Transactions.Include(c => c.CakeId).ToListAsync().Result.FirstOrDefault(c => c.CakeId == id);
                var cakeViewModel = _mapper.Map<TransactionViewModel>(cake);

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
    }
}
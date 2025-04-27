using api.Data;
using api.Dtos.StockDto;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepositery;

        public StockController(IStockRepository stockRepositery)
        {
            _stockRepositery = stockRepositery;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = await _stockRepositery.GetAllAsync(query);
            var stockModel = stock.Select(s => s.ToStockDto());
            return Ok(stockModel);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = await _stockRepositery.GetByIdAsync(id);

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequest stockDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stockModel = stockDto.ToStockFromCreateStockRequestDto();
            await _stockRepositery.CreateAsync(stockModel);

            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id}, stockModel.ToStockDto()); //Assigns a new ID on creation
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromBody] UpdateStockRequest stockDto, [FromRoute] int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = await _stockRepositery.UpdateAsync(id, stockDto);
            
            if (stock == null) 
            {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = await _stockRepositery.DeleteAsync(id);
            
            if (stock == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
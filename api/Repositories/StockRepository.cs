using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.StockDto;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class StockRepository : IStockRepository
    {   
        private readonly ApplicationDbContext _context;
        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> checkStockExists(int stockId)
        {
            return await _context.Stock.AnyAsync(s => s.Id == stockId);
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
           await _context.Stock.AddAsync(stockModel);
           await _context.SaveChangesAsync();
           return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stock.FirstOrDefaultAsync(s => s.Id == id);

            if (stockModel == null)
            {
                return null;
            }

            _context.Stock.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {

            var stocks = _context.Stock.Include(c => c.Comments).ThenInclude(a => a.AppUser).AsQueryable();

            if(!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if(!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if(!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if(query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            var stock = await _context.Stock.Include(c => c.Comments).ThenInclude(a => a.AppUser).FirstOrDefaultAsync(s => s.Id == id);

            if (stock == null)
            {
                return null;
            }

            return stock;
        }

        public Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return _context.Stock.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequest stockModel)
        {
            var stock = await _context.Stock.FirstOrDefaultAsync(s => s.Id == id);   
            
            if (stock == null) 
            {
                return null;
            }

            stock.Symbol = stockModel.Symbol;
            stock.CompanyName = stockModel.CompanyName;
            stock.Purchase = stockModel.Purchase;
            stock.LastDiv = stockModel.LastDiv;
            stock.Industry = stockModel.Industry;
            stock.MarketCap = stockModel.MarketCap;

            await _context.SaveChangesAsync();

            return stock;
        }
    }
}
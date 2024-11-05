using System.Net;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;

namespace server.Services
{
    public class PageService
    {
        private readonly AppDbContext _context;

        public PageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Page>> GetAllPages()
        {
            return await _context.Pages.ToListAsync();
        }

        public async Task<Page> GetPageBySlug(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                throw new HttpRequestException("Slug is required.", null, HttpStatusCode.BadRequest);
            }

            var page = await _context.Pages.FirstOrDefaultAsync(p => p.Slug == slug);
            if (page == null)
            {
                throw new HttpRequestException("Page not found.", null, HttpStatusCode.NotFound);
            }

            return page;
        }

        public async Task<Page> CreatePage(CreatePageDTO pageDto)
        {
            // Ensure Slug is unique
            if (await _context.Pages.AnyAsync(p => p.Slug == pageDto.Slug))
            {
                throw new HttpRequestException("Slug already exists. Please choose a different slug.", null, HttpStatusCode.Conflict);
            }

            // Create a new Page entity
            var page = new Page
            {
                Title = pageDto.Title,
                Slug = pageDto.Slug,
                Content = pageDto.Content
            };

            _context.Pages.Add(page);
            await _context.SaveChangesAsync();
            return page;
        }

        public async Task UpdatePage(int id, UpdatePageDTO pageDTO)
        {

            var existingPage = await _context.Pages.FindAsync(id)
                                ?? throw new HttpRequestException("Page not found.", null, HttpStatusCode.NotFound);

            // Ensure the new Slug is unique if it's changing
            if (pageDTO.Slug != existingPage.Slug && await _context.Pages.AnyAsync(p => p.Slug == pageDTO.Slug))
            {
                throw new HttpRequestException("Slug already exists. Please choose a different slug.", null, HttpStatusCode.Conflict);
            }

            // Update only the fields that have been provided
            if (pageDTO.Slug != null && pageDTO.Slug != existingPage.Slug)
            {
                existingPage.Slug = pageDTO.Slug;
            }

            if (!string.IsNullOrEmpty(pageDTO.Title))
            {
                existingPage.Title = pageDTO.Title;
            }

            if (!string.IsNullOrEmpty(pageDTO.Content))
            {
                existingPage.Content = pageDTO.Content;
            }

            // Update the last modified date
            existingPage.UpdatedAt = DateTime.UtcNow;

            _context.Pages.Update(existingPage);
            await _context.SaveChangesAsync();
        }


        public async Task DeletePage(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                throw new HttpRequestException("Page not found.", null, HttpStatusCode.NotFound);
            }

            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();
        }
    }
}

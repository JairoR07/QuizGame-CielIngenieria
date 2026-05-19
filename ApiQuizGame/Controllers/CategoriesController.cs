using ApiQuizGame.Data;
using ApiQuizGame.DTOs.Categories;
using ApiQuizGame.DTOs.Shared;
using ApiQuizGame.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiQuizGame.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly QuizGameDbContext _context;

        public CategoriesController(QuizGameDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves all categories ordered by level.
        /// </summary>
        /// <returns>List of categories.</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetCategories()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Level)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Level = c.Level,
                    Prize = c.Prize
                })
        .ToListAsync();

            return Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(categories,"Categories retrieved successfully"));
        }
        /// <summary>
        /// Retrieves a category by identifier.
        /// </summary>
        /// <param name="id">Category identifier.</param>
        /// <returns>Category information.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategory(int id)
        {
            var category = await _context.Categories
            .Where(c => c.Id == id)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Level = c.Level,
                Prize = c.Prize
            })
            .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(ApiResponse<string>.Fail("Category not found"));
            }

            return Ok(ApiResponse<CategoryDto>.Ok(category, "Category retrieved successfully"));
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="dto">Category information.</param>
        /// <returns>Created category.</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Level = dto.Level,
                Prize = dto.Prize
            };

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Level = category.Level,
                Prize = category.Prize
            };

            return CreatedAtAction(
            nameof(GetCategory),
            new { id = category.Id },
            ApiResponse<CategoryDto>.Ok(response, "Category created successfully"));
        }
        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">Category identifier.</param>
        /// <param name="dto">Updated category information.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateCategory(int id,UpdateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(ApiResponse<string>.Fail("Category not found"));
            }

            category.Name = dto.Name;
            category.Level = dto.Level;
            category.Prize = dto.Prize;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok(null, "Category updated successfully"));
        }

        /// <summary>
        /// Deletes a category.
        /// </summary>
        /// <param name="id">Category identifier.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(ApiResponse<string>.Fail("Category not found"));
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok(null,"Category deleted successfully"));
        }


    }
}

using AutoMapper;
using CarRentalAPI.DTOs;
using CarRentalAPI.Models;
using CarRentalAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly GenericRepository<CarCategory> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(GenericRepository<CarCategory> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            return Ok(categoryDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            var categoryDto = _mapper.Map<CategoryDTO>(category);
            return Ok(categoryDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDTO categoryDto)
        {
            var category = _mapper.Map<CarCategory>(categoryDto);
            await _categoryRepository.AddAsync(category);

            var categoryResultDto = _mapper.Map<CategoryDTO>(category);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, categoryResultDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CategoryUpdateDTO categoryDto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(categoryDto.Id);
            if (existingCategory == null)
                return NotFound();

            _mapper.Map(categoryDto, existingCategory);
            await _categoryRepository.UpdateAsync(existingCategory);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
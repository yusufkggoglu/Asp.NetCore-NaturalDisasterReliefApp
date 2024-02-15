using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Aid.Dtos;
using Services.Aid.Services;
using Shared.ControllerBases;
using System.Threading.Tasks;

namespace Services.Aid.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HumaneAidController : CustomControllerBase
    {
        public readonly IHumaneAidService _humaneAidService;

        public HumaneAidController(IHumaneAidService humaneAidService)
        {
            _humaneAidService = humaneAidService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _humaneAidService.GetAllAsync();
            return CreateActionResultInstance(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _humaneAidService.GetByIdAsync(id);
            return CreateActionResultInstance(response);
        }
        [HttpGet]
        [Route("/api/[controller]/GetAllByUserId/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string id)
        {
            var response = await _humaneAidService.GetAllByUserIdAsync(id);
            return CreateActionResultInstance(response);
        }
        [HttpPost]
        public async Task<IActionResult> Create(HumaneAidCreateDto dto)
        {
            var response = await _humaneAidService.CreateAsync(dto);
            return CreateActionResultInstance(response);
        }
        [HttpPut]
        public async Task<IActionResult> Update(HumaneAidUpdateDto dto)
        {
            var response = await _humaneAidService.UpdateAsync(dto);
            return CreateActionResultInstance(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _humaneAidService.DeleteAsync(id);
            return CreateActionResultInstance(response);
        }
    }
}

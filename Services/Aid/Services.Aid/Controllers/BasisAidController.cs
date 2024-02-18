using DnsClient.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Aid.ActionFilters;
using Services.Aid.Dtos;
using Services.Aid.Logging;
using Services.Aid.Services;
using Shared.ControllerBases;
using System.Threading.Tasks;

namespace Services.Aid.Controllers
{
    //[ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    [ResponseCache(CacheProfileName = "5mins")]
    public class BasisAidController : CustomControllerBase
    {
        private readonly IBasisAidService _basisAidService;
        public BasisAidController(IBasisAidService basisAidService)
        {
            _basisAidService = basisAidService;
        }

        [HttpGet]
        public async Task<IActionResult> GettAll()
        {
            var response = await _basisAidService.GetAllAsync();
            return CreateActionResultInstance(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _basisAidService.GetByIdAsync(id);
            return CreateActionResultInstance(response);
        }
        [HttpGet]
        [Route("/api/[controller]/GetAllByUserId/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var response = await _basisAidService.GetAllByUserIdAsync(userId);

            return CreateActionResultInstance(response);
        }
        [HttpPost]
        public async Task<IActionResult> Create(BasisAidCreateDto createDto)
        {
            var response = await _basisAidService.CreateAsync(createDto); 
            return CreateActionResultInstance(response);
        }
        [HttpPut] 
        public async Task<IActionResult> Update(BasisAidUpdateDto updateDto)
        {
            var response = await _basisAidService.UpdateAsync(updateDto);
            return CreateActionResultInstance(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _basisAidService.DeleteAsync(id);
            return CreateActionResultInstance(response);
        }
    }
}

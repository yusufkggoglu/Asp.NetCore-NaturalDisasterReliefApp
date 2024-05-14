using Services.Aid.Dtos;
using Shared.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Aid.Services
{
    public interface IHumaneAidService
    {
        public Task<Response<List<HumaneAidDto>>> GetAllAsync();
        public Task<Response<HumaneAidDto>> GetByIdAsync(string id);
        public Task<Response<List<HumaneAidDto>>> GetAllByUserIdAsync(string userId);
        public Task<Response<NoContent>> UpdateAsync(HumaneAidUpdateDto humaneAidUpdateDto);
        public Task<Response<HumaneAidDto>> CreateAsync(HumaneAidCreateDto humaneAidCreateDto);
        public Task<Response<NoContent>> DeleteAsync(string id);
        public Task DeleteOldData();
    }
}

using Services.Aid.Dtos;
using Shared.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Aid.Services
{
    public interface IBasisAidService
    {
        public Task<Response<List<BasisAidDto>>> GetAllAsync();
        public Task<Response<BasisAidDto>> GetByIdAsync(string id);
        public Task<Response<List<BasisAidDto>>> GetAllByUserIdAsync(string userId);
        public Task<Response<NoContent>> UpdateAsync(BasisAidUpdateDto basisAidUpdateDto);
        public Task<Response<BasisAidDto>> CreateAsync(BasisAidCreateDto basisAidCreateDto);
        public Task<Response<NoContent>> DeleteAsync(string id);
        public Task DeleteOldData();
    }
}

using AutoMapper;
using MongoDB.Driver;
using Services.Aid.Dtos;
using Services.Aid.Models;
using Services.Aid.Settings;
using Shared.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Aid.Services
{
    public class BasisAidService : IBasisAidService
    {
        private readonly IMongoCollection<BasisAid> _basisAidCollection;
        private readonly IMapper _mapper;

        public BasisAidService(IDatabaseSettings databaseSettings, IMapper mapper)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _basisAidCollection = database.GetCollection<BasisAid>(databaseSettings.BasisAidCollectionName);
            _mapper = mapper;
        }

        public async Task<Response<BasisAidDto>> CreateAsync(BasisAidCreateDto basisAidCreateDto)
        {
            var newAid = _mapper.Map<BasisAid>(basisAidCreateDto);
            newAid.CreatedTime = System.DateTime.Now;
            await _basisAidCollection.InsertOneAsync(newAid);
            return Response<BasisAidDto>.Success(_mapper.Map<BasisAidDto>(newAid),200);
        }

        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result = await _basisAidCollection.DeleteOneAsync(x => x.Id == id);
            if (result.DeletedCount > 0 )
            {
                return Response<NoContent>.Success(204);
            }

            return Response<NoContent>.Fail("Aid not found.", 404);
        }

        public async Task<Response<List<BasisAidDto>>> GetAllAsync()
        {
            var aids = await _basisAidCollection.Find(basisaid => true).ToListAsync();
            if (!aids.Any())
            {
                aids = new List<BasisAid>();
            }
            return Response<List<BasisAidDto>>.Success(_mapper.Map<List<BasisAidDto>>(aids), 200);
        }

        public async Task<Response<List<BasisAidDto>>> GetAllByUserIdAsync(string userId)
        {
            var aids = await _basisAidCollection.Find<BasisAid>(x => x.UserId == userId).ToListAsync();
            if (!aids.Any())
            {
                aids = new List<BasisAid>();

            }
          
            return Response<List<BasisAidDto>>.Success(_mapper.Map<List<BasisAidDto>>(aids), 200);
        }

        public async Task<Response<BasisAidDto>> GetByIdAsync(string id)
        {
            var aid = await _basisAidCollection.Find<BasisAid>(x => x.Id == id).FirstOrDefaultAsync();
            if (aid == null)
            {
                return Response<BasisAidDto>.Fail("Aid not found", 404);
            }
            return Response<BasisAidDto>.Success(_mapper.Map<BasisAidDto>(aid), 200);
        }

        public async Task<Response<NoContent>> UpdateAsync(BasisAidUpdateDto basisAidUpdateDto)
        {
            var updateAid = _mapper.Map<BasisAid>(basisAidUpdateDto);
            var result = await _basisAidCollection.FindOneAndReplaceAsync(x => x.Id == basisAidUpdateDto.Id, updateAid);
            if (result == null)
            {
                return Response<NoContent>.Fail("Aid not found", 404);
            }

            return Response<NoContent>.Success(204);
        }
    }
}

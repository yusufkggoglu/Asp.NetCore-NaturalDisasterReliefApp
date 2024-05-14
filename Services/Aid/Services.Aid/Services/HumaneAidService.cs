using AutoMapper;
using MongoDB.Driver;
using Services.Aid.Dtos;
using Services.Aid.Models;
using Services.Aid.Settings;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Aid.Services
{
    public class HumaneAidService : IHumaneAidService
    {
        private readonly IMongoCollection<HumaneAid> _humaneAidCollection;
        private readonly IMapper _mapper;

        public HumaneAidService(IDatabaseSettings databaseSettings, IMapper mapper)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _humaneAidCollection = database.GetCollection<HumaneAid>(databaseSettings.HumaneAidCollectionName);
            _mapper = mapper;
        }

        public async Task<Response<HumaneAidDto>> CreateAsync(HumaneAidCreateDto humaneAidCreateDto)
        {
            var newAid = _mapper.Map<HumaneAid>(humaneAidCreateDto);
            newAid.CreatedTime = DateTime.Now;
            await _humaneAidCollection.InsertOneAsync(newAid);
            return Response<HumaneAidDto>.Success(_mapper.Map<HumaneAidDto>(newAid), 200);
        }

        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result = await _humaneAidCollection.DeleteOneAsync<HumaneAid>(x => x.Id == id);
            if (result.DeletedCount > 0 )
            {
                return Response<NoContent>.Success(204);
            }
            return Response<NoContent>.Fail("Aid not found.", 404);
        }

        public async Task<Response<List<HumaneAidDto>>> GetAllAsync()
        {
            var result = await _humaneAidCollection.Find(aid => true).ToListAsync();
            if (!result.Any())
            {
                result = new List<HumaneAid>();
            }
            return Response<List<HumaneAidDto>>.Success(_mapper.Map<List<HumaneAidDto>>(result), 200);
        }

        public async Task<Response<List<HumaneAidDto>>> GetAllByUserIdAsync(string userId)
        {
            var result = await _humaneAidCollection.Find(x => x.UserId == userId).ToListAsync();
            if (!result.Any())
            {
                result = new List<HumaneAid>();
            }
            return Response<List<HumaneAidDto>>.Success(_mapper.Map<List<HumaneAidDto>>(result), 200);
        }

        public async Task<Response<HumaneAidDto>> GetByIdAsync(string id)
        {
            var result = await _humaneAidCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if(result == null)
            {
                return Response<HumaneAidDto>.Fail("Aid not found", 404);
            }
            return Response<HumaneAidDto>.Success(_mapper.Map<HumaneAidDto>(result), 200);
        }

        public async Task<Response<NoContent>> UpdateAsync(HumaneAidUpdateDto humaneAidUpdateDto)
        {
            var updateAid = _mapper.Map<HumaneAid>(humaneAidUpdateDto);
            var result = await _humaneAidCollection.FindOneAndReplaceAsync(x => x.Id == humaneAidUpdateDto.Id, updateAid);
            if (result == null)
            {
                return Response<NoContent>.Fail("Aid not found", 404);
            }
            return Response<NoContent>.Success(204);
        }

        public async Task DeleteOldData()
        {
            var thresholdDate = DateTime.Now.AddDays(-30); // 30 gün önceki tarih

            await _humaneAidCollection.DeleteManyAsync(data => data.CreatedTime < thresholdDate);
        }
    }
}

﻿using AutoMapper;
using Services.Aid.Dtos;
using Services.Aid.Models;

namespace Services.Aid.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping() 
        {
            CreateMap<BasisAid, BasisAidDto>().ReverseMap();
            CreateMap<BasisAid, BasisAidCreateDto>().ReverseMap();
            CreateMap<BasisAid, BasisAidUpdateDto>().ReverseMap();

            CreateMap<HumaneAid, HumaneAidDto>().ReverseMap();
            CreateMap<HumaneAid, HumaneAidCreateDto>().ReverseMap();
            CreateMap<HumaneAid, HumaneAidUpdateDto>().ReverseMap();
        }
    }
}

﻿using AutoMapper;
using Common.Models;
using Common.Models.Dto;
using Common.Models.ObjectResults;
using Common.Models.ResponseTypes;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace LofibayAPI.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TagController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetAll()
        {
            return Ok(new SuccessResponse
            {
                Data = await _unitOfWork.Tags.GetAllAsync()
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetById(int id)
        {
            return Ok(new SuccessResponse
            {
                Data = await _unitOfWork.Tags.GetByIdAsync(id)
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddTag(TagDto tagDto)
        {
            try
            {
                await _unitOfWork.Tags.AddAsync(_mapper.Map<Tag>(tagDto));
                await _unitOfWork.SaveChangesAsync();
                return Ok(new SuccessResponse
                {
                    Message = "A new tag has been added successfully."
                });
            }
            catch (Exception ex)
            {
                return new InternalServerError(new ErrorResponse
                {
                    Message = ex.Message
                });
            }
        }
    }
}
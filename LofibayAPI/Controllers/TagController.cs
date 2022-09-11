using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models.DTOs.Requests;
using Domain.Models.ResponseTypes;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetById(int id)
        {
            return Ok(new SuccessResponse<object>
            {
                Data = await _unitOfWork.Tags.GetByIdAsync(id)
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddTag(AddTagRequest tagDto)
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
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.Message });
            }
        }
    }
}

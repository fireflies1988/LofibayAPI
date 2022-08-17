using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models.DTOs.Requests;
using Domain.Models.ObjectResults;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetAll()
        {
            return Ok(new SuccessResponse<object>
            {
                Data = await _unitOfWork.Tags.GetAllAsync()
            });
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
                return Ok(new SuccessResponse<object>
                {
                    Message = "A new tag has been added successfully."
                });
            }
            catch (Exception ex)
            {
                return new InternalServerError(new ErrorResponse<object>
                {
                    Message = ex.Message
                });
            }
        }
    }
}

using AutoMapper;
using Domain.Interfaces;
using Domain.Models.DTOs.Responses;
using Domain.Models.ResponseTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LofibayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("Users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(new SuccessResponse<IEnumerable<UserInfoResponse>>
            {
                Data = _mapper.Map<IEnumerable<UserInfoResponse>>(await _unitOfWork.Users.GetAllAsync())
            });
        }
    }
}

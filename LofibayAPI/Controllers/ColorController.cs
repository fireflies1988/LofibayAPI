using Domain.Interfaces;
using Domain.Models.ResponseTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LofibayAPI.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ColorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllColors()
        {
            return Ok(new SuccessResponse
            {
                Data = await _unitOfWork.Colors.GetAllAsync()
            });
        }
    }
}

using EStore.Code;
using EStore.DTOs;
using EStore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;

    public AuthController(IAuthRepository authRepository)
    {
      _authRepository = authRepository;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(SignUpDTO dto)
    {
        var result = await _authRepository.SignUp(dto.Name, dto.Email, dto.Password, dto.ConfirmPassword);

        if (!result.Success)
        {
            return BadRequest(new 
            {
                success = false,
                message = result.Error
            });
        }

        return Ok(new
        {
            success = true,
            message = "User registered successfully" 
        });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var userToken = (await _authRepository.Login(dto.Email, dto.Password)).Data;

        if (string.IsNullOrEmpty(userToken as string))
        {
            return Unauthorized(new { success = false, message = "Invalid email or password" });
        }

        return Ok(new
        {
            success = true,
            token = userToken

        });
    }
}

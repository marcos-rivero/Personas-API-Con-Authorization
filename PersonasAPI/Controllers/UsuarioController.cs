using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonasAPI.Modelo;
using PersonasAPI.Modelo.Dto;
using PersonasAPI.Repositorio;

namespace PersonasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        protected ResponseDto _response;

        public UsuarioController(IUsuarioRepositorio usuarioRepositorio, ResponseDto response)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _response = response;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(UsuarioDto usuarioDto)
        {
            var resultado = await _usuarioRepositorio.Register(
            new Usuario { UserName = usuarioDto.UserName }, usuarioDto.Password);
            if(resultado == -1)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Usuario ya existe";
                return BadRequest(_response);
            }
            if(resultado == -500)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Error al crear el usuario";
                return BadRequest(_response);
            }

            _response.DisplayMessage = "Usuario creado con exito";
            _response.Result = resultado;
            return Ok(_response);
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(UsuarioDto user)
        {
            var result = await _usuarioRepositorio.Login(user.UserName, user.Password);
            if (result == "nouser")
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Usuario no existe";
                return BadRequest(_response);
            }
            if (result == "wrongpassword")
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Password incorrecta";
                return BadRequest(_response);
            }
            _response.Result = result;
            _response.DisplayMessage = "Usuario Conectado";
            return Ok(_response);
        }
    }
}

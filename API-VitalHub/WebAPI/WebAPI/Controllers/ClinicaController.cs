using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Domains;
using WebAPI.Interfaces;
using WebAPI.Repositories;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicaController : ControllerBase
    {
        private IClinicaRepository clinicaRepository;
        public ClinicaController()
        {
            clinicaRepository = new ClinicaRepository();
        }

        [HttpGet("ListarTodas")]
        public IActionResult Get()
        {
            return Ok(clinicaRepository.Listar());
        }

        [HttpGet("BuscarPorId")]
        public IActionResult GetById(Guid id)
        {
            return Ok(clinicaRepository.BuscarPorId(id));
        }

        [HttpGet("BuscarPorCidade")]
        public IActionResult GetByCity(string cidade)
        {
            return Ok(clinicaRepository.ListarPorCidade(cidade));
        }
    }
}

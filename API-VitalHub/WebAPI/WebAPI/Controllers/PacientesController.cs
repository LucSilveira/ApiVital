using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.Domains;
using WebAPI.Interfaces;
using WebAPI.Repositories;
using WebAPI.ViewModels;
using WebAPI.Utils;
using WebAPI.Utils.Mail;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacientesController : ControllerBase
    {
        private IPacienteRepository pacienteRepository { get; set; }

        // colocar na Program.cs a config da injection dependence => builder.Services.AddScoped<EmailSendingService>();
        private readonly EmailSendingService _emailSendingService;

        public PacientesController(EmailSendingService emailSendingService)
        {
            pacienteRepository = new PacienteRepository();
            _emailSendingService = emailSendingService;
        }

        [HttpGet("PerfilLogado")]
        public IActionResult BuscarLogado()
        {
            Guid idUsuario = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value);

            return Ok(pacienteRepository.BuscarPorId(idUsuario));
        }

        [Authorize]
        [HttpGet("BuscarPorId")]
        public IActionResult BuscarPorId(Usuario user)
        {
            Guid idUsuario = user.Id;

            return Ok(pacienteRepository.BuscarPorId(idUsuario));
        }


        // Define que este método é acessível através da rota "/SaveDirectory" via HTTP POST
        [HttpPost("SaveDirectory")]
        public async Task<IActionResult> PostDirectory([FromForm] PacienteViewModel form)
        {
            // Cria uma nova instância de Usuario
            Usuario user = new Usuario();

            // Recebe os valores e preenche as propriedades do objeto user
            user.Nome = form.Nome;
            user.Email = form.Email;
            user.TipoUsuarioId = form.IdTipoUsuario;

            // Se um arquivo de imagem foi enviado no formulário
            if (form.Arquivo != null)
            {
                // Chama o método de upload de imagem e obtém o nome do arquivo retornado.
                user.Foto = ImageUploader.UploadImage(form.Arquivo, Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Fotos"));
            }
            // Se nenhum arquivo de imagem foi enviado
            else
            {
                // Define uma imagem padrão para o usuário
                user.Foto = "padrao.png";
            }

            // Recebe os valores e preenche as propriedades do objeto user
            user.Senha = form.Senha;

            // Cria uma nova instância de Paciente
            user.Paciente = new Paciente
            {
                // Recebe os valores e preenche as propriedades do paciente
                DataNascimento = form.DataNascimento,
                Rg = form.Rg,
                Cpf = form.Cpf
            };

            // Cria uma nova instância de Médico
            user.Paciente.Endereco = new Endereco
            {
                Logradouro = form.Logradouro,
                Numero = form.Numero,
                Cep = form.Cep,
                Cidade = form.Cidade
            };

            // Chama o método Cadastrar do repositório de paciente para cadastrar o usuário
            pacienteRepository.Cadastrar(user);

            // Após registrar o usuário, envie o e-mail de boas-vindas
            await _emailSendingService.SendWelcomeEmail(user.Email!, user.Nome!);

            // Retorna uma resposta HTTP 200 (OK) para indicar que o processo foi concluído com sucesso
            return Ok();
        }


        [HttpPost("SaveBlobStorage")]
        public async Task<IActionResult> PostBlob([FromForm] PacienteViewModel form)
        {
            // Cria uma nova instância de Usuario
            Usuario user = new Usuario();

            // Recebe os valores e preenche as propriedades do objeto user
            user.Nome = form.Nome;
            user.Email = form.Email;
            user.TipoUsuarioId = form.IdTipoUsuario;

            // Define a string de conexão do Azure Blob Storage
            var connectionString = "";

            // Define o nome do contêiner do Azure Blob Storage
            var containerName = "containervitalhub";

            // Realiza o upload da imagem para o Blob Storage usando o método estático UploadImagemBlobAsync da classe AzureBlobStorageHelper
            user.Foto = await AzureBlobStorageHelper.UploadImagemBlobAsync(form.Arquivo!, connectionString, containerName);

            // Recebe os valores e preenche as propriedades do objeto user
            user.Senha = form.Senha;

            // Cria uma nova instância de Paciente
            user.Paciente = new Paciente
            {
                DataNascimento = form.DataNascimento,
                Rg = form.Rg,
                Cpf = form.Cpf
            };

            // Cria uma nova instância de Médico
            user.Paciente.Endereco = new Endereco
            {
                Logradouro = form.Logradouro,
                Numero = form.Numero,
                Cep = form.Cep,
                Cidade = form.Cidade
            };

            // Chama o método Cadastrar do repositório de paciente para cadastrar o usuário
            pacienteRepository.Cadastrar(user);

            // Após registrar o usuário, envie o e-mail de boas-vindas
            await _emailSendingService.SendWelcomeEmail(user.Email!, user.Nome!);

            // Retorna uma resposta HTTP 200 (OK) para indicar que o processo foi concluído com sucesso
            return Ok();
        }

        [HttpGet("BuscarPorData")]
        public IActionResult BuscarPorData(DateTime data, Guid id)
        {
            return Ok(pacienteRepository.BuscarPorData(data, id));
        }
    }
}
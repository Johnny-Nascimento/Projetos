using AutoMapper;
using Encurtador_De_Links.Data;
using Encurtador_De_Links.Data.Dto;
using Encurtador_De_Links.Models;
using Microsoft.AspNetCore.Mvc;

namespace Encurtador_De_Links.Controllers;

/*
    Criar roles para permissão de acesso as APIS
    Criar Login para possibilitar a compra de Link vitalicio e receber emails de inatividade
    Criar rotina para inativar links no banco após 24h (trigger?)
    Criar rotina para avisar via email e whatsapp o usuario que o link vai ficar inativo (mensageria kafka rabbit?)
    
    ******* SERÁ?
    Criar uma rotina para que a cada X horas altere o link gerado e envie por email / whatsapp (Pensar sobre a utilidade)
    Google ads
    Criar a possibilidade de assinar um link vitalicio (tentar vincular a um sistema financeiro real ou homologação)
 */

[ApiController]
[Route("[controller]")]
public class EncurtadorController : ControllerBase
{
    private LinkContext _Context;
    private IMapper _Mapper;

    public EncurtadorController(LinkContext context, IMapper mapper)
    {
        _Context = context;
        _Mapper = mapper;
    }

    // Vai ser publico sem roles
    [HttpPost]
    public IActionResult Post([FromBody] CreateLinkDto linkDto)
    {
        if (!Uri.TryCreate(linkDto.Original, UriKind.Absolute, out Uri? validatedUri))
        {
            if (validatedUri == null || (validatedUri.Scheme != Uri.UriSchemeHttp && validatedUri.Scheme != Uri.UriSchemeHttps))
                return BadRequest("Link inválido!");
        }

        Guid id = Guid.NewGuid();

        if (_Context?.Links?.FirstOrDefault(l => l.Id == id) != null)
            return Conflict($"Já existe um recurso com esse ID tente novamente.");

        string linkCurto = $"https://localhost:7245/Encurtador/{id}"; // Reduzir id

        Link linkEncurtado = new Link(
            id,
            DateTime.Now,
            linkDto.Original,
            linkCurto);

        // POR ENQUANTO ESTOU APENAS PASSANDO UM CAMPO MAS FICA APRA CONHEICMENTO A POSSIBILIDADE.
            // linkEncurtado = _Mapper.Map<Link>(linkDto);
        // POR ENQUANTO ESTOU APENAS PASSANDO UM CAMPO MAS FICA APRA CONHEICMENTO A POSSIBILIDADE.

        _Context?.Links?.Add(linkEncurtado);
        _Context?.SaveChanges();

        return CreatedAtAction("GetById", new { id = id }, linkEncurtado);
    }

    // Autenticação, não deve ser usado sem role de admin, via servir para ter um painel de controle
    [HttpGet]
    public IEnumerable<Link>? GetAll([FromQuery] bool trazerInativos, [FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        if (trazerInativos)
            return _Context?.Links?.Skip(skip).Take(take);
        else
            return _Context?.Links?.Where(l => l.Inativo == false).Skip(skip).Take(take);
    }

    // Vai ser publico sem roles
    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var linkCurto = _Context?.Links?.FirstOrDefault(l => l.Id == id);

        if (linkCurto == null)
            return NotFound();

        return Ok(linkCurto.Original);
    }

    // Autenticação, não deve ser usado sem role de admin, via servir para ter um painel de controle
    [HttpPut]
    public IActionResult Update([FromQuery]Guid id, [FromQuery] string link)
    {
        var linkCurto = _Context?.Links?.FirstOrDefault(l => l.Id == id);

        if (linkCurto == null)
            return NotFound();

        linkCurto.Update(link);

        _Context?.Links?.Update(linkCurto);
        _Context?.SaveChanges();

        return Ok(linkCurto);
    }
}

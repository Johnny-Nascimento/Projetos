using AutoMapper;
using Encurtador_De_Links.Data;
using Encurtador_De_Links.Data.Dto;
using Encurtador_De_Links.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Encurtador_De_Links.Controllers;

/*
    Criar roles para permissão de acesso a API
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

        // AUTOMAPPER
        // POR ENQUANTO ESTOU APENAS PASSANDO UM CAMPO URL MAS FICA APRA CONHECIMENTO A POSSIBILIDADE.
        // Link linkEncurtado1 = _Mapper.Map<Link>(linkDto);

        Link linkEncurtado = new Link(
            id,
            DateTime.Now,
            linkDto.Original,
            linkCurto);

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
    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] UpdateLinkDto linkDto)
    {
        if (!Uri.TryCreate(linkDto.Original, UriKind.Absolute, out Uri? validatedUri))
        {
            if (validatedUri == null || (validatedUri.Scheme != Uri.UriSchemeHttp && validatedUri.Scheme != Uri.UriSchemeHttps))
                return BadRequest("Link inválido!");
        }

        var link = _Context?.Links?.FirstOrDefault(l => l.Id == id);

        if (link == null)
            return NotFound();

        _Mapper.Map(linkDto, link);
        _Context?.SaveChanges();

        return NoContent();
    }

    /*
     * Como usar
        {
            "op": "replace",
            "path": "/Original",
            "value": "https://www.google.com"
        }
     */
    // Autenticação, não deve ser usado sem role de admin, via servir para ter um painel de controle
    [HttpPatch("{id}")]
    public IActionResult Patch(Guid id, JsonPatchDocument<UpdateLinkDto> patch)
    {
        var link = _Context?.Links?.FirstOrDefault(l => l.Id == id);

        if (link == null)
            return NotFound();

        var linkUpdate = _Mapper.Map<UpdateLinkDto>(link);
        patch.ApplyTo(linkUpdate);

        if (!TryValidateModel(linkUpdate))
            return ValidationProblem(ModelState);

        if (!Uri.TryCreate(linkUpdate.Original, UriKind.Absolute, out Uri? validatedUri))
        {
            if (validatedUri == null || (validatedUri.Scheme != Uri.UriSchemeHttp && validatedUri.Scheme != Uri.UriSchemeHttps))
                return BadRequest("Link inválido!");
        }

        _Mapper.Map(linkUpdate, link);
        _Context?.SaveChanges();

        return NoContent();
    }

    /*
     * Deleção não vai deletar, apenas alterar para invativo.
     */
    // Autenticação, não deve ser usado sem role de admin, via servir para ter um painel de controle
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var link = _Context?.Links?.FirstOrDefault(l => l.Id == id);

        if (link == null)
            return NotFound();

        _Context?.Remove(link);
        _Context?.SaveChanges();

        return NoContent();
    }
}

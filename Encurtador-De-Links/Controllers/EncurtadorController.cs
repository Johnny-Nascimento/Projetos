using Encurtador_De_Links.Data;
using Encurtador_De_Links.Models;
using Microsoft.AspNetCore.Mvc;

namespace Encurtador_De_Links.Controllers;

[ApiController]
[Route("[controller]")]
public class EncurtadorController : ControllerBase
{
    private LinkContext _Context;

    public EncurtadorController(LinkContext context)
    {
        _Context = context;
    }

    [HttpPost]
    public IActionResult Post([FromQuery] string link)
    {
        if (!Uri.TryCreate(link, UriKind.Absolute, out Uri validatedUri))
        {
            if (validatedUri == null || (validatedUri.Scheme != Uri.UriSchemeHttp && validatedUri.Scheme != Uri.UriSchemeHttps))
                return BadRequest("Link inválido!");
        }

        Guid id = Guid.NewGuid();

        if (_Context.Links?.FirstOrDefault(l => l.Id == id) != null)
            return Conflict($"Já existe um recurso com esse ID tente novamente.");

        string linkCurto = $"https://localhost:7245/Encurtador/{id}"; // Reduzir id

        Link linkEncurtado = new Link(
            id,
            DateTime.Now,
            link,
            linkCurto);

        _Context.Links.Add(linkEncurtado);
        _Context.SaveChanges();

        return CreatedAtAction("GetById", new { id = id }, linkEncurtado);
    }

    // Autenticação, não deve ser usado sem role de admin, via servir para ter um painel de controle
    [HttpGet]
    public IEnumerable<Link>? GetAll([FromQuery] bool trazerInativos, [FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        if (trazerInativos)
            return _Context.Links.Skip(skip).Take(take);
        else
            return _Context.Links?.Where(l => l.Inativo == false).Skip(skip).Take(take);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var linkCurto = _Context.Links?.FirstOrDefault(l => l.Id == id);

        if (linkCurto == null)
            return NotFound();

        return Ok(linkCurto.Original);
    }

    [HttpPut]
    public IActionResult Update([FromQuery]Guid id, [FromQuery] string link)
    {
        var linkCurto = _Context.Links?.FirstOrDefault(l => l.Id == id);

        if (linkCurto == null)
            return NotFound();

        linkCurto.Update(link);

        _Context.Links.Update(linkCurto);
        _Context.SaveChanges();

        return Ok(linkCurto);
    }
}

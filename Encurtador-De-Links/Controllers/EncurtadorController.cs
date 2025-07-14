using Encurtador_De_Links.Models;
using Microsoft.AspNetCore.Mvc;

namespace Encurtador_De_Links.Controllers;

[ApiController]
[Route("[controller]")]
public class EncurtadorController : ControllerBase
{
    private static List<Link>? Links { get; set; } = new List<Link>(); // Futuramente Readonly, banco de dados de vdd

    [HttpPost]
    public IActionResult Post([FromQuery] string link)
    {
        if (!Uri.TryCreate(link, UriKind.Absolute, out Uri validatedUri))
        {
            if (validatedUri == null || (validatedUri.Scheme != Uri.UriSchemeHttp && validatedUri.Scheme != Uri.UriSchemeHttps))
                return BadRequest("Link inválido!");
        }

        Guid id = Guid.NewGuid();

        if (Links?.FirstOrDefault(l => l.Id == id) != null)
            return Conflict($"Já existe um recurso com esse ID tente novamente.");

        string linkCurto = $"https://localhost:7245/Encurtador/{id}"; // Reduzir id

        Link linkEncurtado = new Link(
            id,
            DateTime.Now,
            link,
            linkCurto);

        Links.Add(linkEncurtado);

        return CreatedAtAction("GetById", new { id = id }, linkEncurtado);
    }

    // Autenticação, não deve ser usado sem role de admin, via servir para ter um painel de controle
    [HttpGet]
    public IEnumerable<Link>? GetAll([FromQuery] bool trazerInativos, [FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        if (trazerInativos)
            return Links.Skip(skip).Take(take);
        else
            return Links?.Where(l => l.Inativo == false).Skip(skip).Take(take);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var linkCurto = Links?.FirstOrDefault(l => l.Id == id);

        if (linkCurto == null)
            return NotFound();

        return Ok(linkCurto.Original);
    }

    [HttpPut(Name = "UpdateEncurtador/{linkUpdate}")]
    public IActionResult Update(Link linkUpdate)
    {
        var linkCurto = Links?.FirstOrDefault(l => l.Id == linkUpdate.Id);

        if (linkCurto == null)
            return NotFound();

        linkCurto.Update(linkUpdate);

        return Ok(linkCurto);
    }
}

using Encurtador_De_Links.Models;
using Microsoft.AspNetCore.Mvc;

namespace Encurtador_De_Links.Controllers;

[ApiController]
[Route("[controller]")]
public class EncurtadorController : ControllerBase
{
    private static List<LinkEncurtado>? Links { get; set; } = new List<LinkEncurtado>(); // Futuramente Readonly, banco de dados de vdd

    [HttpPost]
    public IActionResult Post([FromBody] Link link)
    {
        Guid id = Guid.NewGuid();

        if (Links?.FirstOrDefault(l => l.Id == id) != null)
            return Conflict($"Já existe um recurso com esse ID tente novamente.");

        string linkCurto = $"https://localhost:7245/Encurtador/{id}"; // dominio/id

        LinkEncurtado linkEncurtado = new LinkEncurtado(
            id,
            DateTime.Now,
            link.Original,
            linkCurto,
            inativo: false);

        Links.Add(linkEncurtado);

        return CreatedAtAction("GetById", new { id = id }, linkEncurtado);
    }

    // Autenticação, não deve ser usado sem role de admin, via servir para ter um painel de controle
    [HttpGet]
    public IEnumerable<LinkEncurtado>? GetAll([FromQuery] bool trazerInativos, [FromQuery] int skip = 0, [FromQuery] int take = 50)
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

    // Retornar sucesso/erro
    // [HttpPut(Name = "UpdateEncurtador/{linkUpdate}")]
    // public void Update(LinkUpdate linkUpdate)
    // {
    //     LinkEncurtado? linkCurto = GetById(linkUpdate.Id);
    //     if (linkCurto != null)
    //         linkCurto.Original = "teste";
    // }
}

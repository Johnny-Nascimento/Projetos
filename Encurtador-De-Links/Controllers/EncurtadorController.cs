using Encurtador_De_Links.Models;
using Microsoft.AspNetCore.Mvc;

namespace Encurtador_De_Links.Controllers;

[ApiController]
[Route("[controller]")]
public class EncurtadorController : ControllerBase
{
    private List<LinkEncurtado>? Links { get; set; } // Futuramente Readonly, banco de dados de vdd

    [HttpPost(Name = "PostEncurtador")]
    public void Post([FromBody] Link link)
    {
        Guid id = Guid.NewGuid(); // Garantir unicidade?

        string linkCurto = $"https://localhost:7245/Encurtador/{id}";

        LinkEncurtado linkEncurtado = new LinkEncurtado(id, DateTime.Now, link.Original, linkCurto, inativo:false);

        Links.Add(linkEncurtado);

        foreach (var item in Links)
            Console.WriteLine(item.ToString());
    }

    [HttpGet(Name = "GetEncurtador/{id}")]
    public string? GetById(Guid id)
    {
        var linkCurto = Links?.FirstOrDefault(l => l.Id == id);

        return linkCurto?.Original;
    }

    // Autenticação, não deve ser usado sem role de admin, via servir para ter um painel de controle
    [HttpGet(Name = "GetEncurtador/{inativo}")]
    public IEnumerable<LinkEncurtado> GetAll(bool trazerInativos)
    {
        if (trazerInativos)
            return Links;
        else
            return Links.Where(l => l.Inativo == false);
    }

    // Retornar sucesso/erro
    [HttpGet(Name = "UpdateEncurtador/{linkUpdate}")]
    public void Update(LinkUpdate linkUpdate)
    {
        LinkEncurtado? linkCurto = GetById(linkUpdate.Id);
        if (linkCurto != null)
            linkCurto.Original = "teste";
    }
}

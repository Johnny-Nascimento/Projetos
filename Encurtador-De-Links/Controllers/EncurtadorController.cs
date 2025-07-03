using Encurtador_De_Links.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Encurtador_De_Links.Controllers;

[ApiController]
[Route("[controller]")]
public class EncurtadorController : ControllerBase
{
    private List<LinkEncurtado> Links = new List<LinkEncurtado>(); // Futuramente Readonly

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
    public LinkEncurtado? GetById(Guid id)
    {
        var linkCurto = Links.FirstOrDefault(l => l.Id == id);

        // var v = Redirect(linkCurto.Encurtado);

        return linkCurto;
    }
}

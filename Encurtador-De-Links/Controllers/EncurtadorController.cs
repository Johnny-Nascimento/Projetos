using Encurtador_De_Links.Models;
using Microsoft.AspNetCore.Mvc;

namespace Encurtador_De_Links.Controllers;

[ApiController]
[Route("[controller]")]
public class EncurtadorController : ControllerBase
{
    private readonly List<LinkEncurtado> Links = new List<LinkEncurtado>();

    [HttpPost(Name = "PostEncurtador")]
    public void AdicionaLink([FromBody] Link link)
    {
        Guid id = Guid.NewGuid();

        string linkCurto = $"https://localhost:7245/Encurtador/{id}";

        LinkEncurtado linkEncurtado = new LinkEncurtado(id, DateTime.Now, link.Original, linkCurto, inativo:false);

        Links.Add(linkEncurtado);

        foreach (var item in Links)
            Console.WriteLine(item.ToString());
    }
}

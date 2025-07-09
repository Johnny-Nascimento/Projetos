using System.ComponentModel.DataAnnotations;

namespace Encurtador_De_Links.Models;

public class Link
{
    [Required(ErrorMessage = "Link original é obrigatório")]
    [Url(ErrorMessage = "Link inválido")]
    public string Original{ get; set; } = string.Empty; // Talvez remover classe e trabalhar com URI ou URL ou alguma classe especifica para links
}

public class LinkUpdate : Link
{
    public Guid Id { get; set; }
}

public class LinkEncurtado
{
    public Guid Id { get; }
    public DateTime DataHoraCriacao { get; }
    public string Original { get; private set;  } = string.Empty; // Talvez remover classe e trabalhar com URI ou URL ou alguma classe especifica para links
    public string Encurtado { get; } = string.Empty; // Talvez remover classe e trabalhar com URI ou URL ou alguma classe especifica para links
    public bool Inativo { get; private set; }

    public LinkEncurtado(Guid id, DateTime dataHoraCriacao, string original, string encurtado, bool inativo)
    {
        Id = id;
        DataHoraCriacao = dataHoraCriacao;
        Original = original;
        Encurtado = encurtado;
        Inativo = inativo;
    }

    public void Inativar()
    {
        Inativo = true;
    }

    public void Update(LinkUpdate link)
    {
        Original = link.Original;
    }
}

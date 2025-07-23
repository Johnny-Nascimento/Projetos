using System.ComponentModel.DataAnnotations;

namespace Encurtador_De_Links.Models;

public class Link
{
    [Key]
    [Required]
    public Guid Id { get; private set; }

    [Required]
    public DateTime DataHoraCriacao { get; private set; }

    [Required]
    [Url]
    public string Original { get; private set; } = string.Empty; // Talvez remover classe e trabalhar com URI ou URL ou alguma classe especifica para links

    [Required]
    public string Encurtado { get; private set; } = string.Empty; // Talvez remover classe e trabalhar com URI ou URL ou alguma classe especifica para links

    [Required]
    public bool Inativo { get; private set; }

    public Link(Guid id, DateTime dataHoraCriacao, string original, string encurtado, bool inativo = false)
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

    public void Update(string link)
    {
        Original = link;
    }
}

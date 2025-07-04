using System.ComponentModel.DataAnnotations;

namespace Encurtador_De_Links.Models;

public class Link
{
    [Required(ErrorMessage = "Link original é obrigatório")]
    [Url]
    public string Original{ get; set; } = string.Empty; // Talvez remover classe e trabalhar com URI ou URL ou alguma classe especifica para links

    public override string ToString()
    {
        return String.Format("Original:{0}", Original);
    }
}

public class LinkUpdate : Link
{
    public Guid Id { get; set; }

    public override string ToString()
    {
        return String.Format("{0} Id:{1}", base.ToString(), Id);
    }
}

public class LinkEncurtado
{
    public Guid Id { get; }
    public DateTime DataHoraCriacao { get; }
    public string Original { get; } = string.Empty; // Talvez remover classe e trabalhar com URI ou URL ou alguma classe especifica para links
    public string Encurtado { get; } = string.Empty; // Talvez remover classe e trabalhar com URI ou URL ou alguma classe especifica para links
    public bool Inativo { get; }

    public LinkEncurtado(Guid id, DateTime dataHoraCriacao, string original, string encurtado, bool inativo)
    {
        Id = id;
        DataHoraCriacao = dataHoraCriacao;
        Original = original;
        Encurtado = encurtado;
        Inativo = inativo;
    }

    public override string ToString()
    {
        return String.Format("Id:{0}, DataHoraCriacao:{1}, Original:{2}, Encurtado:{3}, Inativo:{4}", Id, DataHoraCriacao, Original, Encurtado, Inativo);
    }
}

using System.ComponentModel.DataAnnotations;

namespace Encurtador_De_Links.Data.Dto;

public class CreateLinkDto
{
    public string Original { get; set; } = string.Empty; // Talvez remover classe e trabalhar com URI ou URL ou alguma classe especifica para links
}

public class UpdateLinkDto
{
    public string Original { get; set; } = string.Empty; // Talvez remover classe e trabalhar com URI ou URL ou alguma classe especifica para links
}

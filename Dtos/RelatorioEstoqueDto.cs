namespace JogosApi.Dtos;

public class RelatorioEstoqueDto
{
    public string Plataforma { get; set; } = string.Empty;
    public int QuantidadeTitulos { get; set; }
    public decimal ValorTotalInventario { get; set; }

    public int QuantidadeTotalTitulos => QuantidadeTitulos;
    public decimal ValorTotalEstoque => ValorTotalInventario;
}

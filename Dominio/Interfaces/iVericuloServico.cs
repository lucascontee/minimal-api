using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Infraestrutura.Infraestrutura
{
    public interface iVeiculoServico
    {
        List<Veiculo?> Todos(int pagina = 1, string? nome = null, string? marca = null );
        Veiculo? BuscaPorId(int id);
        void Incluir(Veiculo veiculo);
        void Atualizar(Veiculo veiculo);
        void ApagarPorId(Veiculo veiculo);
    }
}
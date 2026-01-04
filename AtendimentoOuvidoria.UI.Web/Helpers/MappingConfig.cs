using System.Reflection;
using AtendimentoOuvidoria.Model;
using Mapster;
using AtendimentoOuvidoria.UI.Web.Models;

namespace AtendimentoOuvidoria.UI.Web.Helpers
{
    public static class MappingConfig
    {
        public static void RegisterMaps(IServiceCollection services)
        {
            TypeAdapterConfig<AtendimentoMOD, AgendamentoAtendimentoViewMOD>
                .NewConfig()
                .Map(dest => dest.Agendamento, src => src.Agendamento)
                .Map(dest => dest.MotivoAusencia, src => src.MotivoAusencia)
                .Map(dest => dest.Humor, src => src.Humor)
                .IgnoreNonMapped(true)
                .TwoWays();

            //INTRANET
            TypeAdapterConfig<UsuarioMOD, UsuarioViewMOD>
             .NewConfig()
             .Map(member => member.Id, source => source.CdUsuario)
             .Map(member => member.Nome, source => source.NmUsuario)
             .Map(member => member.Cpf, source => source.NrCpf)
             .Map(member => member.Avatar, source => source.Avatar)
             .TwoWays()
             .IgnoreNonMapped(true);

            TypeAdapterConfig<AvatarMOD, AvatarViewMOD>
             .NewConfig()
             .Map(member => member.Id, source => source.CdAvatar)
             .Map(member => member.Foto, source => source.TxLocalFoto)
             .Map(member => member.Tipo, source => source.TpAvatar)
             .TwoWays()
             .IgnoreNonMapped(true);

            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }
    }
}

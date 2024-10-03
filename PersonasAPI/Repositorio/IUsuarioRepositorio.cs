using PersonasAPI.Modelo;

namespace PersonasAPI.Repositorio
{
    public interface IUsuarioRepositorio
    {
        Task<int> Register(Usuario usuaio, string password);
        Task<string> Login(string userName, string password);
        Task<bool> UserExiste(string userName);
    }
}

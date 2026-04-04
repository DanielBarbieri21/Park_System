using Estacionamento.Abstractions;
using Estacionamento.Models;
using Estacionamento.Repositories;
using Estacionamento.Security;
using System;

namespace Estacionamento.Services
{
    public sealed class AuthenticationService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogService _log;

        public AuthenticationService(IUsuarioRepository usuarioRepository, ILogService log)
        {
            _usuarioRepository = usuarioRepository;
            _log = log;
        }

        public void SeedDefaultAdminIfMissing()
        {
            if (_usuarioRepository.ExisteAdmin())
            {
                return;
            }

            var credentials = PasswordHasher.HashPassword("246895");
            _usuarioRepository.Adicionar(new Usuario
            {
                Nome = "Administrador",
                Email = "admin",
                Login = "admin",
                SenhaHash = credentials.Hash,
                SenhaSalt = credentials.Salt,
                Tipo = TipoUsuario.Admin
            });

            _log.Info("Usuário admin padrão criado.");
        }

        public Usuario? Autenticar(string identificador, string senha)
        {
            if (string.IsNullOrWhiteSpace(identificador) || string.IsNullOrWhiteSpace(senha))
            {
                return null;
            }

            var usuario = _usuarioRepository.ObterPorEmail(identificador.Trim())
                ?? _usuarioRepository.ObterPorLogin(identificador.Trim());

            if (usuario == null)
            {
                return null;
            }

            if (PasswordHasher.Verify(senha, usuario.SenhaHash, usuario.SenhaSalt))
            {
                return usuario;
            }

            if (!string.IsNullOrWhiteSpace(usuario.Senha) && usuario.Senha == senha)
            {
                var credentials = PasswordHasher.HashPassword(senha);
                _usuarioRepository.AtualizarCredenciais(usuario.Id, credentials.Hash, credentials.Salt);
                usuario.SenhaHash = credentials.Hash;
                usuario.SenhaSalt = credentials.Salt;
                _log.Info($"Migração de senha legada executada para usuário {usuario.Login}.");
                return usuario;
            }

            return null;
        }
    }
}

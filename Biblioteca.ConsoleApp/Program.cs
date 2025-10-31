using Biblioteca.Data;
using Biblioteca.Domain;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.ConsoleApp
{
    internal class Program
    {
        private static void Main()
        {
            ExecutarMenu();
        }

        private static void ExecutarMenu()
        {
            while (true)
            {
                ExibirMenuPrincipal();
                Console.Write("Escolha uma opção: ");
                string? escolha = Console.ReadLine();

                switch (escolha)
                {
                    case "1": MenuAdicionarAutor(); break;
                    case "2": MenuAdicionarLivro(); break;
                    case "3": ConsultarAutores(); break;
                    case "4": MenuConsultarAutorComLivros(); break;
                    case "5": MenuAtualizarSobrenomeAutor(); break;
                    case "6": MenuExcluirAutor(); break;
                    case "0":
                        Console.WriteLine("Saindo...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }

                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }

        #region Menus Interativos

        private static void ExibirMenuPrincipal()
        {
            Console.Clear();
            Console.WriteLine("=== Menu Biblioteca EF Core ===");
            Console.WriteLine("1. Adicionar Novo Autor");
            Console.WriteLine("2. Adicionar Livro para Autor Existente");
            Console.WriteLine("3. Listar Todos os Autores");
            Console.WriteLine("4. Listar Autor com Seus Livros");
            Console.WriteLine("5. Atualizar Sobrenome do Autor");
            Console.WriteLine("6. Excluir Autor (sem livros)");
            Console.WriteLine("0. Sair");
            Console.WriteLine("==============================");
        }

        private static void MenuAdicionarAutor()
        {
            Console.Write("Digite o Primeiro Nome do Autor: ");
            string? nome = Console.ReadLine();
            Console.Write("Digite o Sobrenome do Autor: ");
            string? sobrenome = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(sobrenome))
            {
                Console.WriteLine("❌ Nome e sobrenome não podem ser vazios.");
                return;
            }

            AdicionarAutor(nome, sobrenome);
        }

        private static void MenuAdicionarLivro()
        {
            if (!LerInt("Digite o ID do Autor para adicionar o livro: ", out int autorId))
                return;

            Console.Write("Digite o Título do Livro: ");
            string? titulo = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(titulo))
            {
                Console.WriteLine("❌ O título não pode ser vazio.");
                return;
            }

            if (!LerInt("Digite o Ano de Publicação: ", out int ano)) return;
            if (!LerDecimal("Digite o Preço do Livro (ex: 29,90): ", out decimal preco)) return;

            AdicionarLivroParaAutor(autorId, titulo, ano, preco);
        }

        private static void MenuConsultarAutorComLivros()
        {
            if (LerInt("Digite o ID do Autor para ver os detalhes: ", out int autorId))
                ConsultarAutorComLivros(autorId);
        }

        private static void MenuAtualizarSobrenomeAutor()
        {
            if (!LerInt("Digite o ID do Autor para atualizar o sobrenome: ", out int autorId))
                return;

            Console.Write("Digite o NOVO Sobrenome: ");
            string? novoSobrenome = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(novoSobrenome))
            {
                Console.WriteLine("❌ Novo sobrenome não pode ser vazio.");
                return;
            }

            AtualizarSobrenomeAutor(autorId, novoSobrenome);
        }

        private static void MenuExcluirAutor()
        {
            if (LerInt("Digite o ID do Autor para excluir: ", out int autorId))
                ExcluirAutor(autorId);
        }

        #endregion

        #region Operações de Banco de Dados

        private static void AdicionarAutor(string primeiroNome, string ultimoNome)
        {
            using var context = new BibliotecaContext();
            var autor = new Author { FirstName = primeiroNome, LastName = ultimoNome };

            context.Authors.Add(autor);
            int registros = context.SaveChanges();

            Console.WriteLine(registros > 0
                ? $"✅ Autor '{autor.FirstName} {autor.LastName}' adicionado com sucesso (ID: {autor.AuthorId})."
                : "⚠️ Nenhum autor foi adicionado.");
        }

        private static void AdicionarLivroParaAutor(int authorId, string titulo, int anoPublicacao, decimal preco)
        {
            using var context = new BibliotecaContext();

            var autor = context.Authors.Find(authorId);
            if (autor == null)
            {
                Console.WriteLine($"❌ Autor com ID {authorId} não encontrado.");
                return;
            }

            var livro = new Book
            {
                Title = titulo,
                PublishDate = new DateTime(anoPublicacao, 1, 1),
                Price = preco,
                AuthorId = authorId
            };

            context.Books.Add(livro);
            int registros = context.SaveChanges();

            Console.WriteLine(registros > 0
                ? $"📘 Livro '{livro.Title}' adicionado com sucesso (ID: {livro.BookId})."
                : "⚠️ Nenhum livro foi adicionado.");
        }

        private static void ConsultarAutores()
        {
            using var context = new BibliotecaContext();
            var autores = context.Authors
                .AsNoTracking()
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName)
                .ToList();

            Console.WriteLine("\n--- LISTA DE AUTORES ---");

            if (!autores.Any())
            {
                Console.WriteLine("Nenhum autor encontrado.");
                return;
            }

            foreach (var a in autores)
                Console.WriteLine($"ID {a.AuthorId}: {a.FirstName} {a.LastName}");

            Console.WriteLine("-------------------------");
        }

        private static void ConsultarAutorComLivros(int authorId)
        {
            using var context = new BibliotecaContext();
            var autor = context.Authors
                .Include(a => a.Books)
                .AsNoTracking()
                .FirstOrDefault(a => a.AuthorId == authorId);

            if (autor == null)
            {
                Console.WriteLine($"❌ Autor com ID {authorId} não encontrado.");
                return;
            }

            Console.WriteLine($"\n--- DETALHES DO AUTOR ID {authorId} ---");
            Console.WriteLine($"Autor: {autor.FirstName} {autor.LastName}");
            Console.WriteLine("Livros:");

            if (!autor.Books.Any())
            {
                Console.WriteLine("  (Sem livros cadastrados)");
            }
            else
            {
                foreach (var l in autor.Books.OrderBy(b => b.PublishDate))
                    Console.WriteLine($"  - ID {l.BookId}: '{l.Title}' ({l.PublishDate.Year}) — {l.Price:C}");
            }

            Console.WriteLine("--------------------------------");
        }

        private static void AtualizarSobrenomeAutor(int authorId, string novoSobrenome)
        {
            using var context = new BibliotecaContext();
            var autor = context.Authors.Find(authorId);

            if (autor == null)
            {
                Console.WriteLine($"❌ Autor com ID {authorId} não encontrado.");
                return;
            }

            string sobrenomeAntigo = autor.LastName;
            autor.LastName = novoSobrenome;

            int registros = context.SaveChanges();

            Console.WriteLine(registros > 0
                ? $"✅ Sobrenome atualizado: {sobrenomeAntigo} → {autor.LastName}"
                : "⚠️ Nenhuma alteração realizada (o sobrenome pode ser igual ao anterior).");
        }

        private static void ExcluirAutor(int authorId)
        {
            using var context = new BibliotecaContext();
            var autor = context.Authors
                .Include(a => a.Books)
                .FirstOrDefault(a => a.AuthorId == authorId);

            if (autor == null)
            {
                Console.WriteLine($"❌ Autor com ID {authorId} não encontrado.");
                return;
            }

            if (autor.Books.Any())
            {
                Console.WriteLine($"⚠️ O autor possui {autor.Books.Count} livro(s). Exclua-os primeiro.");
                return;
            }

            context.Authors.Remove(autor);
            int registros = context.SaveChanges();

            Console.WriteLine(registros > 0
                ? $"✅ Autor '{autor.FirstName} {autor.LastName}' excluído com sucesso."
                : "⚠️ Nenhum autor foi excluído.");
        }

        #endregion

        #region Utilitários

        private static bool LerInt(string mensagem, out int valor)
        {
            Console.Write(mensagem);
            if (int.TryParse(Console.ReadLine(), out valor))
                return true;

            Console.WriteLine("❌ Valor inválido. Digite um número inteiro.");
            return false;
        }

        private static bool LerDecimal(string mensagem, out decimal valor)
        {
            Console.Write(mensagem);
            if (decimal.TryParse(Console.ReadLine(), out valor))
                return true;

            Console.WriteLine("❌ Valor inválido. Digite um número decimal.");
            return false;
        }

        #endregion
    }
}

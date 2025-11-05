using Biblioteca.Data;
using Biblioteca.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

ExecutarMenu(); 

static void ExecutarMenu()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("=== Menu Biblioteca EF Core ===");
        Console.WriteLine("--- Módulos 2, 4, 6 ---");
        Console.WriteLine("1. Adicionar Novo Autor");
        Console.WriteLine("2. Adicionar Livro para Autor Existente (Usando FK)");
        Console.WriteLine("3. Atualizar Sobrenome do Autor (Conectado)");
        Console.WriteLine("4. Excluir Autor (Verifica Relação 1:*)");
        Console.WriteLine("--- Módulo 8: Dados Relacionados ---");
        Console.WriteLine("5. Adicionar Autor com Livro (Grafo Completo)"); 
        Console.WriteLine("6. Listar Autores (Eager Loading com .Include)"); 
        Console.WriteLine("7. Listar Contagem de Livros por Autor (Projeção)"); 
        Console.WriteLine("8. Carregar Livros Explicita/Tardiamente (Explicit Loading)"); 
        Console.WriteLine("9. Listar Autores que Têm Livros (Filtro)"); 
        Console.WriteLine("0. Sair");
        Console.WriteLine("==============================");
        Console.Write("Escolha uma opção: ");

        string? escolha = Console.ReadLine();

        switch (escolha)
        {
            case "1":
                PromptAdicionarAutor();
                break;
            case "2":
                PromptAdicionarLivroParaAutor();
                break;
            case "3":
                PromptAtualizarSobrenomeAutor();
                break;
            case "4":
                PromptExcluirAutor();
                break;
            case "5": 
                PromptAdicionarAutorComLivro();
                break;
            case "6":
                PromptConsultarAutorComLivros();
                break;
            case "7": 
                ListarContagemDeLivrosPorAutor();
                break;
            case "8": 
                PromptCarregarLivrosExplicitaTardiamente();
                break;
            case "9": 
                ListarAutoresComLivros();
                break;
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

static void PromptAdicionarAutor()
{
    Console.Write("Digite o Primeiro Nome do Autor: ");
    string? nome = Console.ReadLine();
    Console.Write("Digite o Sobrenome do Autor: ");
    string? sobrenome = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(nome) && !string.IsNullOrWhiteSpace(sobrenome))
    {
        AdicionarAutor(nome, sobrenome);
    }
    else { Console.WriteLine("Nome e Sobrenome não podem ser vazios."); }
}

static void PromptAdicionarLivroParaAutor()
{
    Console.Write("Digite o ID do Autor para adicionar o livro: ");
    if (int.TryParse(Console.ReadLine(), out int autorIdLivro))
    {
        Console.Write("Digite o Título do Livro: ");
        string? tituloLivro = Console.ReadLine();
        Console.Write("Digite o Ano de Publicação: ");
        if (int.TryParse(Console.ReadLine(), out int anoLivro))
        {
            Console.Write("Digite o Preço do Livro (ex: 29,90): ");
            if (decimal.TryParse(Console.ReadLine(), out decimal precoLivro))
            {
                if (!string.IsNullOrWhiteSpace(tituloLivro))
                {
                    AdicionarLivroParaAutor(autorIdLivro, tituloLivro, anoLivro, precoLivro);
                }
                else { Console.WriteLine("Título não pode ser vazio."); }
            }
            else { Console.WriteLine("Preço inválido."); }
        }
        else { Console.WriteLine("Ano inválido."); }
    }
    else { Console.WriteLine("ID do Autor inválido."); }
}

static void PromptAtualizarSobrenomeAutor()
{
    Console.Write("Digite o ID do Autor para atualizar o sobrenome: ");
    if (int.TryParse(Console.ReadLine(), out int autorIdAtualizar))
    {
        Console.Write("Digite o NOVO Sobrenome: ");
        string? novoSobrenome = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(novoSobrenome))
        {
            AtualizarSobrenomeAutor(autorIdAtualizar, novoSobrenome);
        }
        else { Console.WriteLine("Novo sobrenome não pode ser vazio."); }
    }
    else { Console.WriteLine("ID do Autor inválido."); }
}

static void PromptExcluirAutor()
{
    Console.Write("Digite o ID do Autor para excluir: ");
    if (int.TryParse(Console.ReadLine(), out int autorIdExcluir))
    {
        ExcluirAutor(autorIdExcluir);
    }
    else { Console.WriteLine("ID do Autor inválido."); }
}

static void PromptConsultarAutorComLivros()
{
    Console.Write("Digite o ID do Autor para ver os detalhes: ");
    if (int.TryParse(Console.ReadLine(), out int autorIdDetalhes))
    {
        ConsultarAutorComLivros(autorIdDetalhes);
    }
    else { Console.WriteLine("ID do Autor inválido."); }
}

static void PromptAdicionarAutorComLivro()
{
    Console.Write("Digite o Primeiro Nome do NOVO Autor: ");
    string? nome = Console.ReadLine();
    Console.Write("Digite o Sobrenome do NOVO Autor: ");
    string? sobrenome = Console.ReadLine();
    Console.Write("Digite o Título do PRIMEIRO Livro deste Autor: ");
    string? tituloLivro = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(sobrenome) || string.IsNullOrWhiteSpace(tituloLivro))
    {
        Console.WriteLine("Todos os campos são obrigatórios.");
        return;
    }

    AdicionarAutorComLivro(nome, sobrenome, tituloLivro);
}

static void PromptCarregarLivrosExplicitaTardiamente()
{
    Console.Write("Digite o ID do Autor para carregar livros explicitamente: ");
    if (int.TryParse(Console.ReadLine(), out int autorId))
    {
        CarregarLivrosExplicitaTardiamente(autorId);
    }
    else { Console.WriteLine("ID do Autor inválido."); }
}


static void AdicionarAutor(string primeiroNome, string ultimoNome)
{
    var autor = new Author { FirstName = primeiroNome, LastName = ultimoNome };
    using var context = new BibliotecaContext();
    context.Authors.Add(autor);
    context.SaveChanges();
    Console.WriteLine($"Autor '{autor.FirstName} {autor.LastName}' adicionado com ID: {autor.AuthorId}");
}

static void AdicionarLivroParaAutor(int authorId, string titulo, int anoPublicacao, decimal preco)
{
    var livro = new Book { Title = titulo, PublishDate = new DateTime(anoPublicacao, 1, 1), Price = preco, AuthorId = authorId };
    using var context = new BibliotecaContext();
    var autorExiste = context.Authors.Any(a => a.AuthorId == authorId);
    if (!autorExiste) { Console.WriteLine($"**ERRO:** Autor com ID {authorId} não encontrado."); return; }

    context.Books.Add(livro);
    context.SaveChanges();
    Console.WriteLine($"Livro '{livro.Title}' (ID: {livro.BookId}) adicionado para o autor ID {authorId}.");
}

static void AtualizarSobrenomeAutor(int authorId, string novoSobrenome)
{
    using var context = new BibliotecaContext();
    var autor = context.Authors.Find(authorId);
    if (autor != null)
    {
        autor.LastName = novoSobrenome;
        context.SaveChanges();
        Console.WriteLine($"Autor ID {authorId} atualizado para: {autor.FirstName} {autor.LastName}");
    }
    else { Console.WriteLine($"Autor com ID {authorId} não encontrado."); }
}

static void ExcluirAutor(int authorId)
{
    using var context = new BibliotecaContext();
    var autor = context.Authors.Include(a => a.Books).FirstOrDefault(a => a.AuthorId == authorId);
    if (autor != null)
    {
        if (autor.Books.Any())
        {
            Console.WriteLine($"**ERRO:** Não é possível excluir o autor ID {authorId} pois ele possui {autor.Books.Count} livro(s) associado(s).");
            return;
        }
        context.Authors.Remove(autor);
        context.SaveChanges();
        Console.WriteLine("Autor excluído com sucesso.");
    }
    else { Console.WriteLine($"Autor com ID {authorId} não encontrado."); }
}

static void AdicionarAutorComLivro(string nomeAutor, string sobrenomeAutor, string tituloLivro)
{
    var autor = new Author { FirstName = nomeAutor, LastName = sobrenomeAutor };
    var livro = new Book { Title = tituloLivro, PublishDate = DateTime.Now, Price = 0m }; 

    autor.Books.Add(livro);

    using var context = new BibliotecaContext();
    context.Authors.Add(autor);
    context.SaveChanges();

    Console.WriteLine($"Autor '{autor.FirstName}' (ID: {autor.AuthorId}) e Livro '{livro.Title}' (ID: {livro.BookId}) adicionados.");
}

static void ConsultarAutorComLivros(int authorId)
{
    using var context = new BibliotecaContext();
    Console.WriteLine($"\n--- DETALHES DO AUTOR ID {authorId} (Eager Loading) ---");
    var autor = context.Authors
                       .Include(a => a.Books) 
                       .AsNoTracking()
                       .FirstOrDefault(a => a.AuthorId == authorId);

    if (autor != null)
    {
        Console.WriteLine($"Autor: {autor.FirstName} {autor.LastName}");
        if (autor.Books.Any())
        {
            foreach (var l in autor.Books)
            {
                Console.WriteLine($"  - Livro ID {l.BookId}: '{l.Title}'");
            }
        }
        else { Console.WriteLine("  (Sem livros cadastrados)"); }
    }
    else { Console.WriteLine($"Autor com ID {authorId} não encontrado."); }
}

static void ListarContagemDeLivrosPorAutor()
{
    using var context = new BibliotecaContext();
    Console.WriteLine("\n--- CONTAGEM DE LIVROS POR AUTOR (Projeção) ---");

    var contagemAutores = context.Authors
                                .AsNoTracking()
                                .Select(a => new { 
                                    Nome = a.FirstName + " " + a.LastName,
                                    Contagem = a.Books.Count()
                                })
                                .ToList();


    foreach (var item in contagemAutores)
    {
        Console.WriteLine($"{item.Nome}: {item.Contagem} livro(s)");
    }
}

static void CarregarLivrosExplicitaTardiamente(int authorId)
{
    using var context = new BibliotecaContext();
    Console.WriteLine($"\n--- CARREGAMENTO EXPLÍCITO (Autor ID: {authorId}) ---");

    var autor = context.Authors.Find(authorId);
    if (autor == null) { Console.WriteLine("Autor não encontrado."); return; }

    Console.WriteLine($"Autor encontrado: {autor.FirstName} {autor.LastName}");
    Console.WriteLine($"autor.Books está carregado? R: {context.Entry(autor).Collection(a => a.Books).IsLoaded}");
   
    context.Entry(autor).Collection(a => a.Books).Load(); 

    Console.WriteLine($"autor.Books está carregado? R: {context.Entry(autor).Collection(a => a.Books).IsLoaded}"); 

    if (autor.Books.Any())
    {
        Console.WriteLine("Livros carregados:");
        foreach (var livro in autor.Books)
        {
            Console.WriteLine($"  - {livro.Title}");
        }
    }
    else
    {
        Console.WriteLine("Autor não possui livros.");
    }
}

static void ListarAutoresComLivros()
{
    using var context = new BibliotecaContext();
    Console.WriteLine("\n--- AUTORES QUE POSSUEM LIVROS (Filtro) ---");

    var autores = context.Authors
                         .Where(a => a.Books.Any())
                         .AsNoTracking()
                         .ToList();

    foreach (var autor in autores)
    {
        Console.WriteLine($"ID {autor.AuthorId}: {autor.FirstName} {autor.LastName}");
    }
}

static void ConsultarAutores()
{
    using var context = new BibliotecaContext();
    Console.WriteLine("\n--- LISTA DE AUTORES ---");
    var autores = context.Authors.AsNoTracking().OrderBy(a => a.LastName).ThenBy(a => a.FirstName).ToList();
    if (!autores.Any()) { Console.WriteLine("Nenhum autor encontrado."); return; }
    foreach (var a in autores)
    {
        Console.WriteLine($"ID {a.AuthorId}: {a.FirstName} {a.LastName}");
    }
    Console.WriteLine("-------------------------");
}
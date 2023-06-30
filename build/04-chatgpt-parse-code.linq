<Query Kind="Statements">
  <NuGetReference>Microsoft.CodeAnalysis.CSharp</NuGetReference>
  <NuGetReference>Microsoft.CodeAnalysis.CSharp.Workspaces</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore.Sqlite</NuGetReference>
  <NuGetReference>OpenAI</NuGetReference>
  <Namespace>Microsoft.CodeAnalysis</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp</Namespace>
  <Namespace>Microsoft.CodeAnalysis.CSharp.Syntax</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Formatting</Namespace>
  <Namespace>Microsoft.CodeAnalysis.Text</Namespace>
  <Namespace>OpenAI_API</Namespace>
  <Namespace>OpenAI_API.Chat</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

#nullable enable

#load "work\opensource\sdcb.arithmetic\chatgpt-prompt-cache"

string solutionRoot = GetParentDirectoryUntilContainsFile(new DirectoryInfo(Util.CurrentQueryPath), "PaddleSharp.sln").ToString();
string file = Path.Combine(solutionRoot, @"src/Sdcb.PaddleDetection/PaddleDetector.cs");
string code = File.ReadAllText(file);

SyntaxTree tree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions().WithDocumentationMode(DocumentationMode.Parse));
CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

ClassDeclarationSyntax theClass = root.DescendantNodes()
	.OfType<ClassDeclarationSyntax>()
	.Single();

(await MethodReplacer.ReplaceMethods(theClass, QueryCancelToken)).ToFullString().Dump();


static DirectoryInfo GetParentDirectoryUntilContainsFile(DirectoryInfo? directory, string fileName)
{
	while (directory != null && !File.Exists(Path.Combine(directory.FullName, fileName)))
	{
		directory = directory.Parent;
	}

	if (directory == null)
	{
		throw new InvalidOperationException($"Could not find parent directory containing file '{fileName}'.");
	}

	return directory;
}

class MethodRemover : CSharpSyntaxRewriter
{
	public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
	{
		// By not calling the base implementation and returning null, we remove the node from the tree.
		return null;
	}

	public override SyntaxNode? VisitOperatorDeclaration(OperatorDeclarationSyntax node)
	{
		// By not calling the base implementation and returning null, we remove the node from the tree.
		return null;
	}

	public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node)
	{
		return null;
	}

	public override SyntaxNode? VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
	{
		// By not calling the base implementation and returning null, we remove the node from the tree.
		return null;
	}

	public static ClassDeclarationSyntax RemoveMethodsAndOperators(ClassDeclarationSyntax classDeclaration)
	{
		var rewriter = new MethodRemover();
		return (ClassDeclarationSyntax)rewriter.Visit(classDeclaration);
	}
}

class CommentRemover : CSharpSyntaxRewriter
{
	public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
	{
		// If the trivia is a comment, we remove it by not returning it.
		if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
			trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
			trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
			trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
		{
			return default;
		}

		// Otherwise, we visit the trivia normally.
		return base.VisitTrivia(trivia);
	}

	public static ClassDeclarationSyntax RemoveComments(ClassDeclarationSyntax classDeclaration)
	{
		var commentRemover = new CommentRemover();
		return (ClassDeclarationSyntax)commentRemover.Visit(classDeclaration);
	}
}

class MethodReplacer : CSharpSyntaxRewriter
{
	private string _dummyClassCode;
	private readonly ChatGPTAsker _gpt;

	public MethodReplacer(ClassDeclarationSyntax classSyntax, ChatGPTAsker gpt)
	{
		ClassDeclarationSyntax dummyClass = (ClassDeclarationSyntax)Formatter.Format(
			CommentRemover.RemoveComments(
			MethodRemover.RemoveMethodsAndOperators(classSyntax)), new AdhocWorkspace());
		_dummyClassCode = dummyClass.ToFullString();
		_gpt = gpt;
	}

	public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node) => TransformNode(node);
	public override SyntaxNode? VisitConstructorDeclaration(ConstructorDeclarationSyntax node) => TransformNode(node);
	public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node) => TransformNode(node);
	public override SyntaxNode? VisitOperatorDeclaration(OperatorDeclarationSyntax node) => TransformNode(node);
	public override SyntaxNode? VisitDestructorDeclaration(DestructorDeclarationSyntax node) => TransformNode(node);
	public override SyntaxNode? VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node) => TransformNode(node);

	SyntaxNode TransformNode(MemberDeclarationSyntax node)
	{
		if (node is DestructorDeclarationSyntax || node.Modifiers.Any(x => x.Text == "public" || x.Text == "protected"))
		{
			// Get the full text of the method declaration, including XML comments.
			string comment = node.GetLeadingTrivia().ToFullString();
			string body = node.ToString();

			// Get the new method code from GetReplacementForFunction.
			string newComment = GetReplacementForFunction(comment, body);

			// Parse the new method code and return
			return SyntaxFactory.ParseMemberDeclaration("\n" + newComment + "\n" + body + "\n")!;
		}
		return node;
	}

	private string GetReplacementForFunction(string comment, string functionCode)
	{
		var prompts = new List<ChatMessage>
		{
			new ChatMessage { Role = ChatMessageRole.System, Content = $"""
				You're a helper C# code optimization robot, please check user prompt carefully and response code only, nothing else.
				User will provide a C# function, you need to add/optimize xml comment to the function, and returns xml comment only, function body no need, Here is some C# code context to help you understand:
				{_dummyClassCode}
				""" },
			new ChatMessage { Role = ChatMessageRole.User, Content = """
				/// <summary>TODO</summary>
				public unsafe static GmpFloat From(int val)
				{
				    Mpf_t raw = new();
				    Mpf_t* ptr = &raw;
				    GmpLib.__gmpf_init_set_si((IntPtr)ptr, val);
				    return new GmpFloat(raw);
				}
				""" },
			new ChatMessage { Role = ChatMessageRole.Assistant, Content = """
				/// <summary>
				/// Create a <see cref="GmpFloat"/> instance from a integer <paramref name="val"/>, precision default to <see cref="DefaultPrecision"/> in bit.
				/// </summary>
				/// <param name="val">The integer value to convert.</param>
				/// <returns>A new instance of <see cref="GmpFloat"/> representing the converted value.</returns>
				""" },
			new ChatMessage { Role = ChatMessageRole.User, Content = functionCode }
		};
		string raw = _gpt(new ChatgptRequest(prompts, ChatgptModels._4, 0.5));
		return string.Join("\n", raw.Split("\n").Where(x => x.StartsWith("///")));
	}

	public static async Task<ClassDeclarationSyntax> ReplaceMethods(ClassDeclarationSyntax classDeclaration, CancellationToken cancellationToken = default)
	{
		string dbDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "chatgpt-cache");
		Directory.CreateDirectory(dbDir);
		using (ChatgptCacheDb db = new(Path.Combine(dbDir, "chatgpt-cache.db")))
		{
			db.Database.EnsureCreated();
		}
		
		List<ChatgptRequest> requests = new List<UserQuery.ChatgptRequest>();
		var getter = new MethodReplacer(classDeclaration, req =>
		{
			requests.Add(req);
			return "";
		});
		getter.Visit(classDeclaration);

		// build cache
		var dc = new DumpContainer().Dump("progress");
		int completed = 0;
		await Parallel.ForEachAsync(requests, cancellationToken, async (req, ct) =>
		{
			using (ChatgptCacheDb db = new(Path.Combine(dbDir, "chatgpt-cache.db")))
			{
				await db.AskChatgptOrFromCache(req, ct);
			}

			Interlocked.Increment(ref completed);
			dc.Content = $"{completed} / {requests.Count}";
		});

		using (ChatgptCacheDb db = new(Path.Combine(dbDir, "chatgpt-cache.db")))
		{
			var rewritter = new MethodReplacer(classDeclaration, req => db.AskChatgptOrFromCache(req).GetAwaiter().GetResult());
			return (ClassDeclarationSyntax)rewritter.Visit(classDeclaration);
		}
	}
}

delegate string ChatGPTAsker(ChatgptRequest req);
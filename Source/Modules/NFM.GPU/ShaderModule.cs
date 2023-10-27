using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using SharpGen.Runtime;
using Vortice.Dxc;

namespace NFM.GPU;

public enum ShaderStage : uint
{
	Compute = DxcShaderStage.Compute,
	Pixel = DxcShaderStage.Pixel,
	Vertex = DxcShaderStage.Vertex,
	Mesh = DxcShaderStage.Mesh,
	Library = DxcShaderStage.Library,
}

public class ShaderModule
{
	static readonly DxcCompilerOptions compilerOptions = new()
	{
		ShaderModel = DxcShaderModel.Model6_6,
		HLSLVersion = 2021,
		SkipOptimizations = Debug.IsDebugMode,
		AllResourcesBound = true,
	};

	public ShaderStage Stage { get; }
	public IDxcBlob Bytecode { get; }

	readonly IDxcLinker linker = Dxc.CreateDxcLinker();

	public ShaderModule(string source) : this(source, ShaderStage.Library, GetIncludes(Assembly.GetCallingAssembly())) {}

	public ShaderModule(string source, ShaderStage stage, string entry = "main") : this(source, entry, (DxcShaderStage)stage, GetIncludes(Assembly.GetCallingAssembly())) {}
	public ShaderModule(string source, ShaderStage stage, CustomIncludeHandler? handler, string entry = "main") : this(source, entry, (DxcShaderStage)stage, handler) {}

	private ShaderModule(string source, string entry, DxcShaderStage stage, CustomIncludeHandler? handler)
	{
		Stage = (ShaderStage)stage;
		IDxcResult moduleResult = DxcCompiler.Compile(stage, source, entry, compilerOptions, includeHandler: handler);

		// Check for errors.
		if (moduleResult.GetStatus().Success)
		{
			Bytecode = moduleResult.GetOutput(DxcOutKind.Object);
		}
		else
		{
			throw new Exception($"Shader module failed to compile with message: \n{moduleResult.GetErrors() }");
		}
	}

	private ShaderModule(IDxcBlob bytecode, DxcShaderStage stage)
	{
		Stage = (ShaderStage)stage;
		Bytecode = bytecode;
	}

	~ShaderModule()
	{
		linker.Release();
		Bytecode.Release();
	}

	public ShaderModule Link(string entryName, ShaderStage stage, params ShaderModule[] others)
	{
		var libNames = new string[others.Length + 1];
		linker.RegisterLibrary("this", Bytecode);
		libNames[0] = "this";

		for (int i = 0; i < others.Length; i++)
		{
			linker.RegisterLibrary($"other{i}", others[i].Bytecode);
			libNames[i + 1] = $"other{i}";
		}

		var result = linker.Link(entryName, DxcCompiler.GetShaderProfile((DxcShaderStage)stage, DxcShaderModel.Model6_6), libNames, new string[] { });
		
		// Check for errors.
		if (result.GetStatus().Success)
		{
			return new ShaderModule(result.GetResult(), (DxcShaderStage)stage);
		}
		else
		{
			var errorBuffer = result.GetErrorBuffer();
			var errorString = Marshal.PtrToStringAnsi(errorBuffer.BufferPointer, errorBuffer.BufferSize);

			throw new Exception($"Shader modules failed to link with message: {errorString}");
		}
	}

	static string SimplifyPath(string path)
	{
		Regex simplifyRegex = new Regex(@"[^\\/]+(?<!\.\.)[\\/]\.\.[\\/]");

		while (true)
		{
			string newPath = simplifyRegex.Replace(path, "" );
			if (newPath == path)
			{
				break;
			}
			else
			{
				path = newPath;
			}
		}

		return path.Replace("./", "");
	}

	static CustomIncludeHandler? GetIncludes(Assembly? embedSource)
	{
        if (embedSource is null)
        {
            return null;
        }

		Func<string, string?> includeResolver = (path) =>
		{
			path = SimplifyPath(path);
			path = $"{embedSource.GetName().Name}.{path.Replace('/', '.')}";

			using Stream? stream = embedSource.GetManifestResourceStream(path);
			if (stream is not null)
			{
				using StreamReader reader = new(stream);
				return reader.ReadToEnd();
			}

			Debug.LogError($"Failed to resolve include \"{path}\"");
			return null;
		};

		return new CustomIncludeHandler(includeResolver);
	}
}


public sealed class CustomIncludeHandler : CallbackBase, IDxcIncludeHandler
{
	private Func<string, string?> resolveMethod;

	public CustomIncludeHandler(Func<string, string?> resolver) => resolveMethod = resolver;

	public Result LoadSource(string filename, out IDxcBlob? includeSource)
	{
		string? source = resolveMethod.Invoke(filename);

		if (source is not null)
		{
			includeSource = PipelineState.CreateBlob(Encoding.ASCII.GetBytes(source));
			return Result.Ok;
		}
		else
		{
			includeSource = null;
			return Result.Fail;
		}
	}
}
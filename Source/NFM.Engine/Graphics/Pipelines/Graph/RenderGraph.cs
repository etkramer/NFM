using NFM.GPU;
using Vortice.DXGI;

namespace NFM.Graphics;

readonly record struct TextureDesc(Vector2i Size, Format Format, Format DSFormat = default, Format SRFormat = default);
readonly record struct BufferDesc(nint SizeBytes, int Stride);

readonly struct TextureHandle
{
	internal int Index { get; }
	internal TextureHandle(int index) => Index = index;
}

readonly struct BufferHandle
{
	internal int Index { get; }
	internal BufferHandle(int index) => Index = index;
}

/// <summary>
/// Owns a render pipeline's resources and the passes that read and write them. Passes declare their
/// usage up front, so the graph can check every read is produced by an earlier pass before the first frame.
/// </summary>
class RenderGraph : IDisposable
{
	private readonly List<string> names = [];
	private readonly List<Func<Resource>> factories = [];
	private Resource[] resources = [];

	private readonly List<ViewPass> passes = [];
	private readonly List<(HashSet<int> Reads, HashSet<int> Writes)> usage = [];

	private bool isBuilt = false;

	public IReadOnlyList<ViewPass> Passes => passes;

	public TextureHandle CreateTexture(string name, TextureDesc desc)
	{
		return new TextureHandle(Declare(name, () => new Texture(desc.Size.X, desc.Size.Y, 1, desc.Format, dsFormat: desc.DSFormat, srFormat: desc.SRFormat)
		{
			Name = name
		}));
	}

	public BufferHandle CreateBuffer(string name, BufferDesc desc)
	{
		return new BufferHandle(Declare(name, () => new RawBuffer(desc.SizeBytes, desc.Stride) { Name = name }));
	}

	private int Declare(string name, Func<Resource> factory)
	{
		Guard.Require(!isBuilt, "Cannot declare resources once the graph is built.");

		names.Add(name);
		factories.Add(factory);

		return factories.Count - 1;
	}

	public void AddPass(ViewPass pass)
	{
		Guard.Require(!isBuilt, "Cannot add passes once the graph is built.");
		passes.Add(pass);
	}

	public Texture Get(TextureHandle handle) => (Texture)resources[handle.Index];
	public RawBuffer Get(BufferHandle handle) => (RawBuffer)resources[handle.Index];

	public string GetName(TextureHandle handle) => names[handle.Index];

	/// <summary>
	/// Realizes every declared resource, sets up each pass, and validates the resulting graph.
	/// </summary>
	public void Build()
	{
		Guard.Require(!isBuilt, "Graph is already built.");
		isBuilt = true;

		resources = new Resource[factories.Count];
		for (int i = 0; i < factories.Count; i++)
		{
			resources[i] = factories[i]();
		}

		factories.Clear();

		foreach (var pass in passes)
		{
			RenderGraphBuilder builder = new();
			pass.Setup(builder);

			usage.Add((builder.Reads, builder.Writes));
			pass.Init(this);
		}

		Validate();
	}

	private void Validate()
	{
		HashSet<int> written = [];

		for (int i = 0; i < passes.Count; i++)
		{
			foreach (int read in usage[i].Reads)
			{
				Guard.Require(written.Contains(read), $"{passes[i].GetType().Name} reads {names[read]}, which no earlier pass writes.");
			}

			written.UnionWith(usage[i].Writes);
		}
	}

	/// <summary>
	/// Renders the graph as text, for logging or inspection.
	/// </summary>
	public string Describe()
	{
		string result = "";

		for (int i = 0; i < passes.Count; i++)
		{
			string reads = string.Join(", ", usage[i].Reads.Select(o => names[o]));
			string writes = string.Join(", ", usage[i].Writes.Select(o => names[o]));

			result += $"{passes[i].GetType().Name}\n";
			result += $"  reads: {(reads.Length == 0 ? "-" : reads)}\n";
			result += $"  writes: {(writes.Length == 0 ? "-" : writes)}\n";
		}

		return result;
	}

	public void Dispose()
	{
		foreach (var pass in passes)
		{
			pass.Dispose();
		}

		foreach (var resource in resources)
		{
			(resource as IDisposable)?.Dispose();
		}
	}
}

class RenderGraphBuilder
{
	internal HashSet<int> Reads { get; } = [];
	internal HashSet<int> Writes { get; } = [];

	public void Read(params TextureHandle[] handles) => handles.ForEach(o => Reads.Add(o.Index));
	public void Read(params BufferHandle[] handles) => handles.ForEach(o => Reads.Add(o.Index));

	public void Write(params TextureHandle[] handles) => handles.ForEach(o => Writes.Add(o.Index));
	public void Write(params BufferHandle[] handles) => handles.ForEach(o => Writes.Add(o.Index));
}

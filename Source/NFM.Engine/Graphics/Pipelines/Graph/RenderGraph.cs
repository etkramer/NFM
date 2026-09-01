using NFM.GPU;
using Vortice.DXGI;

namespace NFM.Graphics;

readonly record struct TextureDesc(Vector2i Size, Format Format, Format DSFormat = default, Format SRFormat = default);

readonly struct TextureHandle
{
	internal int Index { get; }
	internal TextureHandle(int index) => Index = index;
}

/// <summary>
/// Owns a render pipeline's textures and the passes that read and write them. Passes declare their
/// usage up front, so the graph can check every read is produced by an earlier pass before the first frame.
/// </summary>
class RenderGraph : IDisposable
{
	private readonly List<string> names = [];
	private readonly List<TextureDesc> descs = [];
	private Texture[] textures = [];

	private readonly List<ViewPass> passes = [];
	private readonly List<(HashSet<int> Reads, HashSet<int> Writes)> usage = [];

	private bool isBuilt = false;

	public IReadOnlyList<ViewPass> Passes => passes;

	public TextureHandle CreateTexture(string name, TextureDesc desc)
	{
		Guard.Require(!isBuilt, "Cannot declare textures once the graph is built.");

		names.Add(name);
		descs.Add(desc);

		return new TextureHandle(descs.Count - 1);
	}

	public void AddPass(ViewPass pass)
	{
		Guard.Require(!isBuilt, "Cannot add passes once the graph is built.");
		passes.Add(pass);
	}

	public Texture Get(TextureHandle handle) => textures[handle.Index];

	public string GetName(TextureHandle handle) => names[handle.Index];

	/// <summary>
	/// Realizes every declared texture, sets up each pass, and validates the resulting graph.
	/// </summary>
	public void Build()
	{
		Guard.Require(!isBuilt, "Graph is already built.");
		isBuilt = true;

		textures = new Texture[descs.Count];
		for (int i = 0; i < descs.Count; i++)
		{
			var desc = descs[i];

			textures[i] = new Texture(desc.Size.X, desc.Size.Y, 1, desc.Format, dsFormat: desc.DSFormat, srFormat: desc.SRFormat)
			{
				Name = names[i]
			};
		}

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

		foreach (var texture in textures)
		{
			texture.Dispose();
		}
	}
}

class RenderGraphBuilder
{
	internal HashSet<int> Reads { get; } = [];
	internal HashSet<int> Writes { get; } = [];

	public void Read(params TextureHandle[] handles) => handles.ForEach(o => Reads.Add(o.Index));
	public void Write(params TextureHandle[] handles) => handles.ForEach(o => Writes.Add(o.Index));
}

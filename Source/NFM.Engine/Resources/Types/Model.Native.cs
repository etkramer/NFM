using System;

namespace NFM.Resources;

static unsafe class MikkTSpace
{
	[DllImport("mikktspace.dll")]
	static extern int genTangSpaceDefault(void* pContext);

	[DllImport("mikktspace.dll")]
	static extern int genTangSpace(void* pContext, float fAngularThreshold);

	delegate int getNumFaces(SMikkTSpaceContext* pContext);
	delegate int getNumVerticesOfFace(SMikkTSpaceContext* pContext, int iFace);
	delegate void getPosition(SMikkTSpaceContext* pContext, float* fvPosOut, int iFace, int iVert);
	delegate void getNormal(SMikkTSpaceContext* pContext, float* fvNormOut, int iFace, int iVert);
	delegate void getTexCoord(SMikkTSpaceContext* pContext, float* fvTexcOut, int iFace, int iVert);
	delegate void setTSpaceBasic(SMikkTSpaceContext* pContext, float* fvTangent, float fSign, int iFace, int iVert);
	delegate void setTSpace(SMikkTSpaceContext* pContext, float* fvTangent, float* fvBiTangent, float fMagS, float fMagT, int bIsOrientationPreserving, int iFace, int iVert);

	struct SMikkTSpaceContext
	{
		public SMikkTSpaceInterface* m_pInterface;
		public void* m_pUserData;
	};

	struct SMikkTSpaceInterface
	{
		public nint m_getNumFaces;
		public nint m_getNumVerticesOfFace;

		public nint m_getPosition;
		public nint m_getNormal;
		public nint m_getTexCoord;

		public nint m_setTSpaceBasic;
		public nint m_setTSpace;
	}

	public static void GenTangents(Mesh mesh)
	{
		getNumFaces getNumFacesImpl = (pContext) =>
		{
			return mesh.Indices.Length / 3;
		};

		getNumVerticesOfFace getNumVerticesOfFaceImpl = (pContext, iFace) =>
		{
			return 3;
		};

		getPosition getPositionImpl = (pContext, fvPosOut, iFace, iVert) =>
		{
			var vert = mesh.Vertices[mesh.Indices[(iFace * 3) + iVert]];
			*(Vector3*)fvPosOut = vert.Position;
		};

		getNormal getNormalImpl = (pContext, fvNormOut, iFace, iVert) =>
		{
			var vert = mesh.Vertices[mesh.Indices[(iFace * 3) + iVert]];
			*(Vector3*)fvNormOut = vert.Normal;
		};

		getTexCoord getTexCoordImpl = (pContext, fvTexcOut, iFace, iVert) =>
		{
			var vert = mesh.Vertices[mesh.Indices[(iFace * 3) + iVert]];
			*(Vector2*)fvTexcOut = vert.UV0;
		};

		setTSpaceBasic setTSpaceBasicImpl = (pContext, fvTangent, fSign, iFace, iVert) =>
		{
			uint vertIndex = mesh.Indices[(iFace * 3) + iVert];

			mesh.Vertices[vertIndex].Tangent = new Vector4()
			{
				X = fvTangent[0],
				Y = fvTangent[1],
				Z = fvTangent[2],
				W = fSign,
			};
		};

		SMikkTSpaceInterface contextInterface = new()
		{
			m_getNumFaces = Marshal.GetFunctionPointerForDelegate(getNumFacesImpl),
			m_getNumVerticesOfFace = Marshal.GetFunctionPointerForDelegate(getNumVerticesOfFaceImpl),
			m_getPosition = Marshal.GetFunctionPointerForDelegate(getPositionImpl),
			m_getNormal = Marshal.GetFunctionPointerForDelegate(getNormalImpl),
			m_getTexCoord = Marshal.GetFunctionPointerForDelegate(getTexCoordImpl),
			m_setTSpaceBasic = Marshal.GetFunctionPointerForDelegate(setTSpaceBasicImpl),
		};

		SMikkTSpaceContext context = new SMikkTSpaceContext()
		{
			m_pInterface = &contextInterface,
			m_pUserData = null,
		};

		genTangSpaceDefault(&context);
	}
}
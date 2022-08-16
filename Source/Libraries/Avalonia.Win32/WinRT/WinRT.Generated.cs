#pragma warning disable 108
// ReSharper disable RedundantUsingDirective
// ReSharper disable JoinDeclarationAndInitializer
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedType.Local
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantCast
// ReSharper disable IdentifierTypo
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantUnsafeContext
// ReSharper disable RedundantBaseQualifier
// ReSharper disable EmptyStatement
// ReSharper disable RedundantAttributeParentheses
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Avalonia.MicroCom;

namespace Avalonia.Win32.WinRT
{
    internal enum TrustLevel
    {
        BaseTrust,
        PartialTrust,
        FullTrust
    }

    internal enum DirectXAlphaMode
    {
        Unspecified,
        Premultiplied,
        Straight,
        Ignore
    }

    internal enum DirectXPixelFormat
    {
        Unknown = 0,
        R32G32B32A32Typeless = 1,
        R32G32B32A32Float = 2,
        R32G32B32A32UInt = 3,
        R32G32B32A32Int = 4,
        R32G32B32Typeless = 5,
        R32G32B32Float = 6,
        R32G32B32UInt = 7,
        R32G32B32Int = 8,
        R16G16B16A16Typeless = 9,
        R16G16B16A16Float = 10,
        R16G16B16A16UIntNormalized = 11,
        R16G16B16A16UInt = 12,
        R16G16B16A16IntNormalized = 13,
        R16G16B16A16Int = 14,
        R32G32Typeless = 15,
        R32G32Float = 16,
        R32G32UInt = 17,
        R32G32Int = 18,
        R32G8X24Typeless = 19,
        D32FloatS8X24UInt = 20,
        R32FloatX8X24Typeless = 21,
        X32TypelessG8X24UInt = 22,
        R10G10B10A2Typeless = 23,
        R10G10B10A2UIntNormalized = 24,
        R10G10B10A2UInt = 25,
        R11G11B10Float = 26,
        R8G8B8A8Typeless = 27,
        R8G8B8A8UIntNormalized = 28,
        R8G8B8A8UIntNormalizedSrgb = 29,
        R8G8B8A8UInt = 30,
        R8G8B8A8IntNormalized = 31,
        R8G8B8A8Int = 32,
        R16G16Typeless = 33,
        R16G16Float = 34,
        R16G16UIntNormalized = 35,
        R16G16UInt = 36,
        R16G16IntNormalized = 37,
        R16G16Int = 38,
        R32Typeless = 39,
        D32Float = 40,
        R32Float = 41,
        R32UInt = 42,
        R32Int = 43,
        R24G8Typeless = 44,
        D24UIntNormalizedS8UInt = 45,
        R24UIntNormalizedX8Typeless = 46,
        X24TypelessG8UInt = 47,
        R8G8Typeless = 48,
        R8G8UIntNormalized = 49,
        R8G8UInt = 50,
        R8G8IntNormalized = 51,
        R8G8Int = 52,
        R16Typeless = 53,
        R16Float = 54,
        D16UIntNormalized = 55,
        R16UIntNormalized = 56,
        R16UInt = 57,
        R16IntNormalized = 58,
        R16Int = 59,
        R8Typeless = 60,
        R8UIntNormalized = 61,
        R8UInt = 62,
        R8IntNormalized = 63,
        R8Int = 64,
        A8UIntNormalized = 65,
        R1UIntNormalized = 66,
        R9G9B9E5SharedExponent = 67,
        R8G8B8G8UIntNormalized = 68,
        G8R8G8B8UIntNormalized = 69,
        BC1Typeless = 70,
        BC1UIntNormalized = 71,
        BC1UIntNormalizedSrgb = 72,
        BC2Typeless = 73,
        BC2UIntNormalized = 74,
        BC2UIntNormalizedSrgb = 75,
        BC3Typeless = 76,
        BC3UIntNormalized = 77,
        BC3UIntNormalizedSrgb = 78,
        BC4Typeless = 79,
        BC4UIntNormalized = 80,
        BC4IntNormalized = 81,
        BC5Typeless = 82,
        BC5UIntNormalized = 83,
        BC5IntNormalized = 84,
        B5G6R5UIntNormalized = 85,
        B5G5R5A1UIntNormalized = 86,
        B8G8R8A8UIntNormalized = 87,
        B8G8R8X8UIntNormalized = 88,
        R10G10B10XRBiasA2UIntNormalized = 89,
        B8G8R8A8Typeless = 90,
        B8G8R8A8UIntNormalizedSrgb = 91,
        B8G8R8X8Typeless = 92,
        B8G8R8X8UIntNormalizedSrgb = 93,
        BC6HTypeless = 94,
        BC6H16UnsignedFloat = 95,
        BC6H16Float = 96,
        BC7Typeless = 97,
        BC7UIntNormalized = 98,
        BC7UIntNormalizedSrgb = 99,
        Ayuv = 100,
        Y410 = 101,
        Y416 = 102,
        NV12 = 103,
        P010 = 104,
        P016 = 105,
        Opaque420 = 106,
        Yuy2 = 107,
        Y210 = 108,
        Y216 = 109,
        NV11 = 110,
        AI44 = 111,
        IA44 = 112,
        P8 = 113,
        A8P8 = 114,
        B4G4R4A4UIntNormalized = 115,
        P208 = 130,
        V208 = 131,
        V408 = 132
    }

    internal enum PropertyType
    {
        Empty = 0,
        UInt8 = 1,
        Int16 = 2,
        UInt16 = 3,
        Int32 = 4,
        UInt32 = 5,
        Int64 = 6,
        UInt64 = 7,
        Single = 8,
        Double = 9,
        Char16 = 10,
        Boolean = 11,
        String = 12,
        Inspectable = 13,
        DateTime = 14,
        TimeSpan = 15,
        Guid = 16,
        Point = 17,
        Size = 18,
        Rect = 19,
        OtherType = 20,
        UInt8Array = 1025,
        Int16Array = 1026,
        UInt16Array = 1027,
        Int32Array = 1028,
        UInt32Array = 1029,
        Int64Array = 1030,
        UInt64Array = 1031,
        SingleArray = 1032,
        DoubleArray = 1033,
        Char16Array = 1034,
        BooleanArray = 1035,
        StringArray = 1036,
        InspectableArray = 1037,
        DateTimeArray = 1038,
        TimeSpanArray = 1039,
        GuidArray = 1040,
        PointArray = 1041,
        SizeArray = 1042,
        RectArray = 1043,
        OtherTypeArray = 1044
    }

    internal enum AsyncStatus
    {
        Started = 0,
        Completed,
        Canceled,
        Error
    }

    [System.Flags()]
    internal enum CompositionBatchTypes
    {
        None = 0x0,
        Animation = 0x1,
        Effect = 0x2,
        InfiniteAnimation = 0x4,
        AllAnimations = 0x5
    }

    internal enum CompositionBitmapInterpolationMode
    {
        NearestNeighbor,
        Linear,
        MagLinearMinLinearMipLinear,
        MagLinearMinLinearMipNearest,
        MagLinearMinNearestMipLinear,
        MagLinearMinNearestMipNearest,
        MagNearestMinLinearMipLinear,
        MagNearestMinLinearMipNearest,
        MagNearestMinNearestMipLinear,
        MagNearestMinNearestMipNearest
    }

    internal enum CompositionStretch
    {
        None,
        Fill,
        Uniform,
        UniformToFill
    }

    internal enum CompositionBackfaceVisibility
    {
        Inherit,
        Visible,
        Hidden
    }

    internal enum CompositionBorderMode
    {
        Inherit,
        Soft,
        Hard
    }

    internal enum CompositionCompositeMode
    {
        Inherit,
        SourceOver,
        DestinationInvert,
        MinBlend
    }

    internal enum GRAPHICS_EFFECT_PROPERTY_MAPPING
    {
        GRAPHICS_EFFECT_PROPERTY_MAPPING_UNKNOWN,
        GRAPHICS_EFFECT_PROPERTY_MAPPING_DIRECT,
        GRAPHICS_EFFECT_PROPERTY_MAPPING_VECTORX,
        GRAPHICS_EFFECT_PROPERTY_MAPPING_VECTORY,
        GRAPHICS_EFFECT_PROPERTY_MAPPING_VECTORZ,
        GRAPHICS_EFFECT_PROPERTY_MAPPING_VECTORW,
        GRAPHICS_EFFECT_PROPERTY_MAPPING_RECT_TO_VECTOR4,
        GRAPHICS_EFFECT_PROPERTY_MAPPING_RADIANS_TO_DEGREES,
        GRAPHICS_EFFECT_PROPERTY_MAPPING_COLORMATRIX_ALPHA_MODE,
        GRAPHICS_EFFECT_PROPERTY_MAPPING_COLOR_TO_VECTOR3,
        GRAPHICS_EFFECT_PROPERTY_MAPPING_COLOR_TO_VECTOR4
    }

    internal enum CompositionEffectFactoryLoadStatus
    {
        Success = 0,
        EffectTooComplex = 1,
        Pending = 2,
        Other = -1
    }

    internal unsafe partial interface IInspectable : global::Avalonia.MicroCom.IUnknown
    {
        void GetIids(ulong* iidCount, Guid** iids);
        IntPtr RuntimeClassName { get; }

        TrustLevel TrustLevel { get; }
    }

    internal unsafe partial interface IPropertyValue : IInspectable
    {
        PropertyType Type { get; }

        int IsNumericScalar { get; }

        byte UInt8 { get; }

        short Int16 { get; }

        ushort UInt16 { get; }

        int Int32 { get; }

        uint UInt32 { get; }

        long Int64 { get; }

        ulong UInt64 { get; }

        float Single { get; }

        double Double { get; }

        System.Char Char16 { get; }

        int Boolean { get; }

        IntPtr String { get; }

        System.Guid Guid { get; }

        void GetDateTime(void* value);
        void GetTimeSpan(void* value);
        void GetPoint(void* value);
        void GetSize(void* value);
        void GetRect(void* value);
        byte* GetUInt8Array(uint* __valueSize);
        short* GetInt16Array(uint* __valueSize);
        ushort* GetUInt16Array(uint* __valueSize);
        int* GetInt32Array(uint* __valueSize);
        uint* GetUInt32Array(uint* __valueSize);
        long* GetInt64Array(uint* __valueSize);
        ulong* GetUInt64Array(uint* __valueSize);
        float* GetSingleArray(uint* __valueSize);
        double* GetDoubleArray(uint* __valueSize);
        System.Char* GetChar16Array(uint* __valueSize);
        int* GetBooleanArray(uint* __valueSize);
        IntPtr* GetStringArray(uint* __valueSize);
        void** GetInspectableArray(uint* __valueSize);
        System.Guid* GetGuidArray(uint* __valueSize);
        void* GetDateTimeArray(uint* __valueSize);
        void* GetTimeSpanArray(uint* __valueSize);
        void* GetPointArray(uint* __valueSize);
        void* GetSizeArray(uint* __valueSize);
        void* GetRectArray(uint* __valueSize);
    }

    internal unsafe partial interface IAsyncActionCompletedHandler : global::Avalonia.MicroCom.IUnknown
    {
        void Invoke(IAsyncAction asyncInfo, AsyncStatus asyncStatus);
    }

    internal unsafe partial interface IAsyncAction : IInspectable
    {
        void SetCompleted(IAsyncActionCompletedHandler handler);
        IAsyncActionCompletedHandler Completed { get; }

        void GetResults();
    }

    internal unsafe partial interface IDispatcherQueue : IInspectable
    {
    }

    internal unsafe partial interface IDispatcherQueueController : IInspectable
    {
        IDispatcherQueue DispatcherQueue { get; }

        IAsyncAction ShutdownQueueAsync();
    }

    internal unsafe partial interface IActivationFactory : IInspectable
    {
        IntPtr ActivateInstance();
    }

    internal unsafe partial interface ICompositor : IInspectable
    {
        void* CreateColorKeyFrameAnimation();
        void* CreateColorBrush();
        ICompositionColorBrush CreateColorBrushWithColor(Avalonia.Win32.WinRT.WinRTColor* color);
        IContainerVisual CreateContainerVisual();
        void* CreateCubicBezierEasingFunction(System.Numerics.Vector2 controlPoint1, System.Numerics.Vector2 controlPoint2);
        ICompositionEffectFactory CreateEffectFactory(IGraphicsEffect graphicsEffect);
        void* CreateEffectFactoryWithProperties(void* graphicsEffect, void* animatableProperties);
        void* CreateExpressionAnimation();
        void* CreateExpressionAnimationWithExpression(IntPtr expression);
        void* CreateInsetClip();
        void* CreateInsetClipWithInsets(float leftInset, float topInset, float rightInset, float bottomInset);
        void* CreateLinearEasingFunction();
        void* CreatePropertySet();
        void* CreateQuaternionKeyFrameAnimation();
        void* CreateScalarKeyFrameAnimation();
        ICompositionScopedBatch CreateScopedBatch(CompositionBatchTypes batchType);
        ISpriteVisual CreateSpriteVisual();
        ICompositionSurfaceBrush CreateSurfaceBrush();
        ICompositionSurfaceBrush CreateSurfaceBrushWithSurface(ICompositionSurface surface);
        void* CreateTargetForCurrentView();
        void* CreateVector2KeyFrameAnimation();
        void* CreateVector3KeyFrameAnimation();
        void* CreateVector4KeyFrameAnimation();
        void* GetCommitBatch(CompositionBatchTypes batchType);
    }

    internal unsafe partial interface ICompositor2 : IInspectable
    {
        void* CreateAmbientLight();
        void* CreateAnimationGroup();
        ICompositionBackdropBrush CreateBackdropBrush();
        void* CreateDistantLight();
        void* CreateDropShadow();
        void* CreateImplicitAnimationCollection();
        void* CreateLayerVisual();
        void* CreateMaskBrush();
        void* CreateNineGridBrush();
        void* CreatePointLight();
        void* CreateSpotLight();
        void* CreateStepEasingFunction();
        void* CreateStepEasingFunctionWithStepCount(int stepCount);
    }

    internal unsafe partial interface ICompositor3 : IInspectable
    {
        ICompositionBackdropBrush CreateHostBackdropBrush();
    }

    internal unsafe partial interface ICompositorWithBlurredWallpaperBackdropBrush : IInspectable
    {
        ICompositionBackdropBrush TryCreateBlurredWallpaperBackdropBrush();
    }

    internal unsafe partial interface ISpriteVisual : IInspectable
    {
        ICompositionBrush Brush { get; }

        void SetBrush(ICompositionBrush value);
    }

    internal unsafe partial interface ICompositionDrawingSurfaceInterop : global::Avalonia.MicroCom.IUnknown
    {
        Avalonia.Win32.Interop.UnmanagedMethods.POINT BeginDraw(Avalonia.Win32.Interop.UnmanagedMethods.RECT* updateRect, Guid* iid, void** updateObject);
        void EndDraw();
        void Resize(Avalonia.Win32.Interop.UnmanagedMethods.POINT sizePixels);
        void Scroll(Avalonia.Win32.Interop.UnmanagedMethods.RECT* scrollRect, Avalonia.Win32.Interop.UnmanagedMethods.RECT* clipRect, int offsetX, int offsetY);
        void ResumeDraw();
        void SuspendDraw();
    }

    internal unsafe partial interface ICompositionGraphicsDeviceInterop : global::Avalonia.MicroCom.IUnknown
    {
        IUnknown RenderingDevice { get; }

        void SetRenderingDevice(IUnknown value);
    }

    internal unsafe partial interface ICompositorInterop : global::Avalonia.MicroCom.IUnknown
    {
        ICompositionSurface CreateCompositionSurfaceForHandle(IntPtr swapChain);
        ICompositionSurface CreateCompositionSurfaceForSwapChain(IUnknown swapChain);
        ICompositionGraphicsDevice CreateGraphicsDevice(IUnknown renderingDevice);
    }

    internal unsafe partial interface ISwapChainInterop : global::Avalonia.MicroCom.IUnknown
    {
        void SetSwapChain(IUnknown swapChain);
    }

    internal unsafe partial interface ICompositorDesktopInterop : global::Avalonia.MicroCom.IUnknown
    {
        IDesktopWindowTarget CreateDesktopWindowTarget(IntPtr hwndTarget, int isTopmost);
        void EnsureOnThread(int threadId);
    }

    internal unsafe partial interface IDesktopWindowTargetInterop : global::Avalonia.MicroCom.IUnknown
    {
        IntPtr HWnd { get; }
    }

    internal unsafe partial interface IDesktopWindowContentBridgeInterop : global::Avalonia.MicroCom.IUnknown
    {
        void Initialize(ICompositor compositor, IntPtr parentHwnd);
        IntPtr HWnd { get; }

        float AppliedScaleFactor { get; }
    }

    internal unsafe partial interface ICompositionGraphicsDevice : IInspectable
    {
        ICompositionDrawingSurface CreateDrawingSurface(Avalonia.Win32.Interop.UnmanagedMethods.SIZE sizePixels, DirectXPixelFormat pixelFormat, DirectXAlphaMode alphaMode);
        void AddRenderingDeviceReplaced(void* handler, void* token);
        void RemoveRenderingDeviceReplaced(int token);
    }

    internal unsafe partial interface ICompositionSurface : IInspectable
    {
    }

    internal unsafe partial interface IDesktopWindowTarget : IInspectable
    {
        int IsTopmost { get; }
    }

    internal unsafe partial interface ICompositionDrawingSurface : IInspectable
    {
        DirectXAlphaMode AlphaMode { get; }

        DirectXPixelFormat PixelFormat { get; }

        Avalonia.Win32.Interop.UnmanagedMethods.POINT Size { get; }
    }

    internal unsafe partial interface ICompositionSurfaceBrush : IInspectable
    {
        CompositionBitmapInterpolationMode BitmapInterpolationMode { get; }

        void SetBitmapInterpolationMode(CompositionBitmapInterpolationMode value);
        float HorizontalAlignmentRatio { get; }

        void SetHorizontalAlignmentRatio(float value);
        CompositionStretch Stretch { get; }

        void SetStretch(CompositionStretch value);
        ICompositionSurface Surface { get; }

        void SetSurface(ICompositionSurface value);
        float VerticalAlignmentRatio { get; }

        void SetVerticalAlignmentRatio(float value);
    }

    internal unsafe partial interface ICompositionBrush : IInspectable
    {
    }

    internal unsafe partial interface ICompositionClip : IInspectable
    {
    }

    internal unsafe partial interface IVisual : IInspectable
    {
        System.Numerics.Vector2 AnchorPoint { get; }

        void SetAnchorPoint(System.Numerics.Vector2 value);
        CompositionBackfaceVisibility BackfaceVisibility { get; }

        void SetBackfaceVisibility(CompositionBackfaceVisibility value);
        CompositionBorderMode BorderMode { get; }

        void SetBorderMode(CompositionBorderMode value);
        System.Numerics.Vector3 CenterPoint { get; }

        void SetCenterPoint(System.Numerics.Vector3 value);
        ICompositionClip Clip { get; }

        void SetClip(ICompositionClip value);
        CompositionCompositeMode CompositeMode { get; }

        void SetCompositeMode(CompositionCompositeMode value);
        int IsVisible { get; }

        void SetIsVisible(int value);
        System.Numerics.Vector3 Offset { get; }

        void SetOffset(System.Numerics.Vector3 value);
        float Opacity { get; }

        void SetOpacity(float value);
        System.Numerics.Quaternion Orientation { get; }

        void SetOrientation(System.Numerics.Quaternion value);
        IContainerVisual Parent { get; }

        float RotationAngle { get; }

        void SetRotationAngle(float value);
        float RotationAngleInDegrees { get; }

        void SetRotationAngleInDegrees(float value);
        System.Numerics.Vector3 RotationAxis { get; }

        void SetRotationAxis(System.Numerics.Vector3 value);
        System.Numerics.Vector3 Scale { get; }

        void SetScale(System.Numerics.Vector3 value);
        System.Numerics.Vector2 Size { get; }

        void SetSize(System.Numerics.Vector2 value);
        System.Numerics.Matrix4x4 TransformMatrix { get; }

        void SetTransformMatrix(System.Numerics.Matrix4x4 value);
    }

    internal unsafe partial interface IVisual2 : IInspectable
    {
        IVisual ParentForTransform { get; }

        void SetParentForTransform(IVisual value);
        System.Numerics.Vector3 RelativeOffsetAdjustment { get; }

        void SetRelativeOffsetAdjustment(System.Numerics.Vector3 value);
        System.Numerics.Vector2 RelativeSizeAdjustment { get; }

        void SetRelativeSizeAdjustment(System.Numerics.Vector2 value);
    }

    internal unsafe partial interface IContainerVisual : IInspectable
    {
        IVisualCollection Children { get; }
    }

    internal unsafe partial interface IVisualCollection : IInspectable
    {
        int Count { get; }

        void InsertAbove(IVisual newChild, IVisual sibling);
        void InsertAtBottom(IVisual newChild);
        void InsertAtTop(IVisual newChild);
        void InsertBelow(IVisual newChild, IVisual sibling);
        void Remove(IVisual child);
        void RemoveAll();
    }

    internal unsafe partial interface ICompositionTarget : IInspectable
    {
        IVisual Root { get; }

        void SetRoot(IVisual value);
    }

    internal unsafe partial interface IGraphicsEffect : IInspectable
    {
        IntPtr Name { get; }

        void SetName(IntPtr name);
    }

    internal unsafe partial interface IGraphicsEffectSource : IInspectable
    {
    }

    internal unsafe partial interface IGraphicsEffectD2D1Interop : global::Avalonia.MicroCom.IUnknown
    {
        Guid EffectId { get; }

        void GetNamedPropertyMapping(IntPtr name, uint* index, GRAPHICS_EFFECT_PROPERTY_MAPPING* mapping);
        uint PropertyCount { get; }

        IPropertyValue GetProperty(uint index);
        IGraphicsEffectSource GetSource(uint index);
        uint SourceCount { get; }
    }

    internal unsafe partial interface ICompositionEffectSourceParameter : IInspectable
    {
        IntPtr Name { get; }
    }

    internal unsafe partial interface ICompositionEffectSourceParameterFactory : IInspectable
    {
        ICompositionEffectSourceParameter Create(IntPtr name);
    }

    internal unsafe partial interface ICompositionEffectFactory : IInspectable
    {
        ICompositionEffectBrush CreateBrush();
        int ExtendedError { get; }

        CompositionEffectFactoryLoadStatus LoadStatus { get; }
    }

    internal unsafe partial interface ICompositionEffectBrush : IInspectable
    {
        ICompositionBrush GetSourceParameter(IntPtr name);
        void SetSourceParameter(IntPtr name, ICompositionBrush source);
    }

    internal unsafe partial interface ICompositionBackdropBrush : IInspectable
    {
    }

    internal unsafe partial interface ICompositionColorBrush : IInspectable
    {
        Avalonia.Win32.WinRT.WinRTColor Color { get; }

        void SetColor(Avalonia.Win32.WinRT.WinRTColor value);
    }

    internal unsafe partial interface ICompositionScopedBatch : IInspectable
    {
        int IsActive { get; }

        int IsEnded { get; }

        void End();
        void Resume();
        void Suspend();
        int AddCompleted(void* handler);
        void RemoveCompleted(int token);
    }

    internal unsafe partial interface ICompositionRoundedRectangleGeometry : IInspectable
    {
        System.Numerics.Vector2 CornerRadius { get; }

        void SetCornerRadius(System.Numerics.Vector2 value);
        System.Numerics.Vector2 Offset { get; }

        void SetOffset(System.Numerics.Vector2 value);
        System.Numerics.Vector2 Size { get; }

        void SetSize(System.Numerics.Vector2 value);
    }

    internal unsafe partial interface ICompositionGeometry : IInspectable
    {
        float TrimEnd { get; }

        void SetTrimEnd(float value);
        float TrimOffset { get; }

        void SetTrimOffset(float value);
        float TrimStart { get; }

        void SetTrimStart(float value);
    }

    internal unsafe partial interface ICompositionSpriteShape : IInspectable
    {
        ICompositionBrush FillBrush { get; }

        void SetFillBrush(ICompositionBrush value);
        ICompositionGeometry Geometry { get; }

        void SetGeometry(ICompositionGeometry value);
        int IsStrokeNonScaling { get; }

        void SetIsStrokeNonScaling(int value);
        ICompositionBrush StrokeBrush { get; }

        void SetStrokeBrush(ICompositionBrush value);
        void GetStrokeDashArray();
        void GetStrokeDashCap();
        void SetStrokeDashCap();
        void GetStrokeDashOffset();
        void SetStrokeDashOffset();
        void GetStrokeEndCap();
        void SetStrokeEndCap();
        void GetStrokeLineJoin();
        void SetStrokeLineJoin();
        void GetStrokeMiterLimit();
        void SetStrokeMiterLimit();
        void GetStrokeStartCap();
        void SetStrokeStartCap();
        void GetStrokeThickness();
        void SetStrokeThickness();
    }

    internal unsafe partial interface ICompositionShape : IInspectable
    {
        System.Numerics.Vector2 CenterPoint { get; }

        void SetCenterPoint(System.Numerics.Vector2 value);
    }

    internal unsafe partial interface IVectorOfCompositionShape : IInspectable
    {
        void GetAt();
        void GetSize();
        void GetView();
        void IndexOf();
        void SetAt();
        void InsertAt();
        void RemoveAt();
        void Append(ICompositionShape value);
        void RemoveAtEnd();
        void Clear();
    }

    internal unsafe partial interface ICompositionGeometricClip : IInspectable
    {
        ICompositionGeometry Geometry { get; }

        void SetGeometry(ICompositionGeometry value);
    }

    internal unsafe partial interface IShapeVisual : IInspectable
    {
        IUnknown Shapes { get; }
    }

    internal unsafe partial interface ICompositor5 : IInspectable
    {
        IntPtr Comment { get; }

        void SetComment(IntPtr value);
        float GlobalPlaybackRate { get; }

        void SetGlobalPlaybackRate(float value);
        void* CreateBounceScalarAnimation();
        void* CreateBounceVector2Animation();
        void* CreateBounceVector3Animation();
        void* CreateContainerShape();
        void* CreateEllipseGeometry();
        void* CreateLineGeometry();
        void* CreatePathGeometry();
        void* CreatePathGeometryWithPath(void* path);
        void* CreatePathKeyFrameAnimation();
        void* CreateRectangleGeometry();
        ICompositionRoundedRectangleGeometry CreateRoundedRectangleGeometry();
        IShapeVisual CreateShapeVisual();
        void* CreateSpriteShape();
        void* CreateSpriteShapeWithGeometry(void* geometry);
        void* CreateViewBox();
        IAsyncAction RequestCommitAsync();
    }

    internal unsafe partial interface ICompositor6 : IInspectable
    {
        ICompositionGeometricClip CreateGeometricClip();
        ICompositionGeometricClip CreateGeometricClipWithGeometry(ICompositionGeometry geometry);
    }
}

namespace Avalonia.Win32.WinRT.Impl
{
    internal unsafe partial class __MicroComIInspectableProxy : global::Avalonia.MicroCom.MicroComProxyBase, IInspectable
    {
        public void GetIids(ulong* iidCount, Guid** iids)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, iidCount, iids);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetIids failed", __result);
        }

        public IntPtr RuntimeClassName
        {
            get
            {
                int __result;
                IntPtr className = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &className);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetRuntimeClassName failed", __result);
                return className;
            }
        }

        public TrustLevel TrustLevel
        {
            get
            {
                int __result;
                TrustLevel trustLevel = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &trustLevel);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetTrustLevel failed", __result);
                return trustLevel;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IInspectable), new Guid("AF86E2E0-B12D-4c6a-9C5A-D7AA65101E90"), (p, owns) => new __MicroComIInspectableProxy(p, owns));
        }

        protected __MicroComIInspectableProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 3;
    }

    unsafe class __MicroComIInspectableVTable : global::Avalonia.MicroCom.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetIidsDelegate(void* @this, ulong* iidCount, Guid** iids);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetIids(void* @this, ulong* iidCount, Guid** iids)
        {
            IInspectable __target = null;
            try
            {
                {
                    __target = (IInspectable)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetIids(iidCount, iids);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetRuntimeClassNameDelegate(void* @this, IntPtr* className);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetRuntimeClassName(void* @this, IntPtr* className)
        {
            IInspectable __target = null;
            try
            {
                {
                    __target = (IInspectable)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.RuntimeClassName;
                        *className = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetTrustLevelDelegate(void* @this, TrustLevel* trustLevel);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetTrustLevel(void* @this, TrustLevel* trustLevel)
        {
            IInspectable __target = null;
            try
            {
                {
                    __target = (IInspectable)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.TrustLevel;
                        *trustLevel = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIInspectableVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, ulong*, Guid**, int>)&GetIids); 
#else
            base.AddMethod((GetIidsDelegate)GetIids); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetRuntimeClassName); 
#else
            base.AddMethod((GetRuntimeClassNameDelegate)GetRuntimeClassName); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, TrustLevel*, int>)&GetTrustLevel); 
#else
            base.AddMethod((GetTrustLevelDelegate)GetTrustLevel); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IInspectable), new __MicroComIInspectableVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIPropertyValueProxy : __MicroComIInspectableProxy, IPropertyValue
    {
        public PropertyType Type
        {
            get
            {
                int __result;
                PropertyType value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetType failed", __result);
                return value;
            }
        }

        public int IsNumericScalar
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetIsNumericScalar failed", __result);
                return value;
            }
        }

        public byte UInt8
        {
            get
            {
                int __result;
                byte value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetUInt8 failed", __result);
                return value;
            }
        }

        public short Int16
        {
            get
            {
                int __result;
                short value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetInt16 failed", __result);
                return value;
            }
        }

        public ushort UInt16
        {
            get
            {
                int __result;
                ushort value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetUInt16 failed", __result);
                return value;
            }
        }

        public int Int32
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 5])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetInt32 failed", __result);
                return value;
            }
        }

        public uint UInt32
        {
            get
            {
                int __result;
                uint value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 6])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetUInt32 failed", __result);
                return value;
            }
        }

        public long Int64
        {
            get
            {
                int __result;
                long value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 7])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetInt64 failed", __result);
                return value;
            }
        }

        public ulong UInt64
        {
            get
            {
                int __result;
                ulong value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 8])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetUInt64 failed", __result);
                return value;
            }
        }

        public float Single
        {
            get
            {
                int __result;
                float value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 9])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetSingle failed", __result);
                return value;
            }
        }

        public double Double
        {
            get
            {
                int __result;
                double value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 10])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetDouble failed", __result);
                return value;
            }
        }

        public System.Char Char16
        {
            get
            {
                int __result;
                System.Char value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 11])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetChar16 failed", __result);
                return value;
            }
        }

        public int Boolean
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 12])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetBoolean failed", __result);
                return value;
            }
        }

        public IntPtr String
        {
            get
            {
                int __result;
                IntPtr value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 13])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetString failed", __result);
                return value;
            }
        }

        public System.Guid Guid
        {
            get
            {
                int __result;
                System.Guid value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 14])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetGuid failed", __result);
                return value;
            }
        }

        public void GetDateTime(void* value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 15])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetDateTime failed", __result);
        }

        public void GetTimeSpan(void* value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 16])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetTimeSpan failed", __result);
        }

        public void GetPoint(void* value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 17])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetPoint failed", __result);
        }

        public void GetSize(void* value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 18])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetSize failed", __result);
        }

        public void GetRect(void* value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 19])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetRect failed", __result);
        }

        public byte* GetUInt8Array(uint* __valueSize)
        {
            int __result;
            byte* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 20])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetUInt8Array failed", __result);
            return value;
        }

        public short* GetInt16Array(uint* __valueSize)
        {
            int __result;
            short* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 21])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetInt16Array failed", __result);
            return value;
        }

        public ushort* GetUInt16Array(uint* __valueSize)
        {
            int __result;
            ushort* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 22])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetUInt16Array failed", __result);
            return value;
        }

        public int* GetInt32Array(uint* __valueSize)
        {
            int __result;
            int* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 23])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetInt32Array failed", __result);
            return value;
        }

        public uint* GetUInt32Array(uint* __valueSize)
        {
            int __result;
            uint* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 24])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetUInt32Array failed", __result);
            return value;
        }

        public long* GetInt64Array(uint* __valueSize)
        {
            int __result;
            long* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 25])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetInt64Array failed", __result);
            return value;
        }

        public ulong* GetUInt64Array(uint* __valueSize)
        {
            int __result;
            ulong* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 26])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetUInt64Array failed", __result);
            return value;
        }

        public float* GetSingleArray(uint* __valueSize)
        {
            int __result;
            float* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 27])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetSingleArray failed", __result);
            return value;
        }

        public double* GetDoubleArray(uint* __valueSize)
        {
            int __result;
            double* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 28])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetDoubleArray failed", __result);
            return value;
        }

        public System.Char* GetChar16Array(uint* __valueSize)
        {
            int __result;
            System.Char* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 29])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetChar16Array failed", __result);
            return value;
        }

        public int* GetBooleanArray(uint* __valueSize)
        {
            int __result;
            int* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 30])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetBooleanArray failed", __result);
            return value;
        }

        public IntPtr* GetStringArray(uint* __valueSize)
        {
            int __result;
            IntPtr* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 31])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetStringArray failed", __result);
            return value;
        }

        public void** GetInspectableArray(uint* __valueSize)
        {
            int __result;
            void** value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 32])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetInspectableArray failed", __result);
            return value;
        }

        public System.Guid* GetGuidArray(uint* __valueSize)
        {
            int __result;
            System.Guid* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 33])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetGuidArray failed", __result);
            return value;
        }

        public void* GetDateTimeArray(uint* __valueSize)
        {
            int __result;
            void* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 34])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetDateTimeArray failed", __result);
            return value;
        }

        public void* GetTimeSpanArray(uint* __valueSize)
        {
            int __result;
            void* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 35])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetTimeSpanArray failed", __result);
            return value;
        }

        public void* GetPointArray(uint* __valueSize)
        {
            int __result;
            void* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 36])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetPointArray failed", __result);
            return value;
        }

        public void* GetSizeArray(uint* __valueSize)
        {
            int __result;
            void* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 37])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetSizeArray failed", __result);
            return value;
        }

        public void* GetRectArray(uint* __valueSize)
        {
            int __result;
            void* value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 38])(PPV, __valueSize, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetRectArray failed", __result);
            return value;
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IPropertyValue), new Guid("4BD682DD-7554-40E9-9A9B-82654EDE7E62"), (p, owns) => new __MicroComIPropertyValueProxy(p, owns));
        }

        protected __MicroComIPropertyValueProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 39;
    }

    unsafe class __MicroComIPropertyValueVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetTypeDelegate(void* @this, PropertyType* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetType(void* @this, PropertyType* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Type;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetIsNumericScalarDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetIsNumericScalar(void* @this, int* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.IsNumericScalar;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetUInt8Delegate(void* @this, byte* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetUInt8(void* @this, byte* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.UInt8;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetInt16Delegate(void* @this, short* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetInt16(void* @this, short* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Int16;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetUInt16Delegate(void* @this, ushort* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetUInt16(void* @this, ushort* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.UInt16;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetInt32Delegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetInt32(void* @this, int* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Int32;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetUInt32Delegate(void* @this, uint* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetUInt32(void* @this, uint* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.UInt32;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetInt64Delegate(void* @this, long* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetInt64(void* @this, long* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Int64;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetUInt64Delegate(void* @this, ulong* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetUInt64(void* @this, ulong* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.UInt64;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSingleDelegate(void* @this, float* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSingle(void* @this, float* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Single;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetDoubleDelegate(void* @this, double* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetDouble(void* @this, double* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Double;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetChar16Delegate(void* @this, System.Char* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetChar16(void* @this, System.Char* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Char16;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetBooleanDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetBoolean(void* @this, int* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Boolean;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetStringDelegate(void* @this, IntPtr* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetString(void* @this, IntPtr* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.String;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetGuidDelegate(void* @this, System.Guid* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetGuid(void* @this, System.Guid* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Guid;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetDateTimeDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetDateTime(void* @this, void* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetDateTime(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetTimeSpanDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetTimeSpan(void* @this, void* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetTimeSpan(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetPointDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetPoint(void* @this, void* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetPoint(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSizeDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSize(void* @this, void* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetSize(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetRectDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetRect(void* @this, void* value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetRect(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetUInt8ArrayDelegate(void* @this, uint* __valueSize, byte** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetUInt8Array(void* @this, uint* __valueSize, byte** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetUInt8Array(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetInt16ArrayDelegate(void* @this, uint* __valueSize, short** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetInt16Array(void* @this, uint* __valueSize, short** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetInt16Array(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetUInt16ArrayDelegate(void* @this, uint* __valueSize, ushort** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetUInt16Array(void* @this, uint* __valueSize, ushort** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetUInt16Array(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetInt32ArrayDelegate(void* @this, uint* __valueSize, int** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetInt32Array(void* @this, uint* __valueSize, int** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetInt32Array(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetUInt32ArrayDelegate(void* @this, uint* __valueSize, uint** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetUInt32Array(void* @this, uint* __valueSize, uint** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetUInt32Array(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetInt64ArrayDelegate(void* @this, uint* __valueSize, long** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetInt64Array(void* @this, uint* __valueSize, long** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetInt64Array(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetUInt64ArrayDelegate(void* @this, uint* __valueSize, ulong** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetUInt64Array(void* @this, uint* __valueSize, ulong** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetUInt64Array(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSingleArrayDelegate(void* @this, uint* __valueSize, float** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSingleArray(void* @this, uint* __valueSize, float** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetSingleArray(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetDoubleArrayDelegate(void* @this, uint* __valueSize, double** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetDoubleArray(void* @this, uint* __valueSize, double** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetDoubleArray(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetChar16ArrayDelegate(void* @this, uint* __valueSize, System.Char** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetChar16Array(void* @this, uint* __valueSize, System.Char** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetChar16Array(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetBooleanArrayDelegate(void* @this, uint* __valueSize, int** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetBooleanArray(void* @this, uint* __valueSize, int** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetBooleanArray(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetStringArrayDelegate(void* @this, uint* __valueSize, IntPtr** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetStringArray(void* @this, uint* __valueSize, IntPtr** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetStringArray(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetInspectableArrayDelegate(void* @this, uint* __valueSize, void*** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetInspectableArray(void* @this, uint* __valueSize, void*** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetInspectableArray(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetGuidArrayDelegate(void* @this, uint* __valueSize, System.Guid** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetGuidArray(void* @this, uint* __valueSize, System.Guid** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetGuidArray(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetDateTimeArrayDelegate(void* @this, uint* __valueSize, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetDateTimeArray(void* @this, uint* __valueSize, void** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetDateTimeArray(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetTimeSpanArrayDelegate(void* @this, uint* __valueSize, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetTimeSpanArray(void* @this, uint* __valueSize, void** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetTimeSpanArray(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetPointArrayDelegate(void* @this, uint* __valueSize, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetPointArray(void* @this, uint* __valueSize, void** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetPointArray(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSizeArrayDelegate(void* @this, uint* __valueSize, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSizeArray(void* @this, uint* __valueSize, void** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetSizeArray(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetRectArrayDelegate(void* @this, uint* __valueSize, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetRectArray(void* @this, uint* __valueSize, void** value)
        {
            IPropertyValue __target = null;
            try
            {
                {
                    __target = (IPropertyValue)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetRectArray(__valueSize);
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIPropertyValueVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, PropertyType*, int>)&GetType); 
#else
            base.AddMethod((GetTypeDelegate)GetType); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetIsNumericScalar); 
#else
            base.AddMethod((GetIsNumericScalarDelegate)GetIsNumericScalar); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, byte*, int>)&GetUInt8); 
#else
            base.AddMethod((GetUInt8Delegate)GetUInt8); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, short*, int>)&GetInt16); 
#else
            base.AddMethod((GetInt16Delegate)GetInt16); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, ushort*, int>)&GetUInt16); 
#else
            base.AddMethod((GetUInt16Delegate)GetUInt16); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetInt32); 
#else
            base.AddMethod((GetInt32Delegate)GetInt32); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int>)&GetUInt32); 
#else
            base.AddMethod((GetUInt32Delegate)GetUInt32); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, long*, int>)&GetInt64); 
#else
            base.AddMethod((GetInt64Delegate)GetInt64); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, ulong*, int>)&GetUInt64); 
#else
            base.AddMethod((GetUInt64Delegate)GetUInt64); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float*, int>)&GetSingle); 
#else
            base.AddMethod((GetSingleDelegate)GetSingle); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, double*, int>)&GetDouble); 
#else
            base.AddMethod((GetDoubleDelegate)GetDouble); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Char*, int>)&GetChar16); 
#else
            base.AddMethod((GetChar16Delegate)GetChar16); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetBoolean); 
#else
            base.AddMethod((GetBooleanDelegate)GetBoolean); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetString); 
#else
            base.AddMethod((GetStringDelegate)GetString); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Guid*, int>)&GetGuid); 
#else
            base.AddMethod((GetGuidDelegate)GetGuid); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&GetDateTime); 
#else
            base.AddMethod((GetDateTimeDelegate)GetDateTime); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&GetTimeSpan); 
#else
            base.AddMethod((GetTimeSpanDelegate)GetTimeSpan); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&GetPoint); 
#else
            base.AddMethod((GetPointDelegate)GetPoint); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&GetSize); 
#else
            base.AddMethod((GetSizeDelegate)GetSize); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&GetRect); 
#else
            base.AddMethod((GetRectDelegate)GetRect); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, byte**, int>)&GetUInt8Array); 
#else
            base.AddMethod((GetUInt8ArrayDelegate)GetUInt8Array); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, short**, int>)&GetInt16Array); 
#else
            base.AddMethod((GetInt16ArrayDelegate)GetInt16Array); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, ushort**, int>)&GetUInt16Array); 
#else
            base.AddMethod((GetUInt16ArrayDelegate)GetUInt16Array); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int**, int>)&GetInt32Array); 
#else
            base.AddMethod((GetInt32ArrayDelegate)GetInt32Array); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, uint**, int>)&GetUInt32Array); 
#else
            base.AddMethod((GetUInt32ArrayDelegate)GetUInt32Array); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, long**, int>)&GetInt64Array); 
#else
            base.AddMethod((GetInt64ArrayDelegate)GetInt64Array); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, ulong**, int>)&GetUInt64Array); 
#else
            base.AddMethod((GetUInt64ArrayDelegate)GetUInt64Array); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, float**, int>)&GetSingleArray); 
#else
            base.AddMethod((GetSingleArrayDelegate)GetSingleArray); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, double**, int>)&GetDoubleArray); 
#else
            base.AddMethod((GetDoubleArrayDelegate)GetDoubleArray); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, System.Char**, int>)&GetChar16Array); 
#else
            base.AddMethod((GetChar16ArrayDelegate)GetChar16Array); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int**, int>)&GetBooleanArray); 
#else
            base.AddMethod((GetBooleanArrayDelegate)GetBooleanArray); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, IntPtr**, int>)&GetStringArray); 
#else
            base.AddMethod((GetStringArrayDelegate)GetStringArray); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, void***, int>)&GetInspectableArray); 
#else
            base.AddMethod((GetInspectableArrayDelegate)GetInspectableArray); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, System.Guid**, int>)&GetGuidArray); 
#else
            base.AddMethod((GetGuidArrayDelegate)GetGuidArray); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, void**, int>)&GetDateTimeArray); 
#else
            base.AddMethod((GetDateTimeArrayDelegate)GetDateTimeArray); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, void**, int>)&GetTimeSpanArray); 
#else
            base.AddMethod((GetTimeSpanArrayDelegate)GetTimeSpanArray); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, void**, int>)&GetPointArray); 
#else
            base.AddMethod((GetPointArrayDelegate)GetPointArray); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, void**, int>)&GetSizeArray); 
#else
            base.AddMethod((GetSizeArrayDelegate)GetSizeArray); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, void**, int>)&GetRectArray); 
#else
            base.AddMethod((GetRectArrayDelegate)GetRectArray); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IPropertyValue), new __MicroComIPropertyValueVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIAsyncActionCompletedHandlerProxy : global::Avalonia.MicroCom.MicroComProxyBase, IAsyncActionCompletedHandler
    {
        public void Invoke(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, AsyncStatus, int>)(*PPV)[base.VTableSize + 0])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(asyncInfo), asyncStatus);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Invoke failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IAsyncActionCompletedHandler), new Guid("A4ED5C81-76C9-40BD-8BE6-B1D90FB20AE7"), (p, owns) => new __MicroComIAsyncActionCompletedHandlerProxy(p, owns));
        }

        protected __MicroComIAsyncActionCompletedHandlerProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIAsyncActionCompletedHandlerVTable : global::Avalonia.MicroCom.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int InvokeDelegate(void* @this, void* asyncInfo, AsyncStatus asyncStatus);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Invoke(void* @this, void* asyncInfo, AsyncStatus asyncStatus)
        {
            IAsyncActionCompletedHandler __target = null;
            try
            {
                {
                    __target = (IAsyncActionCompletedHandler)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Invoke(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IAsyncAction>(asyncInfo, false), asyncStatus);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIAsyncActionCompletedHandlerVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, AsyncStatus, int>)&Invoke); 
#else
            base.AddMethod((InvokeDelegate)Invoke); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IAsyncActionCompletedHandler), new __MicroComIAsyncActionCompletedHandlerVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIAsyncActionProxy : __MicroComIInspectableProxy, IAsyncAction
    {
        public void SetCompleted(IAsyncActionCompletedHandler handler)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(handler));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetCompleted failed", __result);
        }

        public IAsyncActionCompletedHandler Completed
        {
            get
            {
                int __result;
                void* __marshal_ppv = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &__marshal_ppv);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetCompleted failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IAsyncActionCompletedHandler>(__marshal_ppv, true);
            }
        }

        public void GetResults()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 2])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetResults failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IAsyncAction), new Guid("5A648006-843A-4DA9-865B-9D26E5DFAD7B"), (p, owns) => new __MicroComIAsyncActionProxy(p, owns));
        }

        protected __MicroComIAsyncActionProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 3;
    }

    unsafe class __MicroComIAsyncActionVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetCompletedDelegate(void* @this, void* handler);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetCompleted(void* @this, void* handler)
        {
            IAsyncAction __target = null;
            try
            {
                {
                    __target = (IAsyncAction)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetCompleted(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IAsyncActionCompletedHandler>(handler, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetCompletedDelegate(void* @this, void** ppv);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCompleted(void* @this, void** ppv)
        {
            IAsyncAction __target = null;
            try
            {
                {
                    __target = (IAsyncAction)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Completed;
                        *ppv = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetResultsDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetResults(void* @this)
        {
            IAsyncAction __target = null;
            try
            {
                {
                    __target = (IAsyncAction)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetResults();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIAsyncActionVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetCompleted); 
#else
            base.AddMethod((SetCompletedDelegate)SetCompleted); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetCompleted); 
#else
            base.AddMethod((GetCompletedDelegate)GetCompleted); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&GetResults); 
#else
            base.AddMethod((GetResultsDelegate)GetResults); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IAsyncAction), new __MicroComIAsyncActionVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIDispatcherQueueProxy : __MicroComIInspectableProxy, IDispatcherQueue
    {
        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IDispatcherQueue), new Guid("603E88E4-A338-4FFE-A457-A5CFB9CEB899"), (p, owns) => new __MicroComIDispatcherQueueProxy(p, owns));
        }

        protected __MicroComIDispatcherQueueProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 0;
    }

    unsafe class __MicroComIDispatcherQueueVTable : __MicroComIInspectableVTable
    {
        protected __MicroComIDispatcherQueueVTable()
        {
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IDispatcherQueue), new __MicroComIDispatcherQueueVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIDispatcherQueueControllerProxy : __MicroComIInspectableProxy, IDispatcherQueueController
    {
        public IDispatcherQueue DispatcherQueue
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetDispatcherQueue failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IDispatcherQueue>(__marshal_value, true);
            }
        }

        public IAsyncAction ShutdownQueueAsync()
        {
            int __result;
            void* __marshal_operation = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &__marshal_operation);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("ShutdownQueueAsync failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IAsyncAction>(__marshal_operation, true);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IDispatcherQueueController), new Guid("22F34E66-50DB-4E36-A98D-61C01B384D20"), (p, owns) => new __MicroComIDispatcherQueueControllerProxy(p, owns));
        }

        protected __MicroComIDispatcherQueueControllerProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComIDispatcherQueueControllerVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetDispatcherQueueDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetDispatcherQueue(void* @this, void** value)
        {
            IDispatcherQueueController __target = null;
            try
            {
                {
                    __target = (IDispatcherQueueController)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.DispatcherQueue;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ShutdownQueueAsyncDelegate(void* @this, void** operation);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int ShutdownQueueAsync(void* @this, void** operation)
        {
            IDispatcherQueueController __target = null;
            try
            {
                {
                    __target = (IDispatcherQueueController)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.ShutdownQueueAsync();
                        *operation = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIDispatcherQueueControllerVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetDispatcherQueue); 
#else
            base.AddMethod((GetDispatcherQueueDelegate)GetDispatcherQueue); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&ShutdownQueueAsync); 
#else
            base.AddMethod((ShutdownQueueAsyncDelegate)ShutdownQueueAsync); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IDispatcherQueueController), new __MicroComIDispatcherQueueControllerVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIActivationFactoryProxy : __MicroComIInspectableProxy, IActivationFactory
    {
        public IntPtr ActivateInstance()
        {
            int __result;
            IntPtr instance = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &instance);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("ActivateInstance failed", __result);
            return instance;
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IActivationFactory), new Guid("00000035-0000-0000-C000-000000000046"), (p, owns) => new __MicroComIActivationFactoryProxy(p, owns));
        }

        protected __MicroComIActivationFactoryProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIActivationFactoryVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ActivateInstanceDelegate(void* @this, IntPtr* instance);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int ActivateInstance(void* @this, IntPtr* instance)
        {
            IActivationFactory __target = null;
            try
            {
                {
                    __target = (IActivationFactory)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.ActivateInstance();
                        *instance = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIActivationFactoryVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&ActivateInstance); 
#else
            base.AddMethod((ActivateInstanceDelegate)ActivateInstance); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IActivationFactory), new __MicroComIActivationFactoryVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositorProxy : __MicroComIInspectableProxy, ICompositor
    {
        public void* CreateColorKeyFrameAnimation()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateColorKeyFrameAnimation failed", __result);
            return result;
        }

        public void* CreateColorBrush()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateColorBrush failed", __result);
            return result;
        }

        public ICompositionColorBrush CreateColorBrushWithColor(Avalonia.Win32.WinRT.WinRTColor* color)
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, color, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateColorBrushWithColor failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionColorBrush>(__marshal_result, true);
        }

        public IContainerVisual CreateContainerVisual()
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateContainerVisual failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IContainerVisual>(__marshal_result, true);
        }

        public void* CreateCubicBezierEasingFunction(System.Numerics.Vector2 controlPoint1, System.Numerics.Vector2 controlPoint2)
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, System.Numerics.Vector2, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, controlPoint1, controlPoint2, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateCubicBezierEasingFunction failed", __result);
            return result;
        }

        public ICompositionEffectFactory CreateEffectFactory(IGraphicsEffect graphicsEffect)
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 5])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(graphicsEffect), &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateEffectFactory failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionEffectFactory>(__marshal_result, true);
        }

        public void* CreateEffectFactoryWithProperties(void* graphicsEffect, void* animatableProperties)
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, int>)(*PPV)[base.VTableSize + 6])(PPV, graphicsEffect, animatableProperties, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateEffectFactoryWithProperties failed", __result);
            return result;
        }

        public void* CreateExpressionAnimation()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 7])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateExpressionAnimation failed", __result);
            return result;
        }

        public void* CreateExpressionAnimationWithExpression(IntPtr expression)
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*PPV)[base.VTableSize + 8])(PPV, expression, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateExpressionAnimationWithExpression failed", __result);
            return result;
        }

        public void* CreateInsetClip()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 9])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateInsetClip failed", __result);
            return result;
        }

        public void* CreateInsetClipWithInsets(float leftInset, float topInset, float rightInset, float bottomInset)
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, float, float, float, float, void*, int>)(*PPV)[base.VTableSize + 10])(PPV, leftInset, topInset, rightInset, bottomInset, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateInsetClipWithInsets failed", __result);
            return result;
        }

        public void* CreateLinearEasingFunction()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 11])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateLinearEasingFunction failed", __result);
            return result;
        }

        public void* CreatePropertySet()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 12])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreatePropertySet failed", __result);
            return result;
        }

        public void* CreateQuaternionKeyFrameAnimation()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 13])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateQuaternionKeyFrameAnimation failed", __result);
            return result;
        }

        public void* CreateScalarKeyFrameAnimation()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 14])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateScalarKeyFrameAnimation failed", __result);
            return result;
        }

        public ICompositionScopedBatch CreateScopedBatch(CompositionBatchTypes batchType)
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, CompositionBatchTypes, void*, int>)(*PPV)[base.VTableSize + 15])(PPV, batchType, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateScopedBatch failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionScopedBatch>(__marshal_result, true);
        }

        public ISpriteVisual CreateSpriteVisual()
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 16])(PPV, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateSpriteVisual failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ISpriteVisual>(__marshal_result, true);
        }

        public ICompositionSurfaceBrush CreateSurfaceBrush()
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 17])(PPV, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateSurfaceBrush failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionSurfaceBrush>(__marshal_result, true);
        }

        public ICompositionSurfaceBrush CreateSurfaceBrushWithSurface(ICompositionSurface surface)
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 18])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(surface), &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateSurfaceBrushWithSurface failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionSurfaceBrush>(__marshal_result, true);
        }

        public void* CreateTargetForCurrentView()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 19])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateTargetForCurrentView failed", __result);
            return result;
        }

        public void* CreateVector2KeyFrameAnimation()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 20])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateVector2KeyFrameAnimation failed", __result);
            return result;
        }

        public void* CreateVector3KeyFrameAnimation()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 21])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateVector3KeyFrameAnimation failed", __result);
            return result;
        }

        public void* CreateVector4KeyFrameAnimation()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 22])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateVector4KeyFrameAnimation failed", __result);
            return result;
        }

        public void* GetCommitBatch(CompositionBatchTypes batchType)
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, CompositionBatchTypes, void*, int>)(*PPV)[base.VTableSize + 23])(PPV, batchType, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetCommitBatch failed", __result);
            return result;
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositor), new Guid("B403CA50-7F8C-4E83-985F-CC45060036D8"), (p, owns) => new __MicroComICompositorProxy(p, owns));
        }

        protected __MicroComICompositorProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 24;
    }

    unsafe class __MicroComICompositorVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateColorKeyFrameAnimationDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateColorKeyFrameAnimation(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateColorKeyFrameAnimation();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateColorBrushDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateColorBrush(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateColorBrush();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateColorBrushWithColorDelegate(void* @this, Avalonia.Win32.WinRT.WinRTColor* color, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateColorBrushWithColor(void* @this, Avalonia.Win32.WinRT.WinRTColor* color, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateColorBrushWithColor(color);
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateContainerVisualDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateContainerVisual(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateContainerVisual();
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateCubicBezierEasingFunctionDelegate(void* @this, System.Numerics.Vector2 controlPoint1, System.Numerics.Vector2 controlPoint2, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateCubicBezierEasingFunction(void* @this, System.Numerics.Vector2 controlPoint1, System.Numerics.Vector2 controlPoint2, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateCubicBezierEasingFunction(controlPoint1, controlPoint2);
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateEffectFactoryDelegate(void* @this, void* graphicsEffect, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateEffectFactory(void* @this, void* graphicsEffect, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateEffectFactory(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IGraphicsEffect>(graphicsEffect, false));
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateEffectFactoryWithPropertiesDelegate(void* @this, void* graphicsEffect, void* animatableProperties, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateEffectFactoryWithProperties(void* @this, void* graphicsEffect, void* animatableProperties, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateEffectFactoryWithProperties(graphicsEffect, animatableProperties);
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateExpressionAnimationDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateExpressionAnimation(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateExpressionAnimation();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateExpressionAnimationWithExpressionDelegate(void* @this, IntPtr expression, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateExpressionAnimationWithExpression(void* @this, IntPtr expression, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateExpressionAnimationWithExpression(expression);
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateInsetClipDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateInsetClip(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateInsetClip();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateInsetClipWithInsetsDelegate(void* @this, float leftInset, float topInset, float rightInset, float bottomInset, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateInsetClipWithInsets(void* @this, float leftInset, float topInset, float rightInset, float bottomInset, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateInsetClipWithInsets(leftInset, topInset, rightInset, bottomInset);
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateLinearEasingFunctionDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateLinearEasingFunction(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateLinearEasingFunction();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreatePropertySetDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreatePropertySet(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreatePropertySet();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateQuaternionKeyFrameAnimationDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateQuaternionKeyFrameAnimation(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateQuaternionKeyFrameAnimation();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateScalarKeyFrameAnimationDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateScalarKeyFrameAnimation(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateScalarKeyFrameAnimation();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateScopedBatchDelegate(void* @this, CompositionBatchTypes batchType, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateScopedBatch(void* @this, CompositionBatchTypes batchType, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateScopedBatch(batchType);
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateSpriteVisualDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateSpriteVisual(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateSpriteVisual();
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateSurfaceBrushDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateSurfaceBrush(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateSurfaceBrush();
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateSurfaceBrushWithSurfaceDelegate(void* @this, void* surface, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateSurfaceBrushWithSurface(void* @this, void* surface, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateSurfaceBrushWithSurface(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionSurface>(surface, false));
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateTargetForCurrentViewDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateTargetForCurrentView(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateTargetForCurrentView();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateVector2KeyFrameAnimationDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateVector2KeyFrameAnimation(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateVector2KeyFrameAnimation();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateVector3KeyFrameAnimationDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateVector3KeyFrameAnimation(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateVector3KeyFrameAnimation();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateVector4KeyFrameAnimationDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateVector4KeyFrameAnimation(void* @this, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateVector4KeyFrameAnimation();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetCommitBatchDelegate(void* @this, CompositionBatchTypes batchType, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCommitBatch(void* @this, CompositionBatchTypes batchType, void** result)
        {
            ICompositor __target = null;
            try
            {
                {
                    __target = (ICompositor)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetCommitBatch(batchType);
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositorVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateColorKeyFrameAnimation); 
#else
            base.AddMethod((CreateColorKeyFrameAnimationDelegate)CreateColorKeyFrameAnimation); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateColorBrush); 
#else
            base.AddMethod((CreateColorBrushDelegate)CreateColorBrush); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, Avalonia.Win32.WinRT.WinRTColor*, void**, int>)&CreateColorBrushWithColor); 
#else
            base.AddMethod((CreateColorBrushWithColorDelegate)CreateColorBrushWithColor); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateContainerVisual); 
#else
            base.AddMethod((CreateContainerVisualDelegate)CreateContainerVisual); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, System.Numerics.Vector2, void**, int>)&CreateCubicBezierEasingFunction); 
#else
            base.AddMethod((CreateCubicBezierEasingFunctionDelegate)CreateCubicBezierEasingFunction); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, void**, int>)&CreateEffectFactory); 
#else
            base.AddMethod((CreateEffectFactoryDelegate)CreateEffectFactory); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, void*, void**, int>)&CreateEffectFactoryWithProperties); 
#else
            base.AddMethod((CreateEffectFactoryWithPropertiesDelegate)CreateEffectFactoryWithProperties); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateExpressionAnimation); 
#else
            base.AddMethod((CreateExpressionAnimationDelegate)CreateExpressionAnimation); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, void**, int>)&CreateExpressionAnimationWithExpression); 
#else
            base.AddMethod((CreateExpressionAnimationWithExpressionDelegate)CreateExpressionAnimationWithExpression); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateInsetClip); 
#else
            base.AddMethod((CreateInsetClipDelegate)CreateInsetClip); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float, float, float, float, void**, int>)&CreateInsetClipWithInsets); 
#else
            base.AddMethod((CreateInsetClipWithInsetsDelegate)CreateInsetClipWithInsets); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateLinearEasingFunction); 
#else
            base.AddMethod((CreateLinearEasingFunctionDelegate)CreateLinearEasingFunction); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreatePropertySet); 
#else
            base.AddMethod((CreatePropertySetDelegate)CreatePropertySet); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateQuaternionKeyFrameAnimation); 
#else
            base.AddMethod((CreateQuaternionKeyFrameAnimationDelegate)CreateQuaternionKeyFrameAnimation); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateScalarKeyFrameAnimation); 
#else
            base.AddMethod((CreateScalarKeyFrameAnimationDelegate)CreateScalarKeyFrameAnimation); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, CompositionBatchTypes, void**, int>)&CreateScopedBatch); 
#else
            base.AddMethod((CreateScopedBatchDelegate)CreateScopedBatch); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateSpriteVisual); 
#else
            base.AddMethod((CreateSpriteVisualDelegate)CreateSpriteVisual); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateSurfaceBrush); 
#else
            base.AddMethod((CreateSurfaceBrushDelegate)CreateSurfaceBrush); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, void**, int>)&CreateSurfaceBrushWithSurface); 
#else
            base.AddMethod((CreateSurfaceBrushWithSurfaceDelegate)CreateSurfaceBrushWithSurface); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateTargetForCurrentView); 
#else
            base.AddMethod((CreateTargetForCurrentViewDelegate)CreateTargetForCurrentView); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateVector2KeyFrameAnimation); 
#else
            base.AddMethod((CreateVector2KeyFrameAnimationDelegate)CreateVector2KeyFrameAnimation); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateVector3KeyFrameAnimation); 
#else
            base.AddMethod((CreateVector3KeyFrameAnimationDelegate)CreateVector3KeyFrameAnimation); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateVector4KeyFrameAnimation); 
#else
            base.AddMethod((CreateVector4KeyFrameAnimationDelegate)CreateVector4KeyFrameAnimation); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, CompositionBatchTypes, void**, int>)&GetCommitBatch); 
#else
            base.AddMethod((GetCommitBatchDelegate)GetCommitBatch); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositor), new __MicroComICompositorVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositor2Proxy : __MicroComIInspectableProxy, ICompositor2
    {
        public void* CreateAmbientLight()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateAmbientLight failed", __result);
            return result;
        }

        public void* CreateAnimationGroup()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateAnimationGroup failed", __result);
            return result;
        }

        public ICompositionBackdropBrush CreateBackdropBrush()
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateBackdropBrush failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionBackdropBrush>(__marshal_result, true);
        }

        public void* CreateDistantLight()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateDistantLight failed", __result);
            return result;
        }

        public void* CreateDropShadow()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateDropShadow failed", __result);
            return result;
        }

        public void* CreateImplicitAnimationCollection()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 5])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateImplicitAnimationCollection failed", __result);
            return result;
        }

        public void* CreateLayerVisual()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 6])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateLayerVisual failed", __result);
            return result;
        }

        public void* CreateMaskBrush()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 7])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateMaskBrush failed", __result);
            return result;
        }

        public void* CreateNineGridBrush()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 8])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateNineGridBrush failed", __result);
            return result;
        }

        public void* CreatePointLight()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 9])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreatePointLight failed", __result);
            return result;
        }

        public void* CreateSpotLight()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 10])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateSpotLight failed", __result);
            return result;
        }

        public void* CreateStepEasingFunction()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 11])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateStepEasingFunction failed", __result);
            return result;
        }

        public void* CreateStepEasingFunctionWithStepCount(int stepCount)
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, void*, int>)(*PPV)[base.VTableSize + 12])(PPV, stepCount, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateStepEasingFunctionWithStepCount failed", __result);
            return result;
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositor2), new Guid("735081DC-5E24-45DA-A38F-E32CC349A9A0"), (p, owns) => new __MicroComICompositor2Proxy(p, owns));
        }

        protected __MicroComICompositor2Proxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 13;
    }

    unsafe class __MicroComICompositor2VTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateAmbientLightDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateAmbientLight(void* @this, void** result)
        {
            ICompositor2 __target = null;
            try
            {
                {
                    __target = (ICompositor2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateAmbientLight();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateAnimationGroupDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateAnimationGroup(void* @this, void** result)
        {
            ICompositor2 __target = null;
            try
            {
                {
                    __target = (ICompositor2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateAnimationGroup();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateBackdropBrushDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateBackdropBrush(void* @this, void** result)
        {
            ICompositor2 __target = null;
            try
            {
                {
                    __target = (ICompositor2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateBackdropBrush();
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateDistantLightDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateDistantLight(void* @this, void** result)
        {
            ICompositor2 __target = null;
            try
            {
                {
                    __target = (ICompositor2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateDistantLight();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateDropShadowDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateDropShadow(void* @this, void** result)
        {
            ICompositor2 __target = null;
            try
            {
                {
                    __target = (ICompositor2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateDropShadow();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateImplicitAnimationCollectionDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateImplicitAnimationCollection(void* @this, void** result)
        {
            ICompositor2 __target = null;
            try
            {
                {
                    __target = (ICompositor2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateImplicitAnimationCollection();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateLayerVisualDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateLayerVisual(void* @this, void** result)
        {
            ICompositor2 __target = null;
            try
            {
                {
                    __target = (ICompositor2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateLayerVisual();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateMaskBrushDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateMaskBrush(void* @this, void** result)
        {
            ICompositor2 __target = null;
            try
            {
                {
                    __target = (ICompositor2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateMaskBrush();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateNineGridBrushDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateNineGridBrush(void* @this, void** result)
        {
            ICompositor2 __target = null;
            try
            {
                {
                    __target = (ICompositor2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateNineGridBrush();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreatePointLightDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreatePointLight(void* @this, void** result)
        {
            ICompositor2 __target = null;
            try
            {
                {
                    __target = (ICompositor2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreatePointLight();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateSpotLightDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateSpotLight(void* @this, void** result)
        {
            ICompositor2 __target = null;
            try
            {
                {
                    __target = (ICompositor2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateSpotLight();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateStepEasingFunctionDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateStepEasingFunction(void* @this, void** result)
        {
            ICompositor2 __target = null;
            try
            {
                {
                    __target = (ICompositor2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateStepEasingFunction();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateStepEasingFunctionWithStepCountDelegate(void* @this, int stepCount, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateStepEasingFunctionWithStepCount(void* @this, int stepCount, void** result)
        {
            ICompositor2 __target = null;
            try
            {
                {
                    __target = (ICompositor2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateStepEasingFunctionWithStepCount(stepCount);
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositor2VTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateAmbientLight); 
#else
            base.AddMethod((CreateAmbientLightDelegate)CreateAmbientLight); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateAnimationGroup); 
#else
            base.AddMethod((CreateAnimationGroupDelegate)CreateAnimationGroup); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateBackdropBrush); 
#else
            base.AddMethod((CreateBackdropBrushDelegate)CreateBackdropBrush); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateDistantLight); 
#else
            base.AddMethod((CreateDistantLightDelegate)CreateDistantLight); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateDropShadow); 
#else
            base.AddMethod((CreateDropShadowDelegate)CreateDropShadow); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateImplicitAnimationCollection); 
#else
            base.AddMethod((CreateImplicitAnimationCollectionDelegate)CreateImplicitAnimationCollection); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateLayerVisual); 
#else
            base.AddMethod((CreateLayerVisualDelegate)CreateLayerVisual); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateMaskBrush); 
#else
            base.AddMethod((CreateMaskBrushDelegate)CreateMaskBrush); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateNineGridBrush); 
#else
            base.AddMethod((CreateNineGridBrushDelegate)CreateNineGridBrush); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreatePointLight); 
#else
            base.AddMethod((CreatePointLightDelegate)CreatePointLight); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateSpotLight); 
#else
            base.AddMethod((CreateSpotLightDelegate)CreateSpotLight); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateStepEasingFunction); 
#else
            base.AddMethod((CreateStepEasingFunctionDelegate)CreateStepEasingFunction); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, void**, int>)&CreateStepEasingFunctionWithStepCount); 
#else
            base.AddMethod((CreateStepEasingFunctionWithStepCountDelegate)CreateStepEasingFunctionWithStepCount); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositor2), new __MicroComICompositor2VTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositor3Proxy : __MicroComIInspectableProxy, ICompositor3
    {
        public ICompositionBackdropBrush CreateHostBackdropBrush()
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateHostBackdropBrush failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionBackdropBrush>(__marshal_result, true);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositor3), new Guid("C9DD8EF0-6EB1-4E3C-A658-675D9C64D4AB"), (p, owns) => new __MicroComICompositor3Proxy(p, owns));
        }

        protected __MicroComICompositor3Proxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComICompositor3VTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateHostBackdropBrushDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateHostBackdropBrush(void* @this, void** result)
        {
            ICompositor3 __target = null;
            try
            {
                {
                    __target = (ICompositor3)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateHostBackdropBrush();
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositor3VTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateHostBackdropBrush); 
#else
            base.AddMethod((CreateHostBackdropBrushDelegate)CreateHostBackdropBrush); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositor3), new __MicroComICompositor3VTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositorWithBlurredWallpaperBackdropBrushProxy : __MicroComIInspectableProxy, ICompositorWithBlurredWallpaperBackdropBrush
    {
        public ICompositionBackdropBrush TryCreateBlurredWallpaperBackdropBrush()
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("TryCreateBlurredWallpaperBackdropBrush failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionBackdropBrush>(__marshal_result, true);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositorWithBlurredWallpaperBackdropBrush), new Guid("0D8FB190-F122-5B8D-9FDD-543B0D8EB7F3"), (p, owns) => new __MicroComICompositorWithBlurredWallpaperBackdropBrushProxy(p, owns));
        }

        protected __MicroComICompositorWithBlurredWallpaperBackdropBrushProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComICompositorWithBlurredWallpaperBackdropBrushVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int TryCreateBlurredWallpaperBackdropBrushDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int TryCreateBlurredWallpaperBackdropBrush(void* @this, void** result)
        {
            ICompositorWithBlurredWallpaperBackdropBrush __target = null;
            try
            {
                {
                    __target = (ICompositorWithBlurredWallpaperBackdropBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.TryCreateBlurredWallpaperBackdropBrush();
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositorWithBlurredWallpaperBackdropBrushVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&TryCreateBlurredWallpaperBackdropBrush); 
#else
            base.AddMethod((TryCreateBlurredWallpaperBackdropBrushDelegate)TryCreateBlurredWallpaperBackdropBrush); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositorWithBlurredWallpaperBackdropBrush), new __MicroComICompositorWithBlurredWallpaperBackdropBrushVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComISpriteVisualProxy : __MicroComIInspectableProxy, ISpriteVisual
    {
        public ICompositionBrush Brush
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetBrush failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(__marshal_value, true);
            }
        }

        public void SetBrush(ICompositionBrush value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(value));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetBrush failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ISpriteVisual), new Guid("08E05581-1AD1-4F97-9757-402D76E4233B"), (p, owns) => new __MicroComISpriteVisualProxy(p, owns));
        }

        protected __MicroComISpriteVisualProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComISpriteVisualVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetBrushDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetBrush(void* @this, void** value)
        {
            ISpriteVisual __target = null;
            try
            {
                {
                    __target = (ISpriteVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Brush;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetBrushDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetBrush(void* @this, void* value)
        {
            ISpriteVisual __target = null;
            try
            {
                {
                    __target = (ISpriteVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetBrush(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(value, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComISpriteVisualVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetBrush); 
#else
            base.AddMethod((GetBrushDelegate)GetBrush); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetBrush); 
#else
            base.AddMethod((SetBrushDelegate)SetBrush); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ISpriteVisual), new __MicroComISpriteVisualVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionDrawingSurfaceInteropProxy : global::Avalonia.MicroCom.MicroComProxyBase, ICompositionDrawingSurfaceInterop
    {
        public Avalonia.Win32.Interop.UnmanagedMethods.POINT BeginDraw(Avalonia.Win32.Interop.UnmanagedMethods.RECT* updateRect, Guid* iid, void** updateObject)
        {
            int __result;
            Avalonia.Win32.Interop.UnmanagedMethods.POINT updateOffset = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, updateRect, iid, updateObject, &updateOffset);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("BeginDraw failed", __result);
            return updateOffset;
        }

        public void EndDraw()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 1])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("EndDraw failed", __result);
        }

        public void Resize(Avalonia.Win32.Interop.UnmanagedMethods.POINT sizePixels)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, Avalonia.Win32.Interop.UnmanagedMethods.POINT, int>)(*PPV)[base.VTableSize + 2])(PPV, sizePixels);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Resize failed", __result);
        }

        public void Scroll(Avalonia.Win32.Interop.UnmanagedMethods.RECT* scrollRect, Avalonia.Win32.Interop.UnmanagedMethods.RECT* clipRect, int offsetX, int offsetY)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int, int, int>)(*PPV)[base.VTableSize + 3])(PPV, scrollRect, clipRect, offsetX, offsetY);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Scroll failed", __result);
        }

        public void ResumeDraw()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 4])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("ResumeDraw failed", __result);
        }

        public void SuspendDraw()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 5])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SuspendDraw failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionDrawingSurfaceInterop), new Guid("FD04E6E3-FE0C-4C3C-AB19-A07601A576EE"), (p, owns) => new __MicroComICompositionDrawingSurfaceInteropProxy(p, owns));
        }

        protected __MicroComICompositionDrawingSurfaceInteropProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 6;
    }

    unsafe class __MicroComICompositionDrawingSurfaceInteropVTable : global::Avalonia.MicroCom.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int BeginDrawDelegate(void* @this, Avalonia.Win32.Interop.UnmanagedMethods.RECT* updateRect, Guid* iid, void** updateObject, Avalonia.Win32.Interop.UnmanagedMethods.POINT* updateOffset);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int BeginDraw(void* @this, Avalonia.Win32.Interop.UnmanagedMethods.RECT* updateRect, Guid* iid, void** updateObject, Avalonia.Win32.Interop.UnmanagedMethods.POINT* updateOffset)
        {
            ICompositionDrawingSurfaceInterop __target = null;
            try
            {
                {
                    __target = (ICompositionDrawingSurfaceInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.BeginDraw(updateRect, iid, updateObject);
                        *updateOffset = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int EndDrawDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int EndDraw(void* @this)
        {
            ICompositionDrawingSurfaceInterop __target = null;
            try
            {
                {
                    __target = (ICompositionDrawingSurfaceInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.EndDraw();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ResizeDelegate(void* @this, Avalonia.Win32.Interop.UnmanagedMethods.POINT sizePixels);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Resize(void* @this, Avalonia.Win32.Interop.UnmanagedMethods.POINT sizePixels)
        {
            ICompositionDrawingSurfaceInterop __target = null;
            try
            {
                {
                    __target = (ICompositionDrawingSurfaceInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Resize(sizePixels);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ScrollDelegate(void* @this, Avalonia.Win32.Interop.UnmanagedMethods.RECT* scrollRect, Avalonia.Win32.Interop.UnmanagedMethods.RECT* clipRect, int offsetX, int offsetY);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Scroll(void* @this, Avalonia.Win32.Interop.UnmanagedMethods.RECT* scrollRect, Avalonia.Win32.Interop.UnmanagedMethods.RECT* clipRect, int offsetX, int offsetY)
        {
            ICompositionDrawingSurfaceInterop __target = null;
            try
            {
                {
                    __target = (ICompositionDrawingSurfaceInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Scroll(scrollRect, clipRect, offsetX, offsetY);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ResumeDrawDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int ResumeDraw(void* @this)
        {
            ICompositionDrawingSurfaceInterop __target = null;
            try
            {
                {
                    __target = (ICompositionDrawingSurfaceInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.ResumeDraw();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SuspendDrawDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SuspendDraw(void* @this)
        {
            ICompositionDrawingSurfaceInterop __target = null;
            try
            {
                {
                    __target = (ICompositionDrawingSurfaceInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SuspendDraw();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionDrawingSurfaceInteropVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, Avalonia.Win32.Interop.UnmanagedMethods.RECT*, Guid*, void**, Avalonia.Win32.Interop.UnmanagedMethods.POINT*, int>)&BeginDraw); 
#else
            base.AddMethod((BeginDrawDelegate)BeginDraw); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&EndDraw); 
#else
            base.AddMethod((EndDrawDelegate)EndDraw); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, Avalonia.Win32.Interop.UnmanagedMethods.POINT, int>)&Resize); 
#else
            base.AddMethod((ResizeDelegate)Resize); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, Avalonia.Win32.Interop.UnmanagedMethods.RECT*, Avalonia.Win32.Interop.UnmanagedMethods.RECT*, int, int, int>)&Scroll); 
#else
            base.AddMethod((ScrollDelegate)Scroll); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&ResumeDraw); 
#else
            base.AddMethod((ResumeDrawDelegate)ResumeDraw); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&SuspendDraw); 
#else
            base.AddMethod((SuspendDrawDelegate)SuspendDraw); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionDrawingSurfaceInterop), new __MicroComICompositionDrawingSurfaceInteropVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionGraphicsDeviceInteropProxy : global::Avalonia.MicroCom.MicroComProxyBase, ICompositionGraphicsDeviceInterop
    {
        public IUnknown RenderingDevice
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetRenderingDevice failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IUnknown>(__marshal_value, true);
            }
        }

        public void SetRenderingDevice(IUnknown value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(value));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetRenderingDevice failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionGraphicsDeviceInterop), new Guid("A116FF71-F8BF-4C8A-9C98-70779A32A9C8"), (p, owns) => new __MicroComICompositionGraphicsDeviceInteropProxy(p, owns));
        }

        protected __MicroComICompositionGraphicsDeviceInteropProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComICompositionGraphicsDeviceInteropVTable : global::Avalonia.MicroCom.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetRenderingDeviceDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetRenderingDevice(void* @this, void** value)
        {
            ICompositionGraphicsDeviceInterop __target = null;
            try
            {
                {
                    __target = (ICompositionGraphicsDeviceInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.RenderingDevice;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetRenderingDeviceDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetRenderingDevice(void* @this, void* value)
        {
            ICompositionGraphicsDeviceInterop __target = null;
            try
            {
                {
                    __target = (ICompositionGraphicsDeviceInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetRenderingDevice(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IUnknown>(value, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionGraphicsDeviceInteropVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetRenderingDevice); 
#else
            base.AddMethod((GetRenderingDeviceDelegate)GetRenderingDevice); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetRenderingDevice); 
#else
            base.AddMethod((SetRenderingDeviceDelegate)SetRenderingDevice); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionGraphicsDeviceInterop), new __MicroComICompositionGraphicsDeviceInteropVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositorInteropProxy : global::Avalonia.MicroCom.MicroComProxyBase, ICompositorInterop
    {
        public ICompositionSurface CreateCompositionSurfaceForHandle(IntPtr swapChain)
        {
            int __result;
            void* __marshal_res = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, swapChain, &__marshal_res);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateCompositionSurfaceForHandle failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionSurface>(__marshal_res, true);
        }

        public ICompositionSurface CreateCompositionSurfaceForSwapChain(IUnknown swapChain)
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(swapChain), &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateCompositionSurfaceForSwapChain failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionSurface>(__marshal_result, true);
        }

        public ICompositionGraphicsDevice CreateGraphicsDevice(IUnknown renderingDevice)
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(renderingDevice), &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateGraphicsDevice failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionGraphicsDevice>(__marshal_result, true);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositorInterop), new Guid("25297D5C-3AD4-4C9C-B5CF-E36A38512330"), (p, owns) => new __MicroComICompositorInteropProxy(p, owns));
        }

        protected __MicroComICompositorInteropProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 3;
    }

    unsafe class __MicroComICompositorInteropVTable : global::Avalonia.MicroCom.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateCompositionSurfaceForHandleDelegate(void* @this, IntPtr swapChain, void** res);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateCompositionSurfaceForHandle(void* @this, IntPtr swapChain, void** res)
        {
            ICompositorInterop __target = null;
            try
            {
                {
                    __target = (ICompositorInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateCompositionSurfaceForHandle(swapChain);
                        *res = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateCompositionSurfaceForSwapChainDelegate(void* @this, void* swapChain, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateCompositionSurfaceForSwapChain(void* @this, void* swapChain, void** result)
        {
            ICompositorInterop __target = null;
            try
            {
                {
                    __target = (ICompositorInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateCompositionSurfaceForSwapChain(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IUnknown>(swapChain, false));
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateGraphicsDeviceDelegate(void* @this, void* renderingDevice, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateGraphicsDevice(void* @this, void* renderingDevice, void** result)
        {
            ICompositorInterop __target = null;
            try
            {
                {
                    __target = (ICompositorInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateGraphicsDevice(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IUnknown>(renderingDevice, false));
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositorInteropVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, void**, int>)&CreateCompositionSurfaceForHandle); 
#else
            base.AddMethod((CreateCompositionSurfaceForHandleDelegate)CreateCompositionSurfaceForHandle); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, void**, int>)&CreateCompositionSurfaceForSwapChain); 
#else
            base.AddMethod((CreateCompositionSurfaceForSwapChainDelegate)CreateCompositionSurfaceForSwapChain); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, void**, int>)&CreateGraphicsDevice); 
#else
            base.AddMethod((CreateGraphicsDeviceDelegate)CreateGraphicsDevice); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositorInterop), new __MicroComICompositorInteropVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComISwapChainInteropProxy : global::Avalonia.MicroCom.MicroComProxyBase, ISwapChainInterop
    {
        public void SetSwapChain(IUnknown swapChain)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(swapChain));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetSwapChain failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ISwapChainInterop), new Guid("26f496a0-7f38-45fb-88f7-faaabe67dd59"), (p, owns) => new __MicroComISwapChainInteropProxy(p, owns));
        }

        protected __MicroComISwapChainInteropProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComISwapChainInteropVTable : global::Avalonia.MicroCom.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetSwapChainDelegate(void* @this, void* swapChain);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetSwapChain(void* @this, void* swapChain)
        {
            ISwapChainInterop __target = null;
            try
            {
                {
                    __target = (ISwapChainInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetSwapChain(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IUnknown>(swapChain, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComISwapChainInteropVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetSwapChain); 
#else
            base.AddMethod((SetSwapChainDelegate)SetSwapChain); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ISwapChainInterop), new __MicroComISwapChainInteropVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositorDesktopInteropProxy : global::Avalonia.MicroCom.MicroComProxyBase, ICompositorDesktopInterop
    {
        public IDesktopWindowTarget CreateDesktopWindowTarget(IntPtr hwndTarget, int isTopmost)
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, hwndTarget, isTopmost, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateDesktopWindowTarget failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IDesktopWindowTarget>(__marshal_result, true);
        }

        public void EnsureOnThread(int threadId)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, int>)(*PPV)[base.VTableSize + 1])(PPV, threadId);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("EnsureOnThread failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositorDesktopInterop), new Guid("29E691FA-4567-4DCA-B319-D0F207EB6807"), (p, owns) => new __MicroComICompositorDesktopInteropProxy(p, owns));
        }

        protected __MicroComICompositorDesktopInteropProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComICompositorDesktopInteropVTable : global::Avalonia.MicroCom.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateDesktopWindowTargetDelegate(void* @this, IntPtr hwndTarget, int isTopmost, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateDesktopWindowTarget(void* @this, IntPtr hwndTarget, int isTopmost, void** result)
        {
            ICompositorDesktopInterop __target = null;
            try
            {
                {
                    __target = (ICompositorDesktopInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateDesktopWindowTarget(hwndTarget, isTopmost);
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int EnsureOnThreadDelegate(void* @this, int threadId);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int EnsureOnThread(void* @this, int threadId)
        {
            ICompositorDesktopInterop __target = null;
            try
            {
                {
                    __target = (ICompositorDesktopInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.EnsureOnThread(threadId);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositorDesktopInteropVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int, void**, int>)&CreateDesktopWindowTarget); 
#else
            base.AddMethod((CreateDesktopWindowTargetDelegate)CreateDesktopWindowTarget); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, int>)&EnsureOnThread); 
#else
            base.AddMethod((EnsureOnThreadDelegate)EnsureOnThread); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositorDesktopInterop), new __MicroComICompositorDesktopInteropVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIDesktopWindowTargetInteropProxy : global::Avalonia.MicroCom.MicroComProxyBase, IDesktopWindowTargetInterop
    {
        public IntPtr HWnd
        {
            get
            {
                int __result;
                IntPtr value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetHWnd failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IDesktopWindowTargetInterop), new Guid("35DBF59E-E3F9-45B0-81E7-FE75F4145DC9"), (p, owns) => new __MicroComIDesktopWindowTargetInteropProxy(p, owns));
        }

        protected __MicroComIDesktopWindowTargetInteropProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIDesktopWindowTargetInteropVTable : global::Avalonia.MicroCom.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetHWndDelegate(void* @this, IntPtr* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetHWnd(void* @this, IntPtr* value)
        {
            IDesktopWindowTargetInterop __target = null;
            try
            {
                {
                    __target = (IDesktopWindowTargetInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.HWnd;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIDesktopWindowTargetInteropVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetHWnd); 
#else
            base.AddMethod((GetHWndDelegate)GetHWnd); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IDesktopWindowTargetInterop), new __MicroComIDesktopWindowTargetInteropVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIDesktopWindowContentBridgeInteropProxy : global::Avalonia.MicroCom.MicroComProxyBase, IDesktopWindowContentBridgeInterop
    {
        public void Initialize(ICompositor compositor, IntPtr parentHwnd)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, IntPtr, int>)(*PPV)[base.VTableSize + 0])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(compositor), parentHwnd);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Initialize failed", __result);
        }

        public IntPtr HWnd
        {
            get
            {
                int __result;
                IntPtr value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetHWnd failed", __result);
                return value;
            }
        }

        public float AppliedScaleFactor
        {
            get
            {
                int __result;
                float value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetAppliedScaleFactor failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IDesktopWindowContentBridgeInterop), new Guid("37642806-F421-4FD0-9F82-23AE7C776182"), (p, owns) => new __MicroComIDesktopWindowContentBridgeInteropProxy(p, owns));
        }

        protected __MicroComIDesktopWindowContentBridgeInteropProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 3;
    }

    unsafe class __MicroComIDesktopWindowContentBridgeInteropVTable : global::Avalonia.MicroCom.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int InitializeDelegate(void* @this, void* compositor, IntPtr parentHwnd);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Initialize(void* @this, void* compositor, IntPtr parentHwnd)
        {
            IDesktopWindowContentBridgeInterop __target = null;
            try
            {
                {
                    __target = (IDesktopWindowContentBridgeInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Initialize(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositor>(compositor, false), parentHwnd);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetHWndDelegate(void* @this, IntPtr* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetHWnd(void* @this, IntPtr* value)
        {
            IDesktopWindowContentBridgeInterop __target = null;
            try
            {
                {
                    __target = (IDesktopWindowContentBridgeInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.HWnd;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetAppliedScaleFactorDelegate(void* @this, float* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetAppliedScaleFactor(void* @this, float* value)
        {
            IDesktopWindowContentBridgeInterop __target = null;
            try
            {
                {
                    __target = (IDesktopWindowContentBridgeInterop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.AppliedScaleFactor;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIDesktopWindowContentBridgeInteropVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, IntPtr, int>)&Initialize); 
#else
            base.AddMethod((InitializeDelegate)Initialize); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetHWnd); 
#else
            base.AddMethod((GetHWndDelegate)GetHWnd); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float*, int>)&GetAppliedScaleFactor); 
#else
            base.AddMethod((GetAppliedScaleFactorDelegate)GetAppliedScaleFactor); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IDesktopWindowContentBridgeInterop), new __MicroComIDesktopWindowContentBridgeInteropVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionGraphicsDeviceProxy : __MicroComIInspectableProxy, ICompositionGraphicsDevice
    {
        public ICompositionDrawingSurface CreateDrawingSurface(Avalonia.Win32.Interop.UnmanagedMethods.SIZE sizePixels, DirectXPixelFormat pixelFormat, DirectXAlphaMode alphaMode)
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, Avalonia.Win32.Interop.UnmanagedMethods.SIZE, DirectXPixelFormat, DirectXAlphaMode, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, sizePixels, pixelFormat, alphaMode, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateDrawingSurface failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionDrawingSurface>(__marshal_result, true);
        }

        public void AddRenderingDeviceReplaced(void* handler, void* token)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, handler, token);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("AddRenderingDeviceReplaced failed", __result);
        }

        public void RemoveRenderingDeviceReplaced(int token)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, int>)(*PPV)[base.VTableSize + 2])(PPV, token);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RemoveRenderingDeviceReplaced failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionGraphicsDevice), new Guid("FB22C6E1-80A2-4667-9936-DBEAF6EEFE95"), (p, owns) => new __MicroComICompositionGraphicsDeviceProxy(p, owns));
        }

        protected __MicroComICompositionGraphicsDeviceProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 3;
    }

    unsafe class __MicroComICompositionGraphicsDeviceVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateDrawingSurfaceDelegate(void* @this, Avalonia.Win32.Interop.UnmanagedMethods.SIZE sizePixels, DirectXPixelFormat pixelFormat, DirectXAlphaMode alphaMode, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateDrawingSurface(void* @this, Avalonia.Win32.Interop.UnmanagedMethods.SIZE sizePixels, DirectXPixelFormat pixelFormat, DirectXAlphaMode alphaMode, void** result)
        {
            ICompositionGraphicsDevice __target = null;
            try
            {
                {
                    __target = (ICompositionGraphicsDevice)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateDrawingSurface(sizePixels, pixelFormat, alphaMode);
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int AddRenderingDeviceReplacedDelegate(void* @this, void* handler, void* token);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int AddRenderingDeviceReplaced(void* @this, void* handler, void* token)
        {
            ICompositionGraphicsDevice __target = null;
            try
            {
                {
                    __target = (ICompositionGraphicsDevice)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.AddRenderingDeviceReplaced(handler, token);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveRenderingDeviceReplacedDelegate(void* @this, int token);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RemoveRenderingDeviceReplaced(void* @this, int token)
        {
            ICompositionGraphicsDevice __target = null;
            try
            {
                {
                    __target = (ICompositionGraphicsDevice)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RemoveRenderingDeviceReplaced(token);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionGraphicsDeviceVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, Avalonia.Win32.Interop.UnmanagedMethods.SIZE, DirectXPixelFormat, DirectXAlphaMode, void**, int>)&CreateDrawingSurface); 
#else
            base.AddMethod((CreateDrawingSurfaceDelegate)CreateDrawingSurface); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)&AddRenderingDeviceReplaced); 
#else
            base.AddMethod((AddRenderingDeviceReplacedDelegate)AddRenderingDeviceReplaced); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, int>)&RemoveRenderingDeviceReplaced); 
#else
            base.AddMethod((RemoveRenderingDeviceReplacedDelegate)RemoveRenderingDeviceReplaced); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionGraphicsDevice), new __MicroComICompositionGraphicsDeviceVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionSurfaceProxy : __MicroComIInspectableProxy, ICompositionSurface
    {
        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionSurface), new Guid("1527540D-42C7-47A6-A408-668F79A90DFB"), (p, owns) => new __MicroComICompositionSurfaceProxy(p, owns));
        }

        protected __MicroComICompositionSurfaceProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 0;
    }

    unsafe class __MicroComICompositionSurfaceVTable : __MicroComIInspectableVTable
    {
        protected __MicroComICompositionSurfaceVTable()
        {
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionSurface), new __MicroComICompositionSurfaceVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIDesktopWindowTargetProxy : __MicroComIInspectableProxy, IDesktopWindowTarget
    {
        public int IsTopmost
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetIsTopmost failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IDesktopWindowTarget), new Guid("6329D6CA-3366-490E-9DB3-25312929AC51"), (p, owns) => new __MicroComIDesktopWindowTargetProxy(p, owns));
        }

        protected __MicroComIDesktopWindowTargetProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIDesktopWindowTargetVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetIsTopmostDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetIsTopmost(void* @this, int* value)
        {
            IDesktopWindowTarget __target = null;
            try
            {
                {
                    __target = (IDesktopWindowTarget)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.IsTopmost;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIDesktopWindowTargetVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetIsTopmost); 
#else
            base.AddMethod((GetIsTopmostDelegate)GetIsTopmost); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IDesktopWindowTarget), new __MicroComIDesktopWindowTargetVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionDrawingSurfaceProxy : __MicroComIInspectableProxy, ICompositionDrawingSurface
    {
        public DirectXAlphaMode AlphaMode
        {
            get
            {
                int __result;
                DirectXAlphaMode value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetAlphaMode failed", __result);
                return value;
            }
        }

        public DirectXPixelFormat PixelFormat
        {
            get
            {
                int __result;
                DirectXPixelFormat value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetPixelFormat failed", __result);
                return value;
            }
        }

        public Avalonia.Win32.Interop.UnmanagedMethods.POINT Size
        {
            get
            {
                int __result;
                Avalonia.Win32.Interop.UnmanagedMethods.POINT value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetSize failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionDrawingSurface), new Guid("A166C300-FAD0-4D11-9E67-E433162FF49E"), (p, owns) => new __MicroComICompositionDrawingSurfaceProxy(p, owns));
        }

        protected __MicroComICompositionDrawingSurfaceProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 3;
    }

    unsafe class __MicroComICompositionDrawingSurfaceVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetAlphaModeDelegate(void* @this, DirectXAlphaMode* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetAlphaMode(void* @this, DirectXAlphaMode* value)
        {
            ICompositionDrawingSurface __target = null;
            try
            {
                {
                    __target = (ICompositionDrawingSurface)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.AlphaMode;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetPixelFormatDelegate(void* @this, DirectXPixelFormat* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetPixelFormat(void* @this, DirectXPixelFormat* value)
        {
            ICompositionDrawingSurface __target = null;
            try
            {
                {
                    __target = (ICompositionDrawingSurface)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.PixelFormat;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSizeDelegate(void* @this, Avalonia.Win32.Interop.UnmanagedMethods.POINT* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSize(void* @this, Avalonia.Win32.Interop.UnmanagedMethods.POINT* value)
        {
            ICompositionDrawingSurface __target = null;
            try
            {
                {
                    __target = (ICompositionDrawingSurface)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Size;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionDrawingSurfaceVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, DirectXAlphaMode*, int>)&GetAlphaMode); 
#else
            base.AddMethod((GetAlphaModeDelegate)GetAlphaMode); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, DirectXPixelFormat*, int>)&GetPixelFormat); 
#else
            base.AddMethod((GetPixelFormatDelegate)GetPixelFormat); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, Avalonia.Win32.Interop.UnmanagedMethods.POINT*, int>)&GetSize); 
#else
            base.AddMethod((GetSizeDelegate)GetSize); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionDrawingSurface), new __MicroComICompositionDrawingSurfaceVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionSurfaceBrushProxy : __MicroComIInspectableProxy, ICompositionSurfaceBrush
    {
        public CompositionBitmapInterpolationMode BitmapInterpolationMode
        {
            get
            {
                int __result;
                CompositionBitmapInterpolationMode value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetBitmapInterpolationMode failed", __result);
                return value;
            }
        }

        public void SetBitmapInterpolationMode(CompositionBitmapInterpolationMode value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, CompositionBitmapInterpolationMode, int>)(*PPV)[base.VTableSize + 1])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetBitmapInterpolationMode failed", __result);
        }

        public float HorizontalAlignmentRatio
        {
            get
            {
                int __result;
                float value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetHorizontalAlignmentRatio failed", __result);
                return value;
            }
        }

        public void SetHorizontalAlignmentRatio(float value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, float, int>)(*PPV)[base.VTableSize + 3])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetHorizontalAlignmentRatio failed", __result);
        }

        public CompositionStretch Stretch
        {
            get
            {
                int __result;
                CompositionStretch value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetStretch failed", __result);
                return value;
            }
        }

        public void SetStretch(CompositionStretch value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, CompositionStretch, int>)(*PPV)[base.VTableSize + 5])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetStretch failed", __result);
        }

        public ICompositionSurface Surface
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 6])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetSurface failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionSurface>(__marshal_value, true);
            }
        }

        public void SetSurface(ICompositionSurface value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 7])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(value));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetSurface failed", __result);
        }

        public float VerticalAlignmentRatio
        {
            get
            {
                int __result;
                float value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 8])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetVerticalAlignmentRatio failed", __result);
                return value;
            }
        }

        public void SetVerticalAlignmentRatio(float value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, float, int>)(*PPV)[base.VTableSize + 9])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetVerticalAlignmentRatio failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionSurfaceBrush), new Guid("AD016D79-1E4C-4C0D-9C29-83338C87C162"), (p, owns) => new __MicroComICompositionSurfaceBrushProxy(p, owns));
        }

        protected __MicroComICompositionSurfaceBrushProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 10;
    }

    unsafe class __MicroComICompositionSurfaceBrushVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetBitmapInterpolationModeDelegate(void* @this, CompositionBitmapInterpolationMode* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetBitmapInterpolationMode(void* @this, CompositionBitmapInterpolationMode* value)
        {
            ICompositionSurfaceBrush __target = null;
            try
            {
                {
                    __target = (ICompositionSurfaceBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.BitmapInterpolationMode;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetBitmapInterpolationModeDelegate(void* @this, CompositionBitmapInterpolationMode value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetBitmapInterpolationMode(void* @this, CompositionBitmapInterpolationMode value)
        {
            ICompositionSurfaceBrush __target = null;
            try
            {
                {
                    __target = (ICompositionSurfaceBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetBitmapInterpolationMode(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetHorizontalAlignmentRatioDelegate(void* @this, float* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetHorizontalAlignmentRatio(void* @this, float* value)
        {
            ICompositionSurfaceBrush __target = null;
            try
            {
                {
                    __target = (ICompositionSurfaceBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.HorizontalAlignmentRatio;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetHorizontalAlignmentRatioDelegate(void* @this, float value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetHorizontalAlignmentRatio(void* @this, float value)
        {
            ICompositionSurfaceBrush __target = null;
            try
            {
                {
                    __target = (ICompositionSurfaceBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetHorizontalAlignmentRatio(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetStretchDelegate(void* @this, CompositionStretch* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetStretch(void* @this, CompositionStretch* value)
        {
            ICompositionSurfaceBrush __target = null;
            try
            {
                {
                    __target = (ICompositionSurfaceBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Stretch;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetStretchDelegate(void* @this, CompositionStretch value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetStretch(void* @this, CompositionStretch value)
        {
            ICompositionSurfaceBrush __target = null;
            try
            {
                {
                    __target = (ICompositionSurfaceBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetStretch(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSurfaceDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSurface(void* @this, void** value)
        {
            ICompositionSurfaceBrush __target = null;
            try
            {
                {
                    __target = (ICompositionSurfaceBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Surface;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetSurfaceDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetSurface(void* @this, void* value)
        {
            ICompositionSurfaceBrush __target = null;
            try
            {
                {
                    __target = (ICompositionSurfaceBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetSurface(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionSurface>(value, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetVerticalAlignmentRatioDelegate(void* @this, float* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetVerticalAlignmentRatio(void* @this, float* value)
        {
            ICompositionSurfaceBrush __target = null;
            try
            {
                {
                    __target = (ICompositionSurfaceBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.VerticalAlignmentRatio;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetVerticalAlignmentRatioDelegate(void* @this, float value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetVerticalAlignmentRatio(void* @this, float value)
        {
            ICompositionSurfaceBrush __target = null;
            try
            {
                {
                    __target = (ICompositionSurfaceBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetVerticalAlignmentRatio(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionSurfaceBrushVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, CompositionBitmapInterpolationMode*, int>)&GetBitmapInterpolationMode); 
#else
            base.AddMethod((GetBitmapInterpolationModeDelegate)GetBitmapInterpolationMode); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, CompositionBitmapInterpolationMode, int>)&SetBitmapInterpolationMode); 
#else
            base.AddMethod((SetBitmapInterpolationModeDelegate)SetBitmapInterpolationMode); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float*, int>)&GetHorizontalAlignmentRatio); 
#else
            base.AddMethod((GetHorizontalAlignmentRatioDelegate)GetHorizontalAlignmentRatio); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float, int>)&SetHorizontalAlignmentRatio); 
#else
            base.AddMethod((SetHorizontalAlignmentRatioDelegate)SetHorizontalAlignmentRatio); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, CompositionStretch*, int>)&GetStretch); 
#else
            base.AddMethod((GetStretchDelegate)GetStretch); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, CompositionStretch, int>)&SetStretch); 
#else
            base.AddMethod((SetStretchDelegate)SetStretch); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetSurface); 
#else
            base.AddMethod((GetSurfaceDelegate)GetSurface); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetSurface); 
#else
            base.AddMethod((SetSurfaceDelegate)SetSurface); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float*, int>)&GetVerticalAlignmentRatio); 
#else
            base.AddMethod((GetVerticalAlignmentRatioDelegate)GetVerticalAlignmentRatio); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float, int>)&SetVerticalAlignmentRatio); 
#else
            base.AddMethod((SetVerticalAlignmentRatioDelegate)SetVerticalAlignmentRatio); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionSurfaceBrush), new __MicroComICompositionSurfaceBrushVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionBrushProxy : __MicroComIInspectableProxy, ICompositionBrush
    {
        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionBrush), new Guid("AB0D7608-30C0-40E9-B568-B60A6BD1FB46"), (p, owns) => new __MicroComICompositionBrushProxy(p, owns));
        }

        protected __MicroComICompositionBrushProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 0;
    }

    unsafe class __MicroComICompositionBrushVTable : __MicroComIInspectableVTable
    {
        protected __MicroComICompositionBrushVTable()
        {
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionBrush), new __MicroComICompositionBrushVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionClipProxy : __MicroComIInspectableProxy, ICompositionClip
    {
        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionClip), new Guid("1CCD2A52-CFC7-4ACE-9983-146BB8EB6A3C"), (p, owns) => new __MicroComICompositionClipProxy(p, owns));
        }

        protected __MicroComICompositionClipProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 0;
    }

    unsafe class __MicroComICompositionClipVTable : __MicroComIInspectableVTable
    {
        protected __MicroComICompositionClipVTable()
        {
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionClip), new __MicroComICompositionClipVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIVisualProxy : __MicroComIInspectableProxy, IVisual
    {
        public System.Numerics.Vector2 AnchorPoint
        {
            get
            {
                int __result;
                System.Numerics.Vector2 value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetAnchorPoint failed", __result);
                return value;
            }
        }

        public void SetAnchorPoint(System.Numerics.Vector2 value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)(*PPV)[base.VTableSize + 1])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetAnchorPoint failed", __result);
        }

        public CompositionBackfaceVisibility BackfaceVisibility
        {
            get
            {
                int __result;
                CompositionBackfaceVisibility value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetBackfaceVisibility failed", __result);
                return value;
            }
        }

        public void SetBackfaceVisibility(CompositionBackfaceVisibility value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, CompositionBackfaceVisibility, int>)(*PPV)[base.VTableSize + 3])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetBackfaceVisibility failed", __result);
        }

        public CompositionBorderMode BorderMode
        {
            get
            {
                int __result;
                CompositionBorderMode value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetBorderMode failed", __result);
                return value;
            }
        }

        public void SetBorderMode(CompositionBorderMode value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, CompositionBorderMode, int>)(*PPV)[base.VTableSize + 5])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetBorderMode failed", __result);
        }

        public System.Numerics.Vector3 CenterPoint
        {
            get
            {
                int __result;
                System.Numerics.Vector3 value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 6])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetCenterPoint failed", __result);
                return value;
            }
        }

        public void SetCenterPoint(System.Numerics.Vector3 value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3, int>)(*PPV)[base.VTableSize + 7])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetCenterPoint failed", __result);
        }

        public ICompositionClip Clip
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 8])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetClip failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionClip>(__marshal_value, true);
            }
        }

        public void SetClip(ICompositionClip value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 9])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(value));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetClip failed", __result);
        }

        public CompositionCompositeMode CompositeMode
        {
            get
            {
                int __result;
                CompositionCompositeMode value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 10])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetCompositeMode failed", __result);
                return value;
            }
        }

        public void SetCompositeMode(CompositionCompositeMode value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, CompositionCompositeMode, int>)(*PPV)[base.VTableSize + 11])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetCompositeMode failed", __result);
        }

        public int IsVisible
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 12])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetIsVisible failed", __result);
                return value;
            }
        }

        public void SetIsVisible(int value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, int>)(*PPV)[base.VTableSize + 13])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetIsVisible failed", __result);
        }

        public System.Numerics.Vector3 Offset
        {
            get
            {
                int __result;
                System.Numerics.Vector3 value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 14])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetOffset failed", __result);
                return value;
            }
        }

        public void SetOffset(System.Numerics.Vector3 value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3, int>)(*PPV)[base.VTableSize + 15])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetOffset failed", __result);
        }

        public float Opacity
        {
            get
            {
                int __result;
                float value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 16])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetOpacity failed", __result);
                return value;
            }
        }

        public void SetOpacity(float value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, float, int>)(*PPV)[base.VTableSize + 17])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetOpacity failed", __result);
        }

        public System.Numerics.Quaternion Orientation
        {
            get
            {
                int __result;
                System.Numerics.Quaternion value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 18])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetOrientation failed", __result);
                return value;
            }
        }

        public void SetOrientation(System.Numerics.Quaternion value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Quaternion, int>)(*PPV)[base.VTableSize + 19])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetOrientation failed", __result);
        }

        public IContainerVisual Parent
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 20])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetParent failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IContainerVisual>(__marshal_value, true);
            }
        }

        public float RotationAngle
        {
            get
            {
                int __result;
                float value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 21])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetRotationAngle failed", __result);
                return value;
            }
        }

        public void SetRotationAngle(float value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, float, int>)(*PPV)[base.VTableSize + 22])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetRotationAngle failed", __result);
        }

        public float RotationAngleInDegrees
        {
            get
            {
                int __result;
                float value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 23])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetRotationAngleInDegrees failed", __result);
                return value;
            }
        }

        public void SetRotationAngleInDegrees(float value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, float, int>)(*PPV)[base.VTableSize + 24])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetRotationAngleInDegrees failed", __result);
        }

        public System.Numerics.Vector3 RotationAxis
        {
            get
            {
                int __result;
                System.Numerics.Vector3 value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 25])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetRotationAxis failed", __result);
                return value;
            }
        }

        public void SetRotationAxis(System.Numerics.Vector3 value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3, int>)(*PPV)[base.VTableSize + 26])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetRotationAxis failed", __result);
        }

        public System.Numerics.Vector3 Scale
        {
            get
            {
                int __result;
                System.Numerics.Vector3 value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 27])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetScale failed", __result);
                return value;
            }
        }

        public void SetScale(System.Numerics.Vector3 value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3, int>)(*PPV)[base.VTableSize + 28])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetScale failed", __result);
        }

        public System.Numerics.Vector2 Size
        {
            get
            {
                int __result;
                System.Numerics.Vector2 value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 29])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetSize failed", __result);
                return value;
            }
        }

        public void SetSize(System.Numerics.Vector2 value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)(*PPV)[base.VTableSize + 30])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetSize failed", __result);
        }

        public System.Numerics.Matrix4x4 TransformMatrix
        {
            get
            {
                int __result;
                System.Numerics.Matrix4x4 value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 31])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetTransformMatrix failed", __result);
                return value;
            }
        }

        public void SetTransformMatrix(System.Numerics.Matrix4x4 value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Matrix4x4, int>)(*PPV)[base.VTableSize + 32])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetTransformMatrix failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IVisual), new Guid("117E202D-A859-4C89-873B-C2AA566788E3"), (p, owns) => new __MicroComIVisualProxy(p, owns));
        }

        protected __MicroComIVisualProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 33;
    }

    unsafe class __MicroComIVisualVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetAnchorPointDelegate(void* @this, System.Numerics.Vector2* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetAnchorPoint(void* @this, System.Numerics.Vector2* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.AnchorPoint;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetAnchorPointDelegate(void* @this, System.Numerics.Vector2 value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetAnchorPoint(void* @this, System.Numerics.Vector2 value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetAnchorPoint(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetBackfaceVisibilityDelegate(void* @this, CompositionBackfaceVisibility* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetBackfaceVisibility(void* @this, CompositionBackfaceVisibility* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.BackfaceVisibility;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetBackfaceVisibilityDelegate(void* @this, CompositionBackfaceVisibility value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetBackfaceVisibility(void* @this, CompositionBackfaceVisibility value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetBackfaceVisibility(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetBorderModeDelegate(void* @this, CompositionBorderMode* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetBorderMode(void* @this, CompositionBorderMode* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.BorderMode;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetBorderModeDelegate(void* @this, CompositionBorderMode value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetBorderMode(void* @this, CompositionBorderMode value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetBorderMode(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetCenterPointDelegate(void* @this, System.Numerics.Vector3* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCenterPoint(void* @this, System.Numerics.Vector3* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CenterPoint;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetCenterPointDelegate(void* @this, System.Numerics.Vector3 value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetCenterPoint(void* @this, System.Numerics.Vector3 value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetCenterPoint(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetClipDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetClip(void* @this, void** value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Clip;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetClipDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetClip(void* @this, void* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetClip(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionClip>(value, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetCompositeModeDelegate(void* @this, CompositionCompositeMode* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCompositeMode(void* @this, CompositionCompositeMode* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CompositeMode;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetCompositeModeDelegate(void* @this, CompositionCompositeMode value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetCompositeMode(void* @this, CompositionCompositeMode value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetCompositeMode(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetIsVisibleDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetIsVisible(void* @this, int* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.IsVisible;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetIsVisibleDelegate(void* @this, int value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetIsVisible(void* @this, int value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetIsVisible(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetOffsetDelegate(void* @this, System.Numerics.Vector3* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetOffset(void* @this, System.Numerics.Vector3* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Offset;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetOffsetDelegate(void* @this, System.Numerics.Vector3 value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetOffset(void* @this, System.Numerics.Vector3 value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetOffset(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetOpacityDelegate(void* @this, float* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetOpacity(void* @this, float* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Opacity;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetOpacityDelegate(void* @this, float value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetOpacity(void* @this, float value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetOpacity(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetOrientationDelegate(void* @this, System.Numerics.Quaternion* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetOrientation(void* @this, System.Numerics.Quaternion* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Orientation;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetOrientationDelegate(void* @this, System.Numerics.Quaternion value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetOrientation(void* @this, System.Numerics.Quaternion value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetOrientation(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetParentDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetParent(void* @this, void** value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Parent;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetRotationAngleDelegate(void* @this, float* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetRotationAngle(void* @this, float* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.RotationAngle;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetRotationAngleDelegate(void* @this, float value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetRotationAngle(void* @this, float value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetRotationAngle(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetRotationAngleInDegreesDelegate(void* @this, float* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetRotationAngleInDegrees(void* @this, float* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.RotationAngleInDegrees;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetRotationAngleInDegreesDelegate(void* @this, float value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetRotationAngleInDegrees(void* @this, float value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetRotationAngleInDegrees(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetRotationAxisDelegate(void* @this, System.Numerics.Vector3* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetRotationAxis(void* @this, System.Numerics.Vector3* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.RotationAxis;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetRotationAxisDelegate(void* @this, System.Numerics.Vector3 value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetRotationAxis(void* @this, System.Numerics.Vector3 value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetRotationAxis(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetScaleDelegate(void* @this, System.Numerics.Vector3* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetScale(void* @this, System.Numerics.Vector3* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Scale;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetScaleDelegate(void* @this, System.Numerics.Vector3 value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetScale(void* @this, System.Numerics.Vector3 value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetScale(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSizeDelegate(void* @this, System.Numerics.Vector2* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSize(void* @this, System.Numerics.Vector2* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Size;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetSizeDelegate(void* @this, System.Numerics.Vector2 value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetSize(void* @this, System.Numerics.Vector2 value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetSize(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetTransformMatrixDelegate(void* @this, System.Numerics.Matrix4x4* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetTransformMatrix(void* @this, System.Numerics.Matrix4x4* value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.TransformMatrix;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetTransformMatrixDelegate(void* @this, System.Numerics.Matrix4x4 value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetTransformMatrix(void* @this, System.Numerics.Matrix4x4 value)
        {
            IVisual __target = null;
            try
            {
                {
                    __target = (IVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetTransformMatrix(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIVisualVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2*, int>)&GetAnchorPoint); 
#else
            base.AddMethod((GetAnchorPointDelegate)GetAnchorPoint); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)&SetAnchorPoint); 
#else
            base.AddMethod((SetAnchorPointDelegate)SetAnchorPoint); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, CompositionBackfaceVisibility*, int>)&GetBackfaceVisibility); 
#else
            base.AddMethod((GetBackfaceVisibilityDelegate)GetBackfaceVisibility); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, CompositionBackfaceVisibility, int>)&SetBackfaceVisibility); 
#else
            base.AddMethod((SetBackfaceVisibilityDelegate)SetBackfaceVisibility); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, CompositionBorderMode*, int>)&GetBorderMode); 
#else
            base.AddMethod((GetBorderModeDelegate)GetBorderMode); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, CompositionBorderMode, int>)&SetBorderMode); 
#else
            base.AddMethod((SetBorderModeDelegate)SetBorderMode); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3*, int>)&GetCenterPoint); 
#else
            base.AddMethod((GetCenterPointDelegate)GetCenterPoint); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3, int>)&SetCenterPoint); 
#else
            base.AddMethod((SetCenterPointDelegate)SetCenterPoint); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetClip); 
#else
            base.AddMethod((GetClipDelegate)GetClip); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetClip); 
#else
            base.AddMethod((SetClipDelegate)SetClip); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, CompositionCompositeMode*, int>)&GetCompositeMode); 
#else
            base.AddMethod((GetCompositeModeDelegate)GetCompositeMode); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, CompositionCompositeMode, int>)&SetCompositeMode); 
#else
            base.AddMethod((SetCompositeModeDelegate)SetCompositeMode); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetIsVisible); 
#else
            base.AddMethod((GetIsVisibleDelegate)GetIsVisible); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, int>)&SetIsVisible); 
#else
            base.AddMethod((SetIsVisibleDelegate)SetIsVisible); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3*, int>)&GetOffset); 
#else
            base.AddMethod((GetOffsetDelegate)GetOffset); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3, int>)&SetOffset); 
#else
            base.AddMethod((SetOffsetDelegate)SetOffset); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float*, int>)&GetOpacity); 
#else
            base.AddMethod((GetOpacityDelegate)GetOpacity); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float, int>)&SetOpacity); 
#else
            base.AddMethod((SetOpacityDelegate)SetOpacity); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Quaternion*, int>)&GetOrientation); 
#else
            base.AddMethod((GetOrientationDelegate)GetOrientation); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Quaternion, int>)&SetOrientation); 
#else
            base.AddMethod((SetOrientationDelegate)SetOrientation); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetParent); 
#else
            base.AddMethod((GetParentDelegate)GetParent); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float*, int>)&GetRotationAngle); 
#else
            base.AddMethod((GetRotationAngleDelegate)GetRotationAngle); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float, int>)&SetRotationAngle); 
#else
            base.AddMethod((SetRotationAngleDelegate)SetRotationAngle); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float*, int>)&GetRotationAngleInDegrees); 
#else
            base.AddMethod((GetRotationAngleInDegreesDelegate)GetRotationAngleInDegrees); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float, int>)&SetRotationAngleInDegrees); 
#else
            base.AddMethod((SetRotationAngleInDegreesDelegate)SetRotationAngleInDegrees); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3*, int>)&GetRotationAxis); 
#else
            base.AddMethod((GetRotationAxisDelegate)GetRotationAxis); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3, int>)&SetRotationAxis); 
#else
            base.AddMethod((SetRotationAxisDelegate)SetRotationAxis); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3*, int>)&GetScale); 
#else
            base.AddMethod((GetScaleDelegate)GetScale); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3, int>)&SetScale); 
#else
            base.AddMethod((SetScaleDelegate)SetScale); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2*, int>)&GetSize); 
#else
            base.AddMethod((GetSizeDelegate)GetSize); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)&SetSize); 
#else
            base.AddMethod((SetSizeDelegate)SetSize); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Matrix4x4*, int>)&GetTransformMatrix); 
#else
            base.AddMethod((GetTransformMatrixDelegate)GetTransformMatrix); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Matrix4x4, int>)&SetTransformMatrix); 
#else
            base.AddMethod((SetTransformMatrixDelegate)SetTransformMatrix); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IVisual), new __MicroComIVisualVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIVisual2Proxy : __MicroComIInspectableProxy, IVisual2
    {
        public IVisual ParentForTransform
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetParentForTransform failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IVisual>(__marshal_value, true);
            }
        }

        public void SetParentForTransform(IVisual value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(value));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetParentForTransform failed", __result);
        }

        public System.Numerics.Vector3 RelativeOffsetAdjustment
        {
            get
            {
                int __result;
                System.Numerics.Vector3 value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetRelativeOffsetAdjustment failed", __result);
                return value;
            }
        }

        public void SetRelativeOffsetAdjustment(System.Numerics.Vector3 value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3, int>)(*PPV)[base.VTableSize + 3])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetRelativeOffsetAdjustment failed", __result);
        }

        public System.Numerics.Vector2 RelativeSizeAdjustment
        {
            get
            {
                int __result;
                System.Numerics.Vector2 value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetRelativeSizeAdjustment failed", __result);
                return value;
            }
        }

        public void SetRelativeSizeAdjustment(System.Numerics.Vector2 value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)(*PPV)[base.VTableSize + 5])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetRelativeSizeAdjustment failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IVisual2), new Guid("3052B611-56C3-4C3E-8BF3-F6E1AD473F06"), (p, owns) => new __MicroComIVisual2Proxy(p, owns));
        }

        protected __MicroComIVisual2Proxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 6;
    }

    unsafe class __MicroComIVisual2VTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetParentForTransformDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetParentForTransform(void* @this, void** value)
        {
            IVisual2 __target = null;
            try
            {
                {
                    __target = (IVisual2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.ParentForTransform;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetParentForTransformDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetParentForTransform(void* @this, void* value)
        {
            IVisual2 __target = null;
            try
            {
                {
                    __target = (IVisual2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetParentForTransform(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IVisual>(value, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetRelativeOffsetAdjustmentDelegate(void* @this, System.Numerics.Vector3* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetRelativeOffsetAdjustment(void* @this, System.Numerics.Vector3* value)
        {
            IVisual2 __target = null;
            try
            {
                {
                    __target = (IVisual2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.RelativeOffsetAdjustment;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetRelativeOffsetAdjustmentDelegate(void* @this, System.Numerics.Vector3 value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetRelativeOffsetAdjustment(void* @this, System.Numerics.Vector3 value)
        {
            IVisual2 __target = null;
            try
            {
                {
                    __target = (IVisual2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetRelativeOffsetAdjustment(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetRelativeSizeAdjustmentDelegate(void* @this, System.Numerics.Vector2* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetRelativeSizeAdjustment(void* @this, System.Numerics.Vector2* value)
        {
            IVisual2 __target = null;
            try
            {
                {
                    __target = (IVisual2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.RelativeSizeAdjustment;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetRelativeSizeAdjustmentDelegate(void* @this, System.Numerics.Vector2 value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetRelativeSizeAdjustment(void* @this, System.Numerics.Vector2 value)
        {
            IVisual2 __target = null;
            try
            {
                {
                    __target = (IVisual2)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetRelativeSizeAdjustment(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIVisual2VTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetParentForTransform); 
#else
            base.AddMethod((GetParentForTransformDelegate)GetParentForTransform); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetParentForTransform); 
#else
            base.AddMethod((SetParentForTransformDelegate)SetParentForTransform); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3*, int>)&GetRelativeOffsetAdjustment); 
#else
            base.AddMethod((GetRelativeOffsetAdjustmentDelegate)GetRelativeOffsetAdjustment); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector3, int>)&SetRelativeOffsetAdjustment); 
#else
            base.AddMethod((SetRelativeOffsetAdjustmentDelegate)SetRelativeOffsetAdjustment); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2*, int>)&GetRelativeSizeAdjustment); 
#else
            base.AddMethod((GetRelativeSizeAdjustmentDelegate)GetRelativeSizeAdjustment); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)&SetRelativeSizeAdjustment); 
#else
            base.AddMethod((SetRelativeSizeAdjustmentDelegate)SetRelativeSizeAdjustment); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IVisual2), new __MicroComIVisual2VTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIContainerVisualProxy : __MicroComIInspectableProxy, IContainerVisual
    {
        public IVisualCollection Children
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetChildren failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IVisualCollection>(__marshal_value, true);
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IContainerVisual), new Guid("02F6BC74-ED20-4773-AFE6-D49B4A93DB32"), (p, owns) => new __MicroComIContainerVisualProxy(p, owns));
        }

        protected __MicroComIContainerVisualProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIContainerVisualVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetChildrenDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetChildren(void* @this, void** value)
        {
            IContainerVisual __target = null;
            try
            {
                {
                    __target = (IContainerVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Children;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIContainerVisualVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetChildren); 
#else
            base.AddMethod((GetChildrenDelegate)GetChildren); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IContainerVisual), new __MicroComIContainerVisualVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIVisualCollectionProxy : __MicroComIInspectableProxy, IVisualCollection
    {
        public int Count
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetCount failed", __result);
                return value;
            }
        }

        public void InsertAbove(IVisual newChild, IVisual sibling)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(newChild), global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(sibling));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("InsertAbove failed", __result);
        }

        public void InsertAtBottom(IVisual newChild)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(newChild));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("InsertAtBottom failed", __result);
        }

        public void InsertAtTop(IVisual newChild)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(newChild));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("InsertAtTop failed", __result);
        }

        public void InsertBelow(IVisual newChild, IVisual sibling)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(newChild), global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(sibling));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("InsertBelow failed", __result);
        }

        public void Remove(IVisual child)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 5])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(child));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Remove failed", __result);
        }

        public void RemoveAll()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 6])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RemoveAll failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IVisualCollection), new Guid("8B745505-FD3E-4A98-84A8-E949468C6BCB"), (p, owns) => new __MicroComIVisualCollectionProxy(p, owns));
        }

        protected __MicroComIVisualCollectionProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 7;
    }

    unsafe class __MicroComIVisualCollectionVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetCountDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCount(void* @this, int* value)
        {
            IVisualCollection __target = null;
            try
            {
                {
                    __target = (IVisualCollection)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Count;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int InsertAboveDelegate(void* @this, void* newChild, void* sibling);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int InsertAbove(void* @this, void* newChild, void* sibling)
        {
            IVisualCollection __target = null;
            try
            {
                {
                    __target = (IVisualCollection)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.InsertAbove(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IVisual>(newChild, false), global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IVisual>(sibling, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int InsertAtBottomDelegate(void* @this, void* newChild);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int InsertAtBottom(void* @this, void* newChild)
        {
            IVisualCollection __target = null;
            try
            {
                {
                    __target = (IVisualCollection)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.InsertAtBottom(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IVisual>(newChild, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int InsertAtTopDelegate(void* @this, void* newChild);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int InsertAtTop(void* @this, void* newChild)
        {
            IVisualCollection __target = null;
            try
            {
                {
                    __target = (IVisualCollection)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.InsertAtTop(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IVisual>(newChild, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int InsertBelowDelegate(void* @this, void* newChild, void* sibling);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int InsertBelow(void* @this, void* newChild, void* sibling)
        {
            IVisualCollection __target = null;
            try
            {
                {
                    __target = (IVisualCollection)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.InsertBelow(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IVisual>(newChild, false), global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IVisual>(sibling, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveDelegate(void* @this, void* child);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Remove(void* @this, void* child)
        {
            IVisualCollection __target = null;
            try
            {
                {
                    __target = (IVisualCollection)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Remove(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IVisual>(child, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveAllDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RemoveAll(void* @this)
        {
            IVisualCollection __target = null;
            try
            {
                {
                    __target = (IVisualCollection)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RemoveAll();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIVisualCollectionVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetCount); 
#else
            base.AddMethod((GetCountDelegate)GetCount); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)&InsertAbove); 
#else
            base.AddMethod((InsertAboveDelegate)InsertAbove); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&InsertAtBottom); 
#else
            base.AddMethod((InsertAtBottomDelegate)InsertAtBottom); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&InsertAtTop); 
#else
            base.AddMethod((InsertAtTopDelegate)InsertAtTop); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)&InsertBelow); 
#else
            base.AddMethod((InsertBelowDelegate)InsertBelow); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&Remove); 
#else
            base.AddMethod((RemoveDelegate)Remove); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&RemoveAll); 
#else
            base.AddMethod((RemoveAllDelegate)RemoveAll); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IVisualCollection), new __MicroComIVisualCollectionVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionTargetProxy : __MicroComIInspectableProxy, ICompositionTarget
    {
        public IVisual Root
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetRoot failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IVisual>(__marshal_value, true);
            }
        }

        public void SetRoot(IVisual value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(value));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetRoot failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionTarget), new Guid("A1BEA8BA-D726-4663-8129-6B5E7927FFA6"), (p, owns) => new __MicroComICompositionTargetProxy(p, owns));
        }

        protected __MicroComICompositionTargetProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComICompositionTargetVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetRootDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetRoot(void* @this, void** value)
        {
            ICompositionTarget __target = null;
            try
            {
                {
                    __target = (ICompositionTarget)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Root;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetRootDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetRoot(void* @this, void* value)
        {
            ICompositionTarget __target = null;
            try
            {
                {
                    __target = (ICompositionTarget)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetRoot(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IVisual>(value, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionTargetVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetRoot); 
#else
            base.AddMethod((GetRootDelegate)GetRoot); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetRoot); 
#else
            base.AddMethod((SetRootDelegate)SetRoot); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionTarget), new __MicroComICompositionTargetVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIGraphicsEffectProxy : __MicroComIInspectableProxy, IGraphicsEffect
    {
        public IntPtr Name
        {
            get
            {
                int __result;
                IntPtr name = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &name);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetName failed", __result);
                return name;
            }
        }

        public void SetName(IntPtr name)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 1])(PPV, name);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetName failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IGraphicsEffect), new Guid("CB51C0CE-8FE6-4636-B202-861FAA07D8F3"), (p, owns) => new __MicroComIGraphicsEffectProxy(p, owns));
        }

        protected __MicroComIGraphicsEffectProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComIGraphicsEffectVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetNameDelegate(void* @this, IntPtr* name);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetName(void* @this, IntPtr* name)
        {
            IGraphicsEffect __target = null;
            try
            {
                {
                    __target = (IGraphicsEffect)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Name;
                        *name = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetNameDelegate(void* @this, IntPtr name);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetName(void* @this, IntPtr name)
        {
            IGraphicsEffect __target = null;
            try
            {
                {
                    __target = (IGraphicsEffect)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetName(name);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIGraphicsEffectVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetName); 
#else
            base.AddMethod((GetNameDelegate)GetName); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&SetName); 
#else
            base.AddMethod((SetNameDelegate)SetName); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IGraphicsEffect), new __MicroComIGraphicsEffectVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIGraphicsEffectSourceProxy : __MicroComIInspectableProxy, IGraphicsEffectSource
    {
        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IGraphicsEffectSource), new Guid("2D8F9DDC-4339-4EB9-9216-F9DEB75658A2"), (p, owns) => new __MicroComIGraphicsEffectSourceProxy(p, owns));
        }

        protected __MicroComIGraphicsEffectSourceProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 0;
    }

    unsafe class __MicroComIGraphicsEffectSourceVTable : __MicroComIInspectableVTable
    {
        protected __MicroComIGraphicsEffectSourceVTable()
        {
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IGraphicsEffectSource), new __MicroComIGraphicsEffectSourceVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIGraphicsEffectD2D1InteropProxy : global::Avalonia.MicroCom.MicroComProxyBase, IGraphicsEffectD2D1Interop
    {
        public Guid EffectId
        {
            get
            {
                int __result;
                Guid id = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &id);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetEffectId failed", __result);
                return id;
            }
        }

        public void GetNamedPropertyMapping(IntPtr name, uint* index, GRAPHICS_EFFECT_PROPERTY_MAPPING* mapping)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, name, index, mapping);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetNamedPropertyMapping failed", __result);
        }

        public uint PropertyCount
        {
            get
            {
                int __result;
                uint count = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &count);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetPropertyCount failed", __result);
                return count;
            }
        }

        public IPropertyValue GetProperty(uint index)
        {
            int __result;
            void* __marshal_value = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, uint, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, index, &__marshal_value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetProperty failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IPropertyValue>(__marshal_value, true);
        }

        public IGraphicsEffectSource GetSource(uint index)
        {
            int __result;
            void* __marshal_source = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, uint, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, index, &__marshal_source);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetSource failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IGraphicsEffectSource>(__marshal_source, true);
        }

        public uint SourceCount
        {
            get
            {
                int __result;
                uint count = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 5])(PPV, &count);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetSourceCount failed", __result);
                return count;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IGraphicsEffectD2D1Interop), new Guid("2FC57384-A068-44D7-A331-30982FCF7177"), (p, owns) => new __MicroComIGraphicsEffectD2D1InteropProxy(p, owns));
        }

        protected __MicroComIGraphicsEffectD2D1InteropProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 6;
    }

    unsafe class __MicroComIGraphicsEffectD2D1InteropVTable : global::Avalonia.MicroCom.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetEffectIdDelegate(void* @this, Guid* id);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetEffectId(void* @this, Guid* id)
        {
            IGraphicsEffectD2D1Interop __target = null;
            try
            {
                {
                    __target = (IGraphicsEffectD2D1Interop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.EffectId;
                        *id = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetNamedPropertyMappingDelegate(void* @this, IntPtr name, uint* index, GRAPHICS_EFFECT_PROPERTY_MAPPING* mapping);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetNamedPropertyMapping(void* @this, IntPtr name, uint* index, GRAPHICS_EFFECT_PROPERTY_MAPPING* mapping)
        {
            IGraphicsEffectD2D1Interop __target = null;
            try
            {
                {
                    __target = (IGraphicsEffectD2D1Interop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetNamedPropertyMapping(name, index, mapping);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetPropertyCountDelegate(void* @this, uint* count);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetPropertyCount(void* @this, uint* count)
        {
            IGraphicsEffectD2D1Interop __target = null;
            try
            {
                {
                    __target = (IGraphicsEffectD2D1Interop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.PropertyCount;
                        *count = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetPropertyDelegate(void* @this, uint index, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetProperty(void* @this, uint index, void** value)
        {
            IGraphicsEffectD2D1Interop __target = null;
            try
            {
                {
                    __target = (IGraphicsEffectD2D1Interop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetProperty(index);
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSourceDelegate(void* @this, uint index, void** source);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSource(void* @this, uint index, void** source)
        {
            IGraphicsEffectD2D1Interop __target = null;
            try
            {
                {
                    __target = (IGraphicsEffectD2D1Interop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetSource(index);
                        *source = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSourceCountDelegate(void* @this, uint* count);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSourceCount(void* @this, uint* count)
        {
            IGraphicsEffectD2D1Interop __target = null;
            try
            {
                {
                    __target = (IGraphicsEffectD2D1Interop)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.SourceCount;
                        *count = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIGraphicsEffectD2D1InteropVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, Guid*, int>)&GetEffectId); 
#else
            base.AddMethod((GetEffectIdDelegate)GetEffectId); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, uint*, GRAPHICS_EFFECT_PROPERTY_MAPPING*, int>)&GetNamedPropertyMapping); 
#else
            base.AddMethod((GetNamedPropertyMappingDelegate)GetNamedPropertyMapping); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int>)&GetPropertyCount); 
#else
            base.AddMethod((GetPropertyCountDelegate)GetPropertyCount); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint, void**, int>)&GetProperty); 
#else
            base.AddMethod((GetPropertyDelegate)GetProperty); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint, void**, int>)&GetSource); 
#else
            base.AddMethod((GetSourceDelegate)GetSource); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int>)&GetSourceCount); 
#else
            base.AddMethod((GetSourceCountDelegate)GetSourceCount); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IGraphicsEffectD2D1Interop), new __MicroComIGraphicsEffectD2D1InteropVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionEffectSourceParameterProxy : __MicroComIInspectableProxy, ICompositionEffectSourceParameter
    {
        public IntPtr Name
        {
            get
            {
                int __result;
                IntPtr value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetName failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionEffectSourceParameter), new Guid("858AB13A-3292-4E4E-B3BB-2B6C6544A6EE"), (p, owns) => new __MicroComICompositionEffectSourceParameterProxy(p, owns));
        }

        protected __MicroComICompositionEffectSourceParameterProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComICompositionEffectSourceParameterVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetNameDelegate(void* @this, IntPtr* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetName(void* @this, IntPtr* value)
        {
            ICompositionEffectSourceParameter __target = null;
            try
            {
                {
                    __target = (ICompositionEffectSourceParameter)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Name;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionEffectSourceParameterVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetName); 
#else
            base.AddMethod((GetNameDelegate)GetName); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionEffectSourceParameter), new __MicroComICompositionEffectSourceParameterVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionEffectSourceParameterFactoryProxy : __MicroComIInspectableProxy, ICompositionEffectSourceParameterFactory
    {
        public ICompositionEffectSourceParameter Create(IntPtr name)
        {
            int __result;
            void* __marshal_instance = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, name, &__marshal_instance);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Create failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionEffectSourceParameter>(__marshal_instance, true);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionEffectSourceParameterFactory), new Guid("B3D9F276-ABA3-4724-ACF3-D0397464DB1C"), (p, owns) => new __MicroComICompositionEffectSourceParameterFactoryProxy(p, owns));
        }

        protected __MicroComICompositionEffectSourceParameterFactoryProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComICompositionEffectSourceParameterFactoryVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateDelegate(void* @this, IntPtr name, void** instance);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Create(void* @this, IntPtr name, void** instance)
        {
            ICompositionEffectSourceParameterFactory __target = null;
            try
            {
                {
                    __target = (ICompositionEffectSourceParameterFactory)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Create(name);
                        *instance = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionEffectSourceParameterFactoryVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, void**, int>)&Create); 
#else
            base.AddMethod((CreateDelegate)Create); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionEffectSourceParameterFactory), new __MicroComICompositionEffectSourceParameterFactoryVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionEffectFactoryProxy : __MicroComIInspectableProxy, ICompositionEffectFactory
    {
        public ICompositionEffectBrush CreateBrush()
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateBrush failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionEffectBrush>(__marshal_result, true);
        }

        public int ExtendedError
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetExtendedError failed", __result);
                return value;
            }
        }

        public CompositionEffectFactoryLoadStatus LoadStatus
        {
            get
            {
                int __result;
                CompositionEffectFactoryLoadStatus value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetLoadStatus failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionEffectFactory), new Guid("BE5624AF-BA7E-4510-9850-41C0B4FF74DF"), (p, owns) => new __MicroComICompositionEffectFactoryProxy(p, owns));
        }

        protected __MicroComICompositionEffectFactoryProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 3;
    }

    unsafe class __MicroComICompositionEffectFactoryVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateBrushDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateBrush(void* @this, void** result)
        {
            ICompositionEffectFactory __target = null;
            try
            {
                {
                    __target = (ICompositionEffectFactory)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateBrush();
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetExtendedErrorDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetExtendedError(void* @this, int* value)
        {
            ICompositionEffectFactory __target = null;
            try
            {
                {
                    __target = (ICompositionEffectFactory)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.ExtendedError;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetLoadStatusDelegate(void* @this, CompositionEffectFactoryLoadStatus* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetLoadStatus(void* @this, CompositionEffectFactoryLoadStatus* value)
        {
            ICompositionEffectFactory __target = null;
            try
            {
                {
                    __target = (ICompositionEffectFactory)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.LoadStatus;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionEffectFactoryVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateBrush); 
#else
            base.AddMethod((CreateBrushDelegate)CreateBrush); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetExtendedError); 
#else
            base.AddMethod((GetExtendedErrorDelegate)GetExtendedError); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, CompositionEffectFactoryLoadStatus*, int>)&GetLoadStatus); 
#else
            base.AddMethod((GetLoadStatusDelegate)GetLoadStatus); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionEffectFactory), new __MicroComICompositionEffectFactoryVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionEffectBrushProxy : __MicroComIInspectableProxy, ICompositionEffectBrush
    {
        public ICompositionBrush GetSourceParameter(IntPtr name)
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, name, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetSourceParameter failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(__marshal_result, true);
        }

        public void SetSourceParameter(IntPtr name, ICompositionBrush source)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, name, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(source));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetSourceParameter failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionEffectBrush), new Guid("BF7F795E-83CC-44BF-A447-3E3C071789EC"), (p, owns) => new __MicroComICompositionEffectBrushProxy(p, owns));
        }

        protected __MicroComICompositionEffectBrushProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComICompositionEffectBrushVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSourceParameterDelegate(void* @this, IntPtr name, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSourceParameter(void* @this, IntPtr name, void** result)
        {
            ICompositionEffectBrush __target = null;
            try
            {
                {
                    __target = (ICompositionEffectBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetSourceParameter(name);
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetSourceParameterDelegate(void* @this, IntPtr name, void* source);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetSourceParameter(void* @this, IntPtr name, void* source)
        {
            ICompositionEffectBrush __target = null;
            try
            {
                {
                    __target = (ICompositionEffectBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetSourceParameter(name, global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(source, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionEffectBrushVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, void**, int>)&GetSourceParameter); 
#else
            base.AddMethod((GetSourceParameterDelegate)GetSourceParameter); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)&SetSourceParameter); 
#else
            base.AddMethod((SetSourceParameterDelegate)SetSourceParameter); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionEffectBrush), new __MicroComICompositionEffectBrushVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionBackdropBrushProxy : __MicroComIInspectableProxy, ICompositionBackdropBrush
    {
        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionBackdropBrush), new Guid("C5ACAE58-3898-499E-8D7F-224E91286A5D"), (p, owns) => new __MicroComICompositionBackdropBrushProxy(p, owns));
        }

        protected __MicroComICompositionBackdropBrushProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 0;
    }

    unsafe class __MicroComICompositionBackdropBrushVTable : __MicroComIInspectableVTable
    {
        protected __MicroComICompositionBackdropBrushVTable()
        {
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionBackdropBrush), new __MicroComICompositionBackdropBrushVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionColorBrushProxy : __MicroComIInspectableProxy, ICompositionColorBrush
    {
        public Avalonia.Win32.WinRT.WinRTColor Color
        {
            get
            {
                int __result;
                Avalonia.Win32.WinRT.WinRTColor value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetColor failed", __result);
                return value;
            }
        }

        public void SetColor(Avalonia.Win32.WinRT.WinRTColor value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, Avalonia.Win32.WinRT.WinRTColor, int>)(*PPV)[base.VTableSize + 1])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetColor failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionColorBrush), new Guid("2B264C5E-BF35-4831-8642-CF70C20FFF2F"), (p, owns) => new __MicroComICompositionColorBrushProxy(p, owns));
        }

        protected __MicroComICompositionColorBrushProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComICompositionColorBrushVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetColorDelegate(void* @this, Avalonia.Win32.WinRT.WinRTColor* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetColor(void* @this, Avalonia.Win32.WinRT.WinRTColor* value)
        {
            ICompositionColorBrush __target = null;
            try
            {
                {
                    __target = (ICompositionColorBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Color;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetColorDelegate(void* @this, Avalonia.Win32.WinRT.WinRTColor value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetColor(void* @this, Avalonia.Win32.WinRT.WinRTColor value)
        {
            ICompositionColorBrush __target = null;
            try
            {
                {
                    __target = (ICompositionColorBrush)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetColor(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionColorBrushVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, Avalonia.Win32.WinRT.WinRTColor*, int>)&GetColor); 
#else
            base.AddMethod((GetColorDelegate)GetColor); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, Avalonia.Win32.WinRT.WinRTColor, int>)&SetColor); 
#else
            base.AddMethod((SetColorDelegate)SetColor); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionColorBrush), new __MicroComICompositionColorBrushVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionScopedBatchProxy : __MicroComIInspectableProxy, ICompositionScopedBatch
    {
        public int IsActive
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetIsActive failed", __result);
                return value;
            }
        }

        public int IsEnded
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetIsEnded failed", __result);
                return value;
            }
        }

        public void End()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 2])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("End failed", __result);
        }

        public void Resume()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 3])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Resume failed", __result);
        }

        public void Suspend()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 4])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Suspend failed", __result);
        }

        public int AddCompleted(void* handler)
        {
            int __result;
            int token = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 5])(PPV, handler, &token);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("AddCompleted failed", __result);
            return token;
        }

        public void RemoveCompleted(int token)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, int>)(*PPV)[base.VTableSize + 6])(PPV, token);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RemoveCompleted failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionScopedBatch), new Guid("0D00DAD0-FB07-46FD-8C72-6280D1A3D1DD"), (p, owns) => new __MicroComICompositionScopedBatchProxy(p, owns));
        }

        protected __MicroComICompositionScopedBatchProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 7;
    }

    unsafe class __MicroComICompositionScopedBatchVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetIsActiveDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetIsActive(void* @this, int* value)
        {
            ICompositionScopedBatch __target = null;
            try
            {
                {
                    __target = (ICompositionScopedBatch)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.IsActive;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetIsEndedDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetIsEnded(void* @this, int* value)
        {
            ICompositionScopedBatch __target = null;
            try
            {
                {
                    __target = (ICompositionScopedBatch)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.IsEnded;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int EndDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int End(void* @this)
        {
            ICompositionScopedBatch __target = null;
            try
            {
                {
                    __target = (ICompositionScopedBatch)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.End();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ResumeDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Resume(void* @this)
        {
            ICompositionScopedBatch __target = null;
            try
            {
                {
                    __target = (ICompositionScopedBatch)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Resume();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SuspendDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Suspend(void* @this)
        {
            ICompositionScopedBatch __target = null;
            try
            {
                {
                    __target = (ICompositionScopedBatch)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Suspend();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int AddCompletedDelegate(void* @this, void* handler, int* token);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int AddCompleted(void* @this, void* handler, int* token)
        {
            ICompositionScopedBatch __target = null;
            try
            {
                {
                    __target = (ICompositionScopedBatch)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.AddCompleted(handler);
                        *token = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveCompletedDelegate(void* @this, int token);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RemoveCompleted(void* @this, int token)
        {
            ICompositionScopedBatch __target = null;
            try
            {
                {
                    __target = (ICompositionScopedBatch)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RemoveCompleted(token);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionScopedBatchVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetIsActive); 
#else
            base.AddMethod((GetIsActiveDelegate)GetIsActive); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetIsEnded); 
#else
            base.AddMethod((GetIsEndedDelegate)GetIsEnded); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&End); 
#else
            base.AddMethod((EndDelegate)End); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&Resume); 
#else
            base.AddMethod((ResumeDelegate)Resume); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&Suspend); 
#else
            base.AddMethod((SuspendDelegate)Suspend); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int*, int>)&AddCompleted); 
#else
            base.AddMethod((AddCompletedDelegate)AddCompleted); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, int>)&RemoveCompleted); 
#else
            base.AddMethod((RemoveCompletedDelegate)RemoveCompleted); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionScopedBatch), new __MicroComICompositionScopedBatchVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionRoundedRectangleGeometryProxy : __MicroComIInspectableProxy, ICompositionRoundedRectangleGeometry
    {
        public System.Numerics.Vector2 CornerRadius
        {
            get
            {
                int __result;
                System.Numerics.Vector2 value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetCornerRadius failed", __result);
                return value;
            }
        }

        public void SetCornerRadius(System.Numerics.Vector2 value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)(*PPV)[base.VTableSize + 1])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetCornerRadius failed", __result);
        }

        public System.Numerics.Vector2 Offset
        {
            get
            {
                int __result;
                System.Numerics.Vector2 value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetOffset failed", __result);
                return value;
            }
        }

        public void SetOffset(System.Numerics.Vector2 value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)(*PPV)[base.VTableSize + 3])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetOffset failed", __result);
        }

        public System.Numerics.Vector2 Size
        {
            get
            {
                int __result;
                System.Numerics.Vector2 value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetSize failed", __result);
                return value;
            }
        }

        public void SetSize(System.Numerics.Vector2 value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)(*PPV)[base.VTableSize + 5])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetSize failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionRoundedRectangleGeometry), new Guid("8770C822-1D50-4B8B-B013-7C9A0E46935F"), (p, owns) => new __MicroComICompositionRoundedRectangleGeometryProxy(p, owns));
        }

        protected __MicroComICompositionRoundedRectangleGeometryProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 6;
    }

    unsafe class __MicroComICompositionRoundedRectangleGeometryVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetCornerRadiusDelegate(void* @this, System.Numerics.Vector2* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCornerRadius(void* @this, System.Numerics.Vector2* value)
        {
            ICompositionRoundedRectangleGeometry __target = null;
            try
            {
                {
                    __target = (ICompositionRoundedRectangleGeometry)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CornerRadius;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetCornerRadiusDelegate(void* @this, System.Numerics.Vector2 value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetCornerRadius(void* @this, System.Numerics.Vector2 value)
        {
            ICompositionRoundedRectangleGeometry __target = null;
            try
            {
                {
                    __target = (ICompositionRoundedRectangleGeometry)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetCornerRadius(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetOffsetDelegate(void* @this, System.Numerics.Vector2* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetOffset(void* @this, System.Numerics.Vector2* value)
        {
            ICompositionRoundedRectangleGeometry __target = null;
            try
            {
                {
                    __target = (ICompositionRoundedRectangleGeometry)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Offset;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetOffsetDelegate(void* @this, System.Numerics.Vector2 value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetOffset(void* @this, System.Numerics.Vector2 value)
        {
            ICompositionRoundedRectangleGeometry __target = null;
            try
            {
                {
                    __target = (ICompositionRoundedRectangleGeometry)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetOffset(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSizeDelegate(void* @this, System.Numerics.Vector2* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSize(void* @this, System.Numerics.Vector2* value)
        {
            ICompositionRoundedRectangleGeometry __target = null;
            try
            {
                {
                    __target = (ICompositionRoundedRectangleGeometry)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Size;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetSizeDelegate(void* @this, System.Numerics.Vector2 value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetSize(void* @this, System.Numerics.Vector2 value)
        {
            ICompositionRoundedRectangleGeometry __target = null;
            try
            {
                {
                    __target = (ICompositionRoundedRectangleGeometry)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetSize(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionRoundedRectangleGeometryVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2*, int>)&GetCornerRadius); 
#else
            base.AddMethod((GetCornerRadiusDelegate)GetCornerRadius); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)&SetCornerRadius); 
#else
            base.AddMethod((SetCornerRadiusDelegate)SetCornerRadius); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2*, int>)&GetOffset); 
#else
            base.AddMethod((GetOffsetDelegate)GetOffset); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)&SetOffset); 
#else
            base.AddMethod((SetOffsetDelegate)SetOffset); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2*, int>)&GetSize); 
#else
            base.AddMethod((GetSizeDelegate)GetSize); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)&SetSize); 
#else
            base.AddMethod((SetSizeDelegate)SetSize); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionRoundedRectangleGeometry), new __MicroComICompositionRoundedRectangleGeometryVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionGeometryProxy : __MicroComIInspectableProxy, ICompositionGeometry
    {
        public float TrimEnd
        {
            get
            {
                int __result;
                float value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetTrimEnd failed", __result);
                return value;
            }
        }

        public void SetTrimEnd(float value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, float, int>)(*PPV)[base.VTableSize + 1])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetTrimEnd failed", __result);
        }

        public float TrimOffset
        {
            get
            {
                int __result;
                float value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetTrimOffset failed", __result);
                return value;
            }
        }

        public void SetTrimOffset(float value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, float, int>)(*PPV)[base.VTableSize + 3])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetTrimOffset failed", __result);
        }

        public float TrimStart
        {
            get
            {
                int __result;
                float value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetTrimStart failed", __result);
                return value;
            }
        }

        public void SetTrimStart(float value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, float, int>)(*PPV)[base.VTableSize + 5])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetTrimStart failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionGeometry), new Guid("E985217C-6A17-4207-ABD8-5FD3DD612A9D"), (p, owns) => new __MicroComICompositionGeometryProxy(p, owns));
        }

        protected __MicroComICompositionGeometryProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 6;
    }

    unsafe class __MicroComICompositionGeometryVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetTrimEndDelegate(void* @this, float* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetTrimEnd(void* @this, float* value)
        {
            ICompositionGeometry __target = null;
            try
            {
                {
                    __target = (ICompositionGeometry)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.TrimEnd;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetTrimEndDelegate(void* @this, float value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetTrimEnd(void* @this, float value)
        {
            ICompositionGeometry __target = null;
            try
            {
                {
                    __target = (ICompositionGeometry)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetTrimEnd(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetTrimOffsetDelegate(void* @this, float* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetTrimOffset(void* @this, float* value)
        {
            ICompositionGeometry __target = null;
            try
            {
                {
                    __target = (ICompositionGeometry)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.TrimOffset;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetTrimOffsetDelegate(void* @this, float value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetTrimOffset(void* @this, float value)
        {
            ICompositionGeometry __target = null;
            try
            {
                {
                    __target = (ICompositionGeometry)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetTrimOffset(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetTrimStartDelegate(void* @this, float* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetTrimStart(void* @this, float* value)
        {
            ICompositionGeometry __target = null;
            try
            {
                {
                    __target = (ICompositionGeometry)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.TrimStart;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetTrimStartDelegate(void* @this, float value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetTrimStart(void* @this, float value)
        {
            ICompositionGeometry __target = null;
            try
            {
                {
                    __target = (ICompositionGeometry)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetTrimStart(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionGeometryVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float*, int>)&GetTrimEnd); 
#else
            base.AddMethod((GetTrimEndDelegate)GetTrimEnd); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float, int>)&SetTrimEnd); 
#else
            base.AddMethod((SetTrimEndDelegate)SetTrimEnd); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float*, int>)&GetTrimOffset); 
#else
            base.AddMethod((GetTrimOffsetDelegate)GetTrimOffset); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float, int>)&SetTrimOffset); 
#else
            base.AddMethod((SetTrimOffsetDelegate)SetTrimOffset); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float*, int>)&GetTrimStart); 
#else
            base.AddMethod((GetTrimStartDelegate)GetTrimStart); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float, int>)&SetTrimStart); 
#else
            base.AddMethod((SetTrimStartDelegate)SetTrimStart); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionGeometry), new __MicroComICompositionGeometryVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionSpriteShapeProxy : __MicroComIInspectableProxy, ICompositionSpriteShape
    {
        public ICompositionBrush FillBrush
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetFillBrush failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(__marshal_value, true);
            }
        }

        public void SetFillBrush(ICompositionBrush value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(value));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetFillBrush failed", __result);
        }

        public ICompositionGeometry Geometry
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetGeometry failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometry>(__marshal_value, true);
            }
        }

        public void SetGeometry(ICompositionGeometry value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(value));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetGeometry failed", __result);
        }

        public int IsStrokeNonScaling
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetIsStrokeNonScaling failed", __result);
                return value;
            }
        }

        public void SetIsStrokeNonScaling(int value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, int>)(*PPV)[base.VTableSize + 5])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetIsStrokeNonScaling failed", __result);
        }

        public ICompositionBrush StrokeBrush
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 6])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetStrokeBrush failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(__marshal_value, true);
            }
        }

        public void SetStrokeBrush(ICompositionBrush value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 7])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(value));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetStrokeBrush failed", __result);
        }

        public void GetStrokeDashArray()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 8])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetStrokeDashArray failed", __result);
        }

        public void GetStrokeDashCap()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 9])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetStrokeDashCap failed", __result);
        }

        public void SetStrokeDashCap()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 10])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetStrokeDashCap failed", __result);
        }

        public void GetStrokeDashOffset()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 11])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetStrokeDashOffset failed", __result);
        }

        public void SetStrokeDashOffset()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 12])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetStrokeDashOffset failed", __result);
        }

        public void GetStrokeEndCap()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 13])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetStrokeEndCap failed", __result);
        }

        public void SetStrokeEndCap()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 14])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetStrokeEndCap failed", __result);
        }

        public void GetStrokeLineJoin()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 15])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetStrokeLineJoin failed", __result);
        }

        public void SetStrokeLineJoin()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 16])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetStrokeLineJoin failed", __result);
        }

        public void GetStrokeMiterLimit()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 17])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetStrokeMiterLimit failed", __result);
        }

        public void SetStrokeMiterLimit()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 18])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetStrokeMiterLimit failed", __result);
        }

        public void GetStrokeStartCap()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 19])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetStrokeStartCap failed", __result);
        }

        public void SetStrokeStartCap()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 20])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetStrokeStartCap failed", __result);
        }

        public void GetStrokeThickness()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 21])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetStrokeThickness failed", __result);
        }

        public void SetStrokeThickness()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 22])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetStrokeThickness failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionSpriteShape), new Guid("401B61BB-0007-4363-B1F3-6BCC003FB83E"), (p, owns) => new __MicroComICompositionSpriteShapeProxy(p, owns));
        }

        protected __MicroComICompositionSpriteShapeProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 23;
    }

    unsafe class __MicroComICompositionSpriteShapeVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetFillBrushDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetFillBrush(void* @this, void** value)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.FillBrush;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetFillBrushDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetFillBrush(void* @this, void* value)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetFillBrush(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(value, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetGeometryDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetGeometry(void* @this, void** value)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Geometry;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetGeometryDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetGeometry(void* @this, void* value)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetGeometry(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometry>(value, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetIsStrokeNonScalingDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetIsStrokeNonScaling(void* @this, int* value)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.IsStrokeNonScaling;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetIsStrokeNonScalingDelegate(void* @this, int value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetIsStrokeNonScaling(void* @this, int value)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetIsStrokeNonScaling(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetStrokeBrushDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetStrokeBrush(void* @this, void** value)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.StrokeBrush;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetStrokeBrushDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetStrokeBrush(void* @this, void* value)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetStrokeBrush(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(value, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetStrokeDashArrayDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetStrokeDashArray(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetStrokeDashArray();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetStrokeDashCapDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetStrokeDashCap(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetStrokeDashCap();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetStrokeDashCapDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetStrokeDashCap(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetStrokeDashCap();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetStrokeDashOffsetDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetStrokeDashOffset(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetStrokeDashOffset();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetStrokeDashOffsetDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetStrokeDashOffset(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetStrokeDashOffset();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetStrokeEndCapDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetStrokeEndCap(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetStrokeEndCap();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetStrokeEndCapDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetStrokeEndCap(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetStrokeEndCap();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetStrokeLineJoinDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetStrokeLineJoin(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetStrokeLineJoin();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetStrokeLineJoinDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetStrokeLineJoin(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetStrokeLineJoin();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetStrokeMiterLimitDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetStrokeMiterLimit(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetStrokeMiterLimit();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetStrokeMiterLimitDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetStrokeMiterLimit(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetStrokeMiterLimit();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetStrokeStartCapDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetStrokeStartCap(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetStrokeStartCap();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetStrokeStartCapDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetStrokeStartCap(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetStrokeStartCap();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetStrokeThicknessDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetStrokeThickness(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetStrokeThickness();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetStrokeThicknessDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetStrokeThickness(void* @this)
        {
            ICompositionSpriteShape __target = null;
            try
            {
                {
                    __target = (ICompositionSpriteShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetStrokeThickness();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionSpriteShapeVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetFillBrush); 
#else
            base.AddMethod((GetFillBrushDelegate)GetFillBrush); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetFillBrush); 
#else
            base.AddMethod((SetFillBrushDelegate)SetFillBrush); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetGeometry); 
#else
            base.AddMethod((GetGeometryDelegate)GetGeometry); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetGeometry); 
#else
            base.AddMethod((SetGeometryDelegate)SetGeometry); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetIsStrokeNonScaling); 
#else
            base.AddMethod((GetIsStrokeNonScalingDelegate)GetIsStrokeNonScaling); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, int>)&SetIsStrokeNonScaling); 
#else
            base.AddMethod((SetIsStrokeNonScalingDelegate)SetIsStrokeNonScaling); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetStrokeBrush); 
#else
            base.AddMethod((GetStrokeBrushDelegate)GetStrokeBrush); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetStrokeBrush); 
#else
            base.AddMethod((SetStrokeBrushDelegate)SetStrokeBrush); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&GetStrokeDashArray); 
#else
            base.AddMethod((GetStrokeDashArrayDelegate)GetStrokeDashArray); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&GetStrokeDashCap); 
#else
            base.AddMethod((GetStrokeDashCapDelegate)GetStrokeDashCap); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&SetStrokeDashCap); 
#else
            base.AddMethod((SetStrokeDashCapDelegate)SetStrokeDashCap); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&GetStrokeDashOffset); 
#else
            base.AddMethod((GetStrokeDashOffsetDelegate)GetStrokeDashOffset); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&SetStrokeDashOffset); 
#else
            base.AddMethod((SetStrokeDashOffsetDelegate)SetStrokeDashOffset); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&GetStrokeEndCap); 
#else
            base.AddMethod((GetStrokeEndCapDelegate)GetStrokeEndCap); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&SetStrokeEndCap); 
#else
            base.AddMethod((SetStrokeEndCapDelegate)SetStrokeEndCap); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&GetStrokeLineJoin); 
#else
            base.AddMethod((GetStrokeLineJoinDelegate)GetStrokeLineJoin); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&SetStrokeLineJoin); 
#else
            base.AddMethod((SetStrokeLineJoinDelegate)SetStrokeLineJoin); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&GetStrokeMiterLimit); 
#else
            base.AddMethod((GetStrokeMiterLimitDelegate)GetStrokeMiterLimit); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&SetStrokeMiterLimit); 
#else
            base.AddMethod((SetStrokeMiterLimitDelegate)SetStrokeMiterLimit); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&GetStrokeStartCap); 
#else
            base.AddMethod((GetStrokeStartCapDelegate)GetStrokeStartCap); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&SetStrokeStartCap); 
#else
            base.AddMethod((SetStrokeStartCapDelegate)SetStrokeStartCap); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&GetStrokeThickness); 
#else
            base.AddMethod((GetStrokeThicknessDelegate)GetStrokeThickness); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&SetStrokeThickness); 
#else
            base.AddMethod((SetStrokeThicknessDelegate)SetStrokeThickness); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionSpriteShape), new __MicroComICompositionSpriteShapeVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionShapeProxy : __MicroComIInspectableProxy, ICompositionShape
    {
        public System.Numerics.Vector2 CenterPoint
        {
            get
            {
                int __result;
                System.Numerics.Vector2 value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetCenterPoint failed", __result);
                return value;
            }
        }

        public void SetCenterPoint(System.Numerics.Vector2 value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)(*PPV)[base.VTableSize + 1])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetCenterPoint failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionShape), new Guid("B47CE2F7-9A88-42C4-9E87-2E500CA8688C"), (p, owns) => new __MicroComICompositionShapeProxy(p, owns));
        }

        protected __MicroComICompositionShapeProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComICompositionShapeVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetCenterPointDelegate(void* @this, System.Numerics.Vector2* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCenterPoint(void* @this, System.Numerics.Vector2* value)
        {
            ICompositionShape __target = null;
            try
            {
                {
                    __target = (ICompositionShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CenterPoint;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetCenterPointDelegate(void* @this, System.Numerics.Vector2 value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetCenterPoint(void* @this, System.Numerics.Vector2 value)
        {
            ICompositionShape __target = null;
            try
            {
                {
                    __target = (ICompositionShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetCenterPoint(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionShapeVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2*, int>)&GetCenterPoint); 
#else
            base.AddMethod((GetCenterPointDelegate)GetCenterPoint); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Numerics.Vector2, int>)&SetCenterPoint); 
#else
            base.AddMethod((SetCenterPointDelegate)SetCenterPoint); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionShape), new __MicroComICompositionShapeVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIVectorOfCompositionShapeProxy : __MicroComIInspectableProxy, IVectorOfCompositionShape
    {
        public void GetAt()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 0])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetAt failed", __result);
        }

        public void GetSize()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 1])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetSize failed", __result);
        }

        public void GetView()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 2])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetView failed", __result);
        }

        public void IndexOf()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 3])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("IndexOf failed", __result);
        }

        public void SetAt()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 4])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetAt failed", __result);
        }

        public void InsertAt()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 5])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("InsertAt failed", __result);
        }

        public void RemoveAt()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 6])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RemoveAt failed", __result);
        }

        public void Append(ICompositionShape value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 7])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(value));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Append failed", __result);
        }

        public void RemoveAtEnd()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 8])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RemoveAtEnd failed", __result);
        }

        public void Clear()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 9])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Clear failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IVectorOfCompositionShape), new Guid("42d4219a-be1b-5091-8f1e-90270840fc2d"), (p, owns) => new __MicroComIVectorOfCompositionShapeProxy(p, owns));
        }

        protected __MicroComIVectorOfCompositionShapeProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 10;
    }

    unsafe class __MicroComIVectorOfCompositionShapeVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetAtDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetAt(void* @this)
        {
            IVectorOfCompositionShape __target = null;
            try
            {
                {
                    __target = (IVectorOfCompositionShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetAt();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSizeDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSize(void* @this)
        {
            IVectorOfCompositionShape __target = null;
            try
            {
                {
                    __target = (IVectorOfCompositionShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetSize();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetViewDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetView(void* @this)
        {
            IVectorOfCompositionShape __target = null;
            try
            {
                {
                    __target = (IVectorOfCompositionShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetView();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int IndexOfDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int IndexOf(void* @this)
        {
            IVectorOfCompositionShape __target = null;
            try
            {
                {
                    __target = (IVectorOfCompositionShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.IndexOf();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetAtDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetAt(void* @this)
        {
            IVectorOfCompositionShape __target = null;
            try
            {
                {
                    __target = (IVectorOfCompositionShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetAt();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int InsertAtDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int InsertAt(void* @this)
        {
            IVectorOfCompositionShape __target = null;
            try
            {
                {
                    __target = (IVectorOfCompositionShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.InsertAt();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveAtDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RemoveAt(void* @this)
        {
            IVectorOfCompositionShape __target = null;
            try
            {
                {
                    __target = (IVectorOfCompositionShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RemoveAt();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int AppendDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Append(void* @this, void* value)
        {
            IVectorOfCompositionShape __target = null;
            try
            {
                {
                    __target = (IVectorOfCompositionShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Append(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionShape>(value, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveAtEndDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RemoveAtEnd(void* @this)
        {
            IVectorOfCompositionShape __target = null;
            try
            {
                {
                    __target = (IVectorOfCompositionShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RemoveAtEnd();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ClearDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Clear(void* @this)
        {
            IVectorOfCompositionShape __target = null;
            try
            {
                {
                    __target = (IVectorOfCompositionShape)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Clear();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIVectorOfCompositionShapeVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&GetAt); 
#else
            base.AddMethod((GetAtDelegate)GetAt); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&GetSize); 
#else
            base.AddMethod((GetSizeDelegate)GetSize); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&GetView); 
#else
            base.AddMethod((GetViewDelegate)GetView); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&IndexOf); 
#else
            base.AddMethod((IndexOfDelegate)IndexOf); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&SetAt); 
#else
            base.AddMethod((SetAtDelegate)SetAt); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&InsertAt); 
#else
            base.AddMethod((InsertAtDelegate)InsertAt); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&RemoveAt); 
#else
            base.AddMethod((RemoveAtDelegate)RemoveAt); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&Append); 
#else
            base.AddMethod((AppendDelegate)Append); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&RemoveAtEnd); 
#else
            base.AddMethod((RemoveAtEndDelegate)RemoveAtEnd); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&Clear); 
#else
            base.AddMethod((ClearDelegate)Clear); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IVectorOfCompositionShape), new __MicroComIVectorOfCompositionShapeVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositionGeometricClipProxy : __MicroComIInspectableProxy, ICompositionGeometricClip
    {
        public ICompositionGeometry Geometry
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetGeometry failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometry>(__marshal_value, true);
            }
        }

        public void SetGeometry(ICompositionGeometry value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(value));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetGeometry failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositionGeometricClip), new Guid("C840B581-81C9-4444-A2C1-CCAECE3A50E5"), (p, owns) => new __MicroComICompositionGeometricClipProxy(p, owns));
        }

        protected __MicroComICompositionGeometricClipProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComICompositionGeometricClipVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetGeometryDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetGeometry(void* @this, void** value)
        {
            ICompositionGeometricClip __target = null;
            try
            {
                {
                    __target = (ICompositionGeometricClip)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Geometry;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetGeometryDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetGeometry(void* @this, void* value)
        {
            ICompositionGeometricClip __target = null;
            try
            {
                {
                    __target = (ICompositionGeometricClip)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetGeometry(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometry>(value, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositionGeometricClipVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetGeometry); 
#else
            base.AddMethod((GetGeometryDelegate)GetGeometry); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetGeometry); 
#else
            base.AddMethod((SetGeometryDelegate)SetGeometry); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositionGeometricClip), new __MicroComICompositionGeometricClipVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIShapeVisualProxy : __MicroComIInspectableProxy, IShapeVisual
    {
        public IUnknown Shapes
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetShapes failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IUnknown>(__marshal_value, true);
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IShapeVisual), new Guid("F2BD13C3-BA7E-4B0F-9126-FFB7536B8176"), (p, owns) => new __MicroComIShapeVisualProxy(p, owns));
        }

        protected __MicroComIShapeVisualProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIShapeVisualVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetShapesDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetShapes(void* @this, void** value)
        {
            IShapeVisual __target = null;
            try
            {
                {
                    __target = (IShapeVisual)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Shapes;
                        *value = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIShapeVisualVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetShapes); 
#else
            base.AddMethod((GetShapesDelegate)GetShapes); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IShapeVisual), new __MicroComIShapeVisualVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositor5Proxy : __MicroComIInspectableProxy, ICompositor5
    {
        public IntPtr Comment
        {
            get
            {
                int __result;
                IntPtr value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetComment failed", __result);
                return value;
            }
        }

        public void SetComment(IntPtr value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 1])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetComment failed", __result);
        }

        public float GlobalPlaybackRate
        {
            get
            {
                int __result;
                float value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetGlobalPlaybackRate failed", __result);
                return value;
            }
        }

        public void SetGlobalPlaybackRate(float value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, float, int>)(*PPV)[base.VTableSize + 3])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetGlobalPlaybackRate failed", __result);
        }

        public void* CreateBounceScalarAnimation()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateBounceScalarAnimation failed", __result);
            return result;
        }

        public void* CreateBounceVector2Animation()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 5])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateBounceVector2Animation failed", __result);
            return result;
        }

        public void* CreateBounceVector3Animation()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 6])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateBounceVector3Animation failed", __result);
            return result;
        }

        public void* CreateContainerShape()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 7])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateContainerShape failed", __result);
            return result;
        }

        public void* CreateEllipseGeometry()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 8])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateEllipseGeometry failed", __result);
            return result;
        }

        public void* CreateLineGeometry()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 9])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateLineGeometry failed", __result);
            return result;
        }

        public void* CreatePathGeometry()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 10])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreatePathGeometry failed", __result);
            return result;
        }

        public void* CreatePathGeometryWithPath(void* path)
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 11])(PPV, path, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreatePathGeometryWithPath failed", __result);
            return result;
        }

        public void* CreatePathKeyFrameAnimation()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 12])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreatePathKeyFrameAnimation failed", __result);
            return result;
        }

        public void* CreateRectangleGeometry()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 13])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateRectangleGeometry failed", __result);
            return result;
        }

        public ICompositionRoundedRectangleGeometry CreateRoundedRectangleGeometry()
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 14])(PPV, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateRoundedRectangleGeometry failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionRoundedRectangleGeometry>(__marshal_result, true);
        }

        public IShapeVisual CreateShapeVisual()
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 15])(PPV, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateShapeVisual failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IShapeVisual>(__marshal_result, true);
        }

        public void* CreateSpriteShape()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 16])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateSpriteShape failed", __result);
            return result;
        }

        public void* CreateSpriteShapeWithGeometry(void* geometry)
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 17])(PPV, geometry, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateSpriteShapeWithGeometry failed", __result);
            return result;
        }

        public void* CreateViewBox()
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 18])(PPV, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateViewBox failed", __result);
            return result;
        }

        public IAsyncAction RequestCommitAsync()
        {
            int __result;
            void* __marshal_operation = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 19])(PPV, &__marshal_operation);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RequestCommitAsync failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IAsyncAction>(__marshal_operation, true);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositor5), new Guid("48EA31AD-7FCD-4076-A79C-90CC4B852C9B"), (p, owns) => new __MicroComICompositor5Proxy(p, owns));
        }

        protected __MicroComICompositor5Proxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 20;
    }

    unsafe class __MicroComICompositor5VTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetCommentDelegate(void* @this, IntPtr* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetComment(void* @this, IntPtr* value)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Comment;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetCommentDelegate(void* @this, IntPtr value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetComment(void* @this, IntPtr value)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetComment(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetGlobalPlaybackRateDelegate(void* @this, float* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetGlobalPlaybackRate(void* @this, float* value)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GlobalPlaybackRate;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetGlobalPlaybackRateDelegate(void* @this, float value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetGlobalPlaybackRate(void* @this, float value)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetGlobalPlaybackRate(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateBounceScalarAnimationDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateBounceScalarAnimation(void* @this, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateBounceScalarAnimation();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateBounceVector2AnimationDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateBounceVector2Animation(void* @this, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateBounceVector2Animation();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateBounceVector3AnimationDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateBounceVector3Animation(void* @this, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateBounceVector3Animation();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateContainerShapeDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateContainerShape(void* @this, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateContainerShape();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateEllipseGeometryDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateEllipseGeometry(void* @this, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateEllipseGeometry();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateLineGeometryDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateLineGeometry(void* @this, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateLineGeometry();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreatePathGeometryDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreatePathGeometry(void* @this, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreatePathGeometry();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreatePathGeometryWithPathDelegate(void* @this, void* path, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreatePathGeometryWithPath(void* @this, void* path, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreatePathGeometryWithPath(path);
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreatePathKeyFrameAnimationDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreatePathKeyFrameAnimation(void* @this, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreatePathKeyFrameAnimation();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateRectangleGeometryDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateRectangleGeometry(void* @this, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateRectangleGeometry();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateRoundedRectangleGeometryDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateRoundedRectangleGeometry(void* @this, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateRoundedRectangleGeometry();
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateShapeVisualDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateShapeVisual(void* @this, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateShapeVisual();
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateSpriteShapeDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateSpriteShape(void* @this, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateSpriteShape();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateSpriteShapeWithGeometryDelegate(void* @this, void* geometry, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateSpriteShapeWithGeometry(void* @this, void* geometry, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateSpriteShapeWithGeometry(geometry);
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateViewBoxDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateViewBox(void* @this, void** result)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateViewBox();
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RequestCommitAsyncDelegate(void* @this, void** operation);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RequestCommitAsync(void* @this, void** operation)
        {
            ICompositor5 __target = null;
            try
            {
                {
                    __target = (ICompositor5)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.RequestCommitAsync();
                        *operation = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositor5VTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetComment); 
#else
            base.AddMethod((GetCommentDelegate)GetComment); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&SetComment); 
#else
            base.AddMethod((SetCommentDelegate)SetComment); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float*, int>)&GetGlobalPlaybackRate); 
#else
            base.AddMethod((GetGlobalPlaybackRateDelegate)GetGlobalPlaybackRate); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, float, int>)&SetGlobalPlaybackRate); 
#else
            base.AddMethod((SetGlobalPlaybackRateDelegate)SetGlobalPlaybackRate); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateBounceScalarAnimation); 
#else
            base.AddMethod((CreateBounceScalarAnimationDelegate)CreateBounceScalarAnimation); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateBounceVector2Animation); 
#else
            base.AddMethod((CreateBounceVector2AnimationDelegate)CreateBounceVector2Animation); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateBounceVector3Animation); 
#else
            base.AddMethod((CreateBounceVector3AnimationDelegate)CreateBounceVector3Animation); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateContainerShape); 
#else
            base.AddMethod((CreateContainerShapeDelegate)CreateContainerShape); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateEllipseGeometry); 
#else
            base.AddMethod((CreateEllipseGeometryDelegate)CreateEllipseGeometry); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateLineGeometry); 
#else
            base.AddMethod((CreateLineGeometryDelegate)CreateLineGeometry); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreatePathGeometry); 
#else
            base.AddMethod((CreatePathGeometryDelegate)CreatePathGeometry); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, void**, int>)&CreatePathGeometryWithPath); 
#else
            base.AddMethod((CreatePathGeometryWithPathDelegate)CreatePathGeometryWithPath); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreatePathKeyFrameAnimation); 
#else
            base.AddMethod((CreatePathKeyFrameAnimationDelegate)CreatePathKeyFrameAnimation); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateRectangleGeometry); 
#else
            base.AddMethod((CreateRectangleGeometryDelegate)CreateRectangleGeometry); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateRoundedRectangleGeometry); 
#else
            base.AddMethod((CreateRoundedRectangleGeometryDelegate)CreateRoundedRectangleGeometry); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateShapeVisual); 
#else
            base.AddMethod((CreateShapeVisualDelegate)CreateShapeVisual); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateSpriteShape); 
#else
            base.AddMethod((CreateSpriteShapeDelegate)CreateSpriteShape); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, void**, int>)&CreateSpriteShapeWithGeometry); 
#else
            base.AddMethod((CreateSpriteShapeWithGeometryDelegate)CreateSpriteShapeWithGeometry); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateViewBox); 
#else
            base.AddMethod((CreateViewBoxDelegate)CreateViewBox); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&RequestCommitAsync); 
#else
            base.AddMethod((RequestCommitAsyncDelegate)RequestCommitAsync); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositor5), new __MicroComICompositor5VTable().CreateVTable());
    }

    internal unsafe partial class __MicroComICompositor6Proxy : __MicroComIInspectableProxy, ICompositor6
    {
        public ICompositionGeometricClip CreateGeometricClip()
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateGeometricClip failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometricClip>(__marshal_result, true);
        }

        public ICompositionGeometricClip CreateGeometricClipWithGeometry(ICompositionGeometry geometry)
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(geometry), &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateGeometricClipWithGeometry failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometricClip>(__marshal_result, true);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(ICompositor6), new Guid("7A38B2BD-CEC8-4EEB-830F-D8D07AEDEBC3"), (p, owns) => new __MicroComICompositor6Proxy(p, owns));
        }

        protected __MicroComICompositor6Proxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComICompositor6VTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateGeometricClipDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateGeometricClip(void* @this, void** result)
        {
            ICompositor6 __target = null;
            try
            {
                {
                    __target = (ICompositor6)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateGeometricClip();
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateGeometricClipWithGeometryDelegate(void* @this, void* geometry, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateGeometricClipWithGeometry(void* @this, void* geometry, void** result)
        {
            ICompositor6 __target = null;
            try
            {
                {
                    __target = (ICompositor6)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateGeometricClipWithGeometry(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometry>(geometry, false));
                        *result = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComICompositor6VTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateGeometricClip); 
#else
            base.AddMethod((CreateGeometricClipDelegate)CreateGeometricClip); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, void**, int>)&CreateGeometricClipWithGeometry); 
#else
            base.AddMethod((CreateGeometricClipWithGeometryDelegate)CreateGeometricClipWithGeometry); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(ICompositor6), new __MicroComICompositor6VTable().CreateVTable());
    }
}
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

namespace Avalonia.Win32.Win32Com
{
    [System.Flags()]
    internal enum FILEOPENDIALOGOPTIONS
    {
        FOS_OVERWRITEPROMPT = 0x00000002,
        FOS_STRICTFILETYPES = 0x00000004,
        FOS_NOCHANGEDIR = 0x00000008,
        FOS_PICKFOLDERS = 0x00000020,
        FOS_FORCEFILESYSTEM = 0x00000040,
        FOS_ALLNONSTORAGEITEMS = 0x00000080,
        FOS_NOVALIDATE = 0x00000100,
        FOS_ALLOWMULTISELECT = 0x00000200,
        FOS_PATHMUSTEXIST = 0x00000800,
        FOS_FILEMUSTEXIST = 0x00001000,
        FOS_CREATEPROMPT = 0x00002000,
        FOS_SHAREAWARE = 0x00004000,
        FOS_NOREADONLYRETURN = 0x00008000,
        FOS_NOTESTFILECREATE = 0x00010000,
        FOS_HIDEMRUPLACES = 0x00020000,
        FOS_HIDEPINNEDPLACES = 0x00040000,
        FOS_NODEREFERENCELINKS = 0x00100000,
        FOS_DONTADDTORECENT = 0x02000000,
        FOS_FORCESHOWHIDDEN = 0x10000000,
        FOS_DEFAULTNOMINIMODE = 0x20000000
    }

    internal unsafe partial interface IShellItem : global::Avalonia.MicroCom.IUnknown
    {
        void* BindToHandler(void* pbc, System.Guid* bhid, System.Guid* riid);
        IShellItem Parent { get; }

        System.Char* GetDisplayName(uint sigdnName);
        uint GetAttributes(uint sfgaoMask);
        int Compare(IShellItem psi, uint hint);
    }

    internal unsafe partial interface IShellItemArray : global::Avalonia.MicroCom.IUnknown
    {
        void* BindToHandler(void* pbc, System.Guid* bhid, System.Guid* riid);
        void* GetPropertyStore(ushort flags, System.Guid* riid);
        void* GetPropertyDescriptionList(void* keyType, System.Guid* riid);
        ushort GetAttributes(int AttribFlags, ushort sfgaoMask);
        int Count { get; }

        IShellItem GetItemAt(int dwIndex);
        void* EnumItems();
    }

    internal unsafe partial interface IModalWindow : global::Avalonia.MicroCom.IUnknown
    {
        int Show(IntPtr hwndOwner);
    }

    internal unsafe partial interface IFileDialog : IModalWindow
    {
        void SetFileTypes(ushort cFileTypes, void* rgFilterSpec);
        void SetFileTypeIndex(ushort iFileType);
        ushort FileTypeIndex { get; }

        int Advise(void* pfde);
        void Unadvise(int dwCookie);
        void SetOptions(FILEOPENDIALOGOPTIONS fos);
        FILEOPENDIALOGOPTIONS Options { get; }

        void SetDefaultFolder(IShellItem psi);
        void SetFolder(IShellItem psi);
        IShellItem Folder { get; }

        IShellItem CurrentSelection { get; }

        void SetFileName(System.Char* pszName);
        System.Char* FileName { get; }

        void SetTitle(System.Char* pszTitle);
        void SetOkButtonLabel(System.Char* pszText);
        void SetFileNameLabel(System.Char* pszLabel);
        IShellItem Result { get; }

        void AddPlace(IShellItem psi, int fdap);
        void SetDefaultExtension(System.Char* pszDefaultExtension);
        void Close(int hr);
        void SetClientGuid(System.Guid* guid);
        void ClearClientData();
        void SetFilter(void* pFilter);
    }

    internal unsafe partial interface IFileOpenDialog : IFileDialog
    {
        IShellItemArray Results { get; }

        IShellItemArray SelectedItems { get; }
    }
}

namespace Avalonia.Win32.Win32Com.Impl
{
    internal unsafe partial class __MicroComIShellItemProxy : global::Avalonia.MicroCom.MicroComProxyBase, IShellItem
    {
        public void* BindToHandler(void* pbc, System.Guid* bhid, System.Guid* riid)
        {
            int __result;
            void* ppv = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, pbc, bhid, riid, &ppv);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("BindToHandler failed", __result);
            return ppv;
        }

        public IShellItem Parent
        {
            get
            {
                int __result;
                void* __marshal_ppsi = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &__marshal_ppsi);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetParent failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IShellItem>(__marshal_ppsi, true);
            }
        }

        public System.Char* GetDisplayName(uint sigdnName)
        {
            int __result;
            System.Char* ppszName = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, uint, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, sigdnName, &ppszName);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetDisplayName failed", __result);
            return ppszName;
        }

        public uint GetAttributes(uint sfgaoMask)
        {
            int __result;
            uint psfgaoAttribs = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, uint, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, sfgaoMask, &psfgaoAttribs);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetAttributes failed", __result);
            return psfgaoAttribs;
        }

        public int Compare(IShellItem psi, uint hint)
        {
            int __result;
            int piOrder = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, uint, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(psi), hint, &piOrder);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Compare failed", __result);
            return piOrder;
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IShellItem), new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe"), (p, owns) => new __MicroComIShellItemProxy(p, owns));
        }

        protected __MicroComIShellItemProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 5;
    }

    unsafe class __MicroComIShellItemVTable : global::Avalonia.MicroCom.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int BindToHandlerDelegate(void* @this, void* pbc, System.Guid* bhid, System.Guid* riid, void** ppv);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int BindToHandler(void* @this, void* pbc, System.Guid* bhid, System.Guid* riid, void** ppv)
        {
            IShellItem __target = null;
            try
            {
                {
                    __target = (IShellItem)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.BindToHandler(pbc, bhid, riid);
                        *ppv = __result;
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
        delegate int GetParentDelegate(void* @this, void** ppsi);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetParent(void* @this, void** ppsi)
        {
            IShellItem __target = null;
            try
            {
                {
                    __target = (IShellItem)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Parent;
                        *ppsi = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
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
        delegate int GetDisplayNameDelegate(void* @this, uint sigdnName, System.Char** ppszName);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetDisplayName(void* @this, uint sigdnName, System.Char** ppszName)
        {
            IShellItem __target = null;
            try
            {
                {
                    __target = (IShellItem)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetDisplayName(sigdnName);
                        *ppszName = __result;
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
        delegate int GetAttributesDelegate(void* @this, uint sfgaoMask, uint* psfgaoAttribs);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetAttributes(void* @this, uint sfgaoMask, uint* psfgaoAttribs)
        {
            IShellItem __target = null;
            try
            {
                {
                    __target = (IShellItem)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetAttributes(sfgaoMask);
                        *psfgaoAttribs = __result;
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
        delegate int CompareDelegate(void* @this, void* psi, uint hint, int* piOrder);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Compare(void* @this, void* psi, uint hint, int* piOrder)
        {
            IShellItem __target = null;
            try
            {
                {
                    __target = (IShellItem)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Compare(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IShellItem>(psi, false), hint);
                        *piOrder = __result;
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

        protected __MicroComIShellItemVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, System.Guid*, System.Guid*, void**, int>)&BindToHandler); 
#else
            base.AddMethod((BindToHandlerDelegate)BindToHandler); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetParent); 
#else
            base.AddMethod((GetParentDelegate)GetParent); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint, System.Char**, int>)&GetDisplayName); 
#else
            base.AddMethod((GetDisplayNameDelegate)GetDisplayName); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint, uint*, int>)&GetAttributes); 
#else
            base.AddMethod((GetAttributesDelegate)GetAttributes); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, uint, int*, int>)&Compare); 
#else
            base.AddMethod((CompareDelegate)Compare); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IShellItem), new __MicroComIShellItemVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIShellItemArrayProxy : global::Avalonia.MicroCom.MicroComProxyBase, IShellItemArray
    {
        public void* BindToHandler(void* pbc, System.Guid* bhid, System.Guid* riid)
        {
            int __result;
            void* ppvOut = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, pbc, bhid, riid, &ppvOut);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("BindToHandler failed", __result);
            return ppvOut;
        }

        public void* GetPropertyStore(ushort flags, System.Guid* riid)
        {
            int __result;
            void* ppv = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, ushort, void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, flags, riid, &ppv);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetPropertyStore failed", __result);
            return ppv;
        }

        public void* GetPropertyDescriptionList(void* keyType, System.Guid* riid)
        {
            int __result;
            void* ppv = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, keyType, riid, &ppv);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetPropertyDescriptionList failed", __result);
            return ppv;
        }

        public ushort GetAttributes(int AttribFlags, ushort sfgaoMask)
        {
            int __result;
            ushort psfgaoAttribs = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, ushort, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, AttribFlags, sfgaoMask, &psfgaoAttribs);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetAttributes failed", __result);
            return psfgaoAttribs;
        }

        public int Count
        {
            get
            {
                int __result;
                int pdwNumItems = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, &pdwNumItems);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetCount failed", __result);
                return pdwNumItems;
            }
        }

        public IShellItem GetItemAt(int dwIndex)
        {
            int __result;
            void* __marshal_ppsi = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, void*, int>)(*PPV)[base.VTableSize + 5])(PPV, dwIndex, &__marshal_ppsi);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetItemAt failed", __result);
            return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IShellItem>(__marshal_ppsi, true);
        }

        public void* EnumItems()
        {
            int __result;
            void* ppenumShellItems = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 6])(PPV, &ppenumShellItems);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("EnumItems failed", __result);
            return ppenumShellItems;
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IShellItemArray), new Guid("B63EA76D-1F85-456F-A19C-48159EFA858B"), (p, owns) => new __MicroComIShellItemArrayProxy(p, owns));
        }

        protected __MicroComIShellItemArrayProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 7;
    }

    unsafe class __MicroComIShellItemArrayVTable : global::Avalonia.MicroCom.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int BindToHandlerDelegate(void* @this, void* pbc, System.Guid* bhid, System.Guid* riid, void** ppvOut);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int BindToHandler(void* @this, void* pbc, System.Guid* bhid, System.Guid* riid, void** ppvOut)
        {
            IShellItemArray __target = null;
            try
            {
                {
                    __target = (IShellItemArray)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.BindToHandler(pbc, bhid, riid);
                        *ppvOut = __result;
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
        delegate int GetPropertyStoreDelegate(void* @this, ushort flags, System.Guid* riid, void** ppv);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetPropertyStore(void* @this, ushort flags, System.Guid* riid, void** ppv)
        {
            IShellItemArray __target = null;
            try
            {
                {
                    __target = (IShellItemArray)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetPropertyStore(flags, riid);
                        *ppv = __result;
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
        delegate int GetPropertyDescriptionListDelegate(void* @this, void* keyType, System.Guid* riid, void** ppv);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetPropertyDescriptionList(void* @this, void* keyType, System.Guid* riid, void** ppv)
        {
            IShellItemArray __target = null;
            try
            {
                {
                    __target = (IShellItemArray)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetPropertyDescriptionList(keyType, riid);
                        *ppv = __result;
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
        delegate int GetAttributesDelegate(void* @this, int AttribFlags, ushort sfgaoMask, ushort* psfgaoAttribs);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetAttributes(void* @this, int AttribFlags, ushort sfgaoMask, ushort* psfgaoAttribs)
        {
            IShellItemArray __target = null;
            try
            {
                {
                    __target = (IShellItemArray)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetAttributes(AttribFlags, sfgaoMask);
                        *psfgaoAttribs = __result;
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
        delegate int GetCountDelegate(void* @this, int* pdwNumItems);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCount(void* @this, int* pdwNumItems)
        {
            IShellItemArray __target = null;
            try
            {
                {
                    __target = (IShellItemArray)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Count;
                        *pdwNumItems = __result;
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
        delegate int GetItemAtDelegate(void* @this, int dwIndex, void** ppsi);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetItemAt(void* @this, int dwIndex, void** ppsi)
        {
            IShellItemArray __target = null;
            try
            {
                {
                    __target = (IShellItemArray)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetItemAt(dwIndex);
                        *ppsi = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
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
        delegate int EnumItemsDelegate(void* @this, void** ppenumShellItems);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int EnumItems(void* @this, void** ppenumShellItems)
        {
            IShellItemArray __target = null;
            try
            {
                {
                    __target = (IShellItemArray)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.EnumItems();
                        *ppenumShellItems = __result;
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

        protected __MicroComIShellItemArrayVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, System.Guid*, System.Guid*, void**, int>)&BindToHandler); 
#else
            base.AddMethod((BindToHandlerDelegate)BindToHandler); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, ushort, System.Guid*, void**, int>)&GetPropertyStore); 
#else
            base.AddMethod((GetPropertyStoreDelegate)GetPropertyStore); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, System.Guid*, void**, int>)&GetPropertyDescriptionList); 
#else
            base.AddMethod((GetPropertyDescriptionListDelegate)GetPropertyDescriptionList); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, ushort, ushort*, int>)&GetAttributes); 
#else
            base.AddMethod((GetAttributesDelegate)GetAttributes); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetCount); 
#else
            base.AddMethod((GetCountDelegate)GetCount); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, void**, int>)&GetItemAt); 
#else
            base.AddMethod((GetItemAtDelegate)GetItemAt); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&EnumItems); 
#else
            base.AddMethod((EnumItemsDelegate)EnumItems); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IShellItemArray), new __MicroComIShellItemArrayVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIModalWindowProxy : global::Avalonia.MicroCom.MicroComProxyBase, IModalWindow
    {
        public int Show(IntPtr hwndOwner)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 0])(PPV, hwndOwner);
            return __result;
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IModalWindow), new Guid("B4DB1657-70D7-485E-8E3E-6FCB5A5C1802"), (p, owns) => new __MicroComIModalWindowProxy(p, owns));
        }

        protected __MicroComIModalWindowProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIModalWindowVTable : global::Avalonia.MicroCom.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ShowDelegate(void* @this, IntPtr hwndOwner);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Show(void* @this, IntPtr hwndOwner)
        {
            IModalWindow __target = null;
            try
            {
                {
                    __target = (IModalWindow)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Show(hwndOwner);
                        return __result;
                    }
                }
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return default;
            }
        }

        protected __MicroComIModalWindowVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&Show); 
#else
            base.AddMethod((ShowDelegate)Show); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IModalWindow), new __MicroComIModalWindowVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIFileDialogProxy : __MicroComIModalWindowProxy, IFileDialog
    {
        public void SetFileTypes(ushort cFileTypes, void* rgFilterSpec)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, ushort, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, cFileTypes, rgFilterSpec);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetFileTypes failed", __result);
        }

        public void SetFileTypeIndex(ushort iFileType)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, ushort, int>)(*PPV)[base.VTableSize + 1])(PPV, iFileType);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetFileTypeIndex failed", __result);
        }

        public ushort FileTypeIndex
        {
            get
            {
                int __result;
                ushort piFileType = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &piFileType);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetFileTypeIndex failed", __result);
                return piFileType;
            }
        }

        public int Advise(void* pfde)
        {
            int __result;
            int pdwCookie = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, pfde, &pdwCookie);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Advise failed", __result);
            return pdwCookie;
        }

        public void Unadvise(int dwCookie)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, int>)(*PPV)[base.VTableSize + 4])(PPV, dwCookie);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Unadvise failed", __result);
        }

        public void SetOptions(FILEOPENDIALOGOPTIONS fos)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, FILEOPENDIALOGOPTIONS, int>)(*PPV)[base.VTableSize + 5])(PPV, fos);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetOptions failed", __result);
        }

        public FILEOPENDIALOGOPTIONS Options
        {
            get
            {
                int __result;
                FILEOPENDIALOGOPTIONS pfos = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 6])(PPV, &pfos);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetOptions failed", __result);
                return pfos;
            }
        }

        public void SetDefaultFolder(IShellItem psi)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 7])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(psi));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetDefaultFolder failed", __result);
        }

        public void SetFolder(IShellItem psi)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 8])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(psi));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetFolder failed", __result);
        }

        public IShellItem Folder
        {
            get
            {
                int __result;
                void* __marshal_ppsi = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 9])(PPV, &__marshal_ppsi);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetFolder failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IShellItem>(__marshal_ppsi, true);
            }
        }

        public IShellItem CurrentSelection
        {
            get
            {
                int __result;
                void* __marshal_ppsi = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 10])(PPV, &__marshal_ppsi);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetCurrentSelection failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IShellItem>(__marshal_ppsi, true);
            }
        }

        public void SetFileName(System.Char* pszName)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 11])(PPV, pszName);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetFileName failed", __result);
        }

        public System.Char* FileName
        {
            get
            {
                int __result;
                System.Char* pszName = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 12])(PPV, &pszName);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetFileName failed", __result);
                return pszName;
            }
        }

        public void SetTitle(System.Char* pszTitle)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 13])(PPV, pszTitle);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetTitle failed", __result);
        }

        public void SetOkButtonLabel(System.Char* pszText)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 14])(PPV, pszText);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetOkButtonLabel failed", __result);
        }

        public void SetFileNameLabel(System.Char* pszLabel)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 15])(PPV, pszLabel);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetFileNameLabel failed", __result);
        }

        public IShellItem Result
        {
            get
            {
                int __result;
                void* __marshal_ppsi = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 16])(PPV, &__marshal_ppsi);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetResult failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IShellItem>(__marshal_ppsi, true);
            }
        }

        public void AddPlace(IShellItem psi, int fdap)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int, int>)(*PPV)[base.VTableSize + 17])(PPV, global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(psi), fdap);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("AddPlace failed", __result);
        }

        public void SetDefaultExtension(System.Char* pszDefaultExtension)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 18])(PPV, pszDefaultExtension);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetDefaultExtension failed", __result);
        }

        public void Close(int hr)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, int>)(*PPV)[base.VTableSize + 19])(PPV, hr);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Close failed", __result);
        }

        public void SetClientGuid(System.Guid* guid)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 20])(PPV, guid);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetClientGuid failed", __result);
        }

        public void ClearClientData()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 21])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("ClearClientData failed", __result);
        }

        public void SetFilter(void* pFilter)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 22])(PPV, pFilter);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetFilter failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IFileDialog), new Guid("42F85136-DB7E-439C-85F1-E4075D135FC8"), (p, owns) => new __MicroComIFileDialogProxy(p, owns));
        }

        protected __MicroComIFileDialogProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 23;
    }

    unsafe class __MicroComIFileDialogVTable : __MicroComIModalWindowVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetFileTypesDelegate(void* @this, ushort cFileTypes, void* rgFilterSpec);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetFileTypes(void* @this, ushort cFileTypes, void* rgFilterSpec)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetFileTypes(cFileTypes, rgFilterSpec);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int SetFileTypeIndexDelegate(void* @this, ushort iFileType);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetFileTypeIndex(void* @this, ushort iFileType)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetFileTypeIndex(iFileType);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int GetFileTypeIndexDelegate(void* @this, ushort* piFileType);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetFileTypeIndex(void* @this, ushort* piFileType)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.FileTypeIndex;
                        *piFileType = __result;
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
        delegate int AdviseDelegate(void* @this, void* pfde, int* pdwCookie);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Advise(void* @this, void* pfde, int* pdwCookie)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Advise(pfde);
                        *pdwCookie = __result;
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
        delegate int UnadviseDelegate(void* @this, int dwCookie);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Unadvise(void* @this, int dwCookie)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Unadvise(dwCookie);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int SetOptionsDelegate(void* @this, FILEOPENDIALOGOPTIONS fos);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetOptions(void* @this, FILEOPENDIALOGOPTIONS fos)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetOptions(fos);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int GetOptionsDelegate(void* @this, FILEOPENDIALOGOPTIONS* pfos);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetOptions(void* @this, FILEOPENDIALOGOPTIONS* pfos)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Options;
                        *pfos = __result;
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
        delegate int SetDefaultFolderDelegate(void* @this, void* psi);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetDefaultFolder(void* @this, void* psi)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetDefaultFolder(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IShellItem>(psi, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int SetFolderDelegate(void* @this, void* psi);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetFolder(void* @this, void* psi)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetFolder(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IShellItem>(psi, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int GetFolderDelegate(void* @this, void** ppsi);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetFolder(void* @this, void** ppsi)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Folder;
                        *ppsi = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
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
        delegate int GetCurrentSelectionDelegate(void* @this, void** ppsi);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCurrentSelection(void* @this, void** ppsi)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CurrentSelection;
                        *ppsi = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
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
        delegate int SetFileNameDelegate(void* @this, System.Char* pszName);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetFileName(void* @this, System.Char* pszName)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetFileName(pszName);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int GetFileNameDelegate(void* @this, System.Char** pszName);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetFileName(void* @this, System.Char** pszName)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.FileName;
                        *pszName = __result;
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
        delegate int SetTitleDelegate(void* @this, System.Char* pszTitle);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetTitle(void* @this, System.Char* pszTitle)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetTitle(pszTitle);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int SetOkButtonLabelDelegate(void* @this, System.Char* pszText);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetOkButtonLabel(void* @this, System.Char* pszText)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetOkButtonLabel(pszText);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int SetFileNameLabelDelegate(void* @this, System.Char* pszLabel);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetFileNameLabel(void* @this, System.Char* pszLabel)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetFileNameLabel(pszLabel);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int GetResultDelegate(void* @this, void** ppsi);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetResult(void* @this, void** ppsi)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Result;
                        *ppsi = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
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
        delegate int AddPlaceDelegate(void* @this, void* psi, int fdap);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int AddPlace(void* @this, void* psi, int fdap)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.AddPlace(global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IShellItem>(psi, false), fdap);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int SetDefaultExtensionDelegate(void* @this, System.Char* pszDefaultExtension);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetDefaultExtension(void* @this, System.Char* pszDefaultExtension)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetDefaultExtension(pszDefaultExtension);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int CloseDelegate(void* @this, int hr);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Close(void* @this, int hr)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Close(hr);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int SetClientGuidDelegate(void* @this, System.Guid* guid);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetClientGuid(void* @this, System.Guid* guid)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetClientGuid(guid);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int ClearClientDataDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int ClearClientData(void* @this)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.ClearClientData();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
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
        delegate int SetFilterDelegate(void* @this, void* pFilter);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetFilter(void* @this, void* pFilter)
        {
            IFileDialog __target = null;
            try
            {
                {
                    __target = (IFileDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetFilter(pFilter);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::Avalonia.MicroCom.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIFileDialogVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, ushort, void*, int>)&SetFileTypes); 
#else
            base.AddMethod((SetFileTypesDelegate)SetFileTypes); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, ushort, int>)&SetFileTypeIndex); 
#else
            base.AddMethod((SetFileTypeIndexDelegate)SetFileTypeIndex); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, ushort*, int>)&GetFileTypeIndex); 
#else
            base.AddMethod((GetFileTypeIndexDelegate)GetFileTypeIndex); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int*, int>)&Advise); 
#else
            base.AddMethod((AdviseDelegate)Advise); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, int>)&Unadvise); 
#else
            base.AddMethod((UnadviseDelegate)Unadvise); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, FILEOPENDIALOGOPTIONS, int>)&SetOptions); 
#else
            base.AddMethod((SetOptionsDelegate)SetOptions); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, FILEOPENDIALOGOPTIONS*, int>)&GetOptions); 
#else
            base.AddMethod((GetOptionsDelegate)GetOptions); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetDefaultFolder); 
#else
            base.AddMethod((SetDefaultFolderDelegate)SetDefaultFolder); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetFolder); 
#else
            base.AddMethod((SetFolderDelegate)SetFolder); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetFolder); 
#else
            base.AddMethod((GetFolderDelegate)GetFolder); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetCurrentSelection); 
#else
            base.AddMethod((GetCurrentSelectionDelegate)GetCurrentSelection); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Char*, int>)&SetFileName); 
#else
            base.AddMethod((SetFileNameDelegate)SetFileName); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Char**, int>)&GetFileName); 
#else
            base.AddMethod((GetFileNameDelegate)GetFileName); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Char*, int>)&SetTitle); 
#else
            base.AddMethod((SetTitleDelegate)SetTitle); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Char*, int>)&SetOkButtonLabel); 
#else
            base.AddMethod((SetOkButtonLabelDelegate)SetOkButtonLabel); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Char*, int>)&SetFileNameLabel); 
#else
            base.AddMethod((SetFileNameLabelDelegate)SetFileNameLabel); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetResult); 
#else
            base.AddMethod((GetResultDelegate)GetResult); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int, int>)&AddPlace); 
#else
            base.AddMethod((AddPlaceDelegate)AddPlace); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Char*, int>)&SetDefaultExtension); 
#else
            base.AddMethod((SetDefaultExtensionDelegate)SetDefaultExtension); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, int>)&Close); 
#else
            base.AddMethod((CloseDelegate)Close); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, System.Guid*, int>)&SetClientGuid); 
#else
            base.AddMethod((SetClientGuidDelegate)SetClientGuid); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&ClearClientData); 
#else
            base.AddMethod((ClearClientDataDelegate)ClearClientData); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetFilter); 
#else
            base.AddMethod((SetFilterDelegate)SetFilter); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IFileDialog), new __MicroComIFileDialogVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIFileOpenDialogProxy : __MicroComIFileDialogProxy, IFileOpenDialog
    {
        public IShellItemArray Results
        {
            get
            {
                int __result;
                void* __marshal_ppenum = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_ppenum);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetResults failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IShellItemArray>(__marshal_ppenum, true);
            }
        }

        public IShellItemArray SelectedItems
        {
            get
            {
                int __result;
                void* __marshal_ppsai = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &__marshal_ppsai);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetSelectedItems failed", __result);
                return global::Avalonia.MicroCom.MicroComRuntime.CreateProxyOrNullFor<IShellItemArray>(__marshal_ppsai, true);
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::Avalonia.MicroCom.MicroComRuntime.Register(typeof(IFileOpenDialog), new Guid("D57C7288-D4AD-4768-BE02-9D969532D960"), (p, owns) => new __MicroComIFileOpenDialogProxy(p, owns));
        }

        protected __MicroComIFileOpenDialogProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComIFileOpenDialogVTable : __MicroComIFileDialogVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetResultsDelegate(void* @this, void** ppenum);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetResults(void* @this, void** ppenum)
        {
            IFileOpenDialog __target = null;
            try
            {
                {
                    __target = (IFileOpenDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Results;
                        *ppenum = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
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
        delegate int GetSelectedItemsDelegate(void* @this, void** ppsai);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSelectedItems(void* @this, void** ppsai)
        {
            IFileOpenDialog __target = null;
            try
            {
                {
                    __target = (IFileOpenDialog)global::Avalonia.MicroCom.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.SelectedItems;
                        *ppsai = global::Avalonia.MicroCom.MicroComRuntime.GetNativePointer(__result, true);
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

        protected __MicroComIFileOpenDialogVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetResults); 
#else
            base.AddMethod((GetResultsDelegate)GetResults); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetSelectedItems); 
#else
            base.AddMethod((GetSelectedItemsDelegate)GetSelectedItems); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::Avalonia.MicroCom.MicroComRuntime.RegisterVTable(typeof(IFileOpenDialog), new __MicroComIFileOpenDialogVTable().CreateVTable());
    }
}
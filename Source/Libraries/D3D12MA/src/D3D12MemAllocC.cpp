#include "D3D12MemAlloc.h"
using namespace D3D12MA;

#define EXPORT extern "C" __declspec(dllexport)

EXPORT HRESULT CreateVirtualBlock(const VIRTUAL_BLOCK_DESC* pDesc, VirtualBlock** ppVirtualBlock)
{
	return D3D12MA::CreateVirtualBlock(pDesc, ppVirtualBlock);
}

EXPORT ULONG VirtualBlock_Release(VirtualBlock* block)
{
	return block->Release();
}

EXPORT void VirtualBlock_Clear(VirtualBlock* block)
{
	block->Clear();
}

EXPORT HRESULT VirtualBlock_Allocate(VirtualBlock* block, VIRTUAL_ALLOCATION_DESC* pDesc, VirtualAllocation* pAllocation, UINT64* pOffset)
{
	return block->Allocate(pDesc, pAllocation, pOffset);
}

EXPORT void VirtualBlock_FreeAllocation(VirtualBlock* block, VirtualAllocation* pAllocation)
{
	block->FreeAllocation(*pAllocation);
}

EXPORT BOOL VirtualBlock_IsEmpty(VirtualBlock* block)
{
	return block->IsEmpty();
}

EXPORT void VirtualBlock_GetAllocationInfo(VirtualBlock* block, VirtualAllocation* pAllocation, VIRTUAL_ALLOCATION_INFO* pAllocationInfo)
{
	block->GetAllocationInfo(*pAllocation, pAllocationInfo);
}
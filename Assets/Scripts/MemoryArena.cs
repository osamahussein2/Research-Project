using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;

public unsafe class MemoryArena : IDisposable
{
    byte* memoryBlock;
    int offsetMemory;

    int memorySize;

    public MemoryArena(int sizeInBytes, int alignment)
    {
        // Allocate a memory block
        memoryBlock = (byte*)UnsafeUtility.Malloc(sizeInBytes, alignment, Allocator.Persistent);
        offsetMemory = 0; // No offset at the moment of initialization

        // Get the size in bytes
        memorySize = sizeInBytes;
    }

    public Pointer* Allocate<Pointer>(int count) where Pointer : unmanaged
    {
        // Get the size of the memory arena being allocated multiplied by the count
        int size = UnsafeUtility.SizeOf<Pointer>() * count;

        // Set the pointer to add a memory block by the offset memory
        Pointer* pointer = (Pointer*)(memoryBlock + offsetMemory);

        // Increment offset memory by size so that the next allocation won't override the previous one
        offsetMemory += size;

        // Return a pointer holding that memory data
        return pointer;
    }

    public void ResetOffsetMemory()
    {
        // Reset the offset memory to 0 to restart allocation
        offsetMemory = 0;
    }

    public int GetMemorySize()
    {
        // Return memory size
        return memorySize;
    }

    public void Dispose()
    {
        if (memoryBlock != null)
        {
            // Free memory block
            UnsafeUtility.Free(memoryBlock, Allocator.Persistent);
            memoryBlock = null;

            memorySize = 0;

#if DEBUG
            Debug.Log("Arena freed!");
#endif
        }
    }
}

using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

public class CameraPositionControler : MonoBehaviour
{
    //Shared Memory
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern SafeFileHandle OpenFileMapping(
     uint dwDesiredAccess,
     bool bInheritHandle,
     string lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr MapViewOfFile(
    SafeFileHandle hFileMappingObject,
    UInt32 dwDesiredAccess,
    UInt32 dwFileOffsetHigh,
    UInt32 dwFileOffsetLow,
    UIntPtr dwNumberOfBytesToMap);

    string szMapName = "UnityPerspectiveScreen";

    const UInt32 STANDARD_RIGHTS_REQUIRED = 0x000F0000;
    const UInt32 SECTION_QUERY = 0x0001;
    const UInt32 SECTION_MAP_WRITE = 0x0002;
    const UInt32 SECTION_MAP_READ = 0x0004;
    const UInt32 SECTION_MAP_EXECUTE = 0x0008;
    const UInt32 SECTION_EXTEND_SIZE = 0x0010;
    const UInt32 SECTION_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SECTION_QUERY |
        SECTION_MAP_WRITE |
        SECTION_MAP_READ |
        SECTION_MAP_EXECUTE |
        SECTION_EXTEND_SIZE);
    private SafeFileHandle sHandle;
    private IntPtr hHandle;
    private IntPtr pBuffer;
    bool attachSuccessful;

    public float look_speed = 2.0f;

    void Start()
    {
        sHandle = new SafeFileHandle(hHandle, true);
        attachSuccessful = Attach(szMapName, 256);
    }

    unsafe public bool Attach(string SharedMemoryName, UInt32 NumBytes)
    {
        if (!sHandle.IsInvalid) return false;
        sHandle = OpenFileMapping(SECTION_ALL_ACCESS, false, SharedMemoryName);
        if (sHandle.IsInvalid) return false;
        pBuffer = MapViewOfFile(sHandle, SECTION_ALL_ACCESS, 0, 0, new UIntPtr(NumBytes));
        return true;
    }

    unsafe public void Detach()
    {
        if (!sHandle.IsInvalid && !sHandle.IsClosed)
        {
            sHandle.Close();
        }
        pBuffer = IntPtr.Zero;
    }
    void Update()
    {
        // Lookat perspective screen incase attach is not successful
        GameObject perspective_screen = GameObject.Find("PerspectiveScreen");
        transform.LookAt(perspective_screen.transform.TransformPoint(new Vector3(0.0f, 0.0f, 0.5f)));

        if (!attachSuccessful)
        {
            attachSuccessful = Attach(szMapName, 256);
            return;
        }
        unsafe
        {
            
            float* s_memory = (float*)pBuffer.ToPointer();

            // X coordinate is inverted from C++ program to unity
            transform.position = new Vector3(-s_memory[0], s_memory[1], s_memory[2]);
            perspective_screen.transform.position = new Vector3(-s_memory[3], s_memory[4], s_memory[5]);
            perspective_screen.transform.rotation = Quaternion.Euler(new Vector3(s_memory[6], -s_memory[7], -s_memory[8]));
            perspective_screen.transform.localScale = new Vector3(s_memory[9], s_memory[10], s_memory[11]);
        }

        // Lookat the middle of the perspective screen, now that positions have been updated
        Quaternion target_rotation = Quaternion.LookRotation(perspective_screen.transform.TransformPoint(new Vector3(0.0f, 0.0f, 0.5f)) - transform.position);

        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, look_speed * Time.deltaTime);
    }

    void OnApplicationQuit()
    {
        if (attachSuccessful)
        {
            Detach();
        }
    }
}

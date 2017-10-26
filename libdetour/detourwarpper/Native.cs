using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibDetour
{
    // 顶点结构
    [StructLayout(LayoutKind.Explicit)]
    unsafe public struct DHVertex
    {
        [FieldOffset(0)]
        public float x; // x坐标
        [FieldOffset(4)]
        public float y; // y坐标
        [FieldOffset(8)]
        public float z; // z坐标
        public override string ToString()
        {
            return string.Format("({0},{1},{2})", x, y, z);
        }
    };

    // 三角型结构
    [StructLayout(LayoutKind.Explicit)]
    unsafe public struct DHTriangle
    {
        [FieldOffset(0)]
        public DHVertex vertices0;
        [FieldOffset(12)]
        public DHVertex vertices1;
        [FieldOffset(24)]
        public DHVertex vertices2;
        [FieldOffset(36)]
        public fixed int indices[3]; // 索引

        public override string ToString()
        {
            return string.Format("({0}-{1}-{2})", vertices0, vertices1, vertices2);
        }
    };
    public class Native
    {
        const string DLLNAME = "libdetour";
        [DllImport(DLLNAME, CharSet = CharSet.Auto)]
        public static extern IntPtr dh_create();
        [DllImport(DLLNAME, CharSet = CharSet.Auto)]
        public static extern void dh_delete(IntPtr dh);

        [DllImport(DLLNAME, CharSet = CharSet.Auto)]
        public static extern bool dh_load(IntPtr dh, IntPtr sb);

        [DllImport(DLLNAME, CharSet = CharSet.Auto)]
        public static extern void dh_unload(IntPtr dh);
        //

        //

        [DllImport(DLLNAME, CharSet = CharSet.Auto)]
        unsafe public static extern void dh_get_vectices(IntPtr dh, ref void* vectices, int* length);

        [DllImport(DLLNAME, CharSet = CharSet.Auto)]
        unsafe public static extern void dh_get_indices(IntPtr dh, ref void* indices, int* length);

        [DllImport(DLLNAME, CharSet = CharSet.Auto)]
        unsafe public static extern void dh_get_triangles(IntPtr dh, ref void* triangles, int* length);

        [DllImport(DLLNAME, CharSet = CharSet.Auto)]
        unsafe public static extern bool dh_findpath(IntPtr dh, ref DHVertex start, ref DHVertex end, void* path, ref int length);

        [DllImport(DLLNAME, CharSet = CharSet.Auto)]
        unsafe public static extern bool dh_find_straight_path(IntPtr dh, ref DHVertex start, ref DHVertex end, void* path, ref int length);
        


        [DllImport(DLLNAME, CharSet = CharSet.Auto)]
        unsafe public static extern void* dh_createpath();



        [DllImport(DLLNAME, CharSet = CharSet.Auto)]
        unsafe public static extern void dh_deletepath(void* path);


        [DllImport(DLLNAME, CharSet = CharSet.Auto)]
        unsafe public static extern DHVertex* dh_getpathdata(void* path);
    }
}

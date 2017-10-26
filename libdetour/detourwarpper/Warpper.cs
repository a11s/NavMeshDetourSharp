using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibDetour
{
    public unsafe class Warpper : IDisposable
    {
        IntPtr dh = IntPtr.Zero;

        bool mapLoaded = false;

        public bool IsNull
        {
            get
            {
                return dh == IntPtr.Zero;
            }
        }

        public bool MapLoaded
        {
            get
            {
                return mapLoaded;
            }
        }

        public Warpper()
        {
            dh = Native.dh_create();

        }
        public bool Load(string bytesFileName)
        {
            if (IsNull)
            {
                return false;
            }
            if (mapLoaded)
            {
                Unload();
            }
            var hs = Marshal.StringToHGlobalAnsi(bytesFileName);

            try
            {
                mapLoaded = Native.dh_load(dh, hs);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Marshal.FreeHGlobal(hs);
            }
            return mapLoaded;
        }

        public void Unload()
        {
            if (IsNull)
            {
                return;
            }
            Native.dh_unload(dh);
            mapLoaded = false;
        }

        public bool FindPath(bool isStraight, DHVertex start, DHVertex end, DHPath result)
        {
            if (IsNull)
            {
                return false;
            }
            int len = 0;
            bool res = false;
            if (isStraight)
            {
                res = Native.dh_find_straight_path(dh, ref start, ref end, result, ref len);
            }
            else
            {
                res = Native.dh_findpath(dh, ref start, ref end, result, ref len);
            }
            if (res)
            {
                result.Length = len;
            }
            return res;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                if (!IsNull)
                {
                    if (mapLoaded)
                    {
                        Unload();
                    }
                    Native.dh_delete(dh);
                    dh = IntPtr.Zero;
                }
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DetourWarpper() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }


        #endregion
        public DHTriangle[] GetTriangles()
        {
            if (IsNull || !mapLoaded)
            {
                return null;
            }
            void* vuv1 = (void*)0;
            int size1 = 0;
            Native.dh_get_triangles(dh, ref vuv1, &size1);
            var ret = new DHTriangle[size1];
            for (int i = 0; i < size1; i++)
            {
                DHTriangle* ut = (DHTriangle*)vuv1;
                ret[i] = ut[i];
            }
            return ret;

        }

        public int[] GetIndices()
        {
            if (IsNull || !mapLoaded)
            {
                return null;
            }
            void* vuv1 = (void*)0;
            int size1 = 0;
            Native.dh_get_indices(dh, ref vuv1, &size1);
            var ret = new int[size1];
            for (int i = 0; i < size1; i++)
            {
                int* ut = (int*)vuv1;
                ret[i] = ut[i];
            }
            return ret;
        }

        public DHVertex[] GetVertexes()
        {
            if (IsNull || !mapLoaded)
            {
                return null;
            }
            void* vuv1 = (void*)0;
            int size1 = 0;
            Native.dh_get_vectices(dh, ref vuv1, &size1);
            var ret = new DHVertex[size1];
            for (int i = 0; i < size1; i++)
            {
                DHVertex* ut = (DHVertex*)vuv1;
                ret[i] = ut[i];
            }
            return ret;
        }

    }
}

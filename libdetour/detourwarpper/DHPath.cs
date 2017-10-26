using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDetour
{

    public unsafe class DHPath : IDisposable
    {
        void* path = null;

        public bool IsNull
        {
            get
            {
                return new IntPtr(path) == IntPtr.Zero;
            }
        }

        public void* Path
        {
            get
            {
                return path;
            }
        }
        /// <summary>
        /// get cached vertex length( findpath fill this field)
        /// </summary>
        public int Length { get; internal set; }
        /// <summary>
        /// get vector ptr and length, no copy
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public unsafe DHVertex* GetPathDataPtr(out int len)
        {
            len = Length;
            if (IsNull)
            {
                len = 0;
                return null;
            }
            DHVertex* vertices = Native.dh_getpathdata(path);
            return vertices;
        }
        /// <summary>
        /// copy path data to array
        /// </summary>
        /// <returns></returns>
        public DHVertex[] GetPathData()
        {
            if (IsNull)
            {
                return null;
            }
            DHVertex* vertices = Native.dh_getpathdata(path);
            var ret = new DHVertex[Length];
            for (int i = 0; i < Length; i++)
            {
                ret[i] = vertices[i];
            }
            return ret;
        }


        public DHPath()
        {
            path = Native.dh_createpath();
            if (IsNull)
            {
                //create failed;
                return;
            }
        }
        public static implicit operator void* (DHPath d)
        {
            return d.Path;
        }

        //public static implicit operator DHPath(void* ptr)
        //{
        //    return new DHPath(ptr);
        //}


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
                    Native.dh_deletepath(path);
                    path = null;
                }
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DHPath() {
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



    }
}

using HelixToolkit.Wpf.SharpDX;
using LibDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo
{
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using DiffuseMaterial=System.Windows.Media.Media3D.DiffuseMaterial;
    using Color = System.Windows.Media.Color;
    using SolidColorBrush = System.Windows.Media.SolidColorBrush;
     public class MapReader
    {
        public class navMeshInfo
        {
            internal navNode[] nodes;
            internal Point3D[] vecs;
        }
        public class navNode
        {
            public int nodeID;
            internal int[] poly;
        }
        public static navMeshInfo LoadMeshInfoFromFiles(string bytesFileName)
        {
            navMeshInfo info = new navMeshInfo();
            List<Point3D> listVec = new List<Point3D>();

            string s1, s2;
            var lines = System.IO.File.ReadAllLines(bytesFileName, Encoding.UTF8).Where(a => !string.IsNullOrWhiteSpace(a) && !a.StartsWith("--")).ToArray();
            s1 = lines[0];
            s2 = lines[1];

            string[] arr1 = s1.Split(new char[] { ',', ';' });
            string[] arr2 = s2.Split(new char[] { ',', ';' });

            string[] arrvertices;
            string[] arrindices;
            int ttttttt;
            if (arr1.All(a => int.TryParse(a, out ttttttt)))
            {
                //如果全都是整数,说明是索引
                arrvertices = arr2;
                arrindices = arr1;
            }
            else
            {
                arrvertices = arr1;
                arrindices = arr2;
            }

            //张建是用逗号分开的
            var svecs = arrvertices.Select(a => double.Parse(a)).ToArray();
            var snodes = arrindices.Select(a => int.Parse(a)).ToArray();
            for (int i = 0; i < svecs.Length; i += 3)
            {

                Point3D v3 = new Point3D();
                v3.X = svecs[i];
                v3.Y = svecs[i + 1];
                v3.Z = svecs[i + 2];
                listVec.Add(v3);
            }
            info.vecs = listVec.ToArray();

            List<navNode> polys = new List<navNode>();
            int index = 0;
            //因为一定是三角形
            for (int i = 0; i < snodes.Length; i += 3)
            {
                int k = 0;
                navNode node = new navNode();
                List<int> points = new List<int>();
                points.Add(snodes[i]);
                points.Add(snodes[i + 1]);
                points.Add(snodes[i + 2]);
                node.nodeID = index;
                index++;
                List<int> poly = new List<int>();
                poly.Add(points[0]);
                poly.Add(points[1]);
                poly.Add(points[2]);

                node.poly = poly.ToArray();
                //node.genBorder();//这里生成的border 是顶点border
                //node.genCenter(info);
                polys.Add(node);
            }
            info.nodes = polys.ToArray();
            //info.calcBound();
            //info.genBorder();
            System.Diagnostics.Debug.WriteLine("顶点数" + info.vecs.Length + "\t多边形数:" + info.nodes.Length);
            return info;
        }

        public static IntPtr UG;

        public static IntPtr CurrentPath = IntPtr.Zero;

        public static System.Windows.Media.Media3D.GeometryModel3D MakeTile(List<Point3D> allPoints, List<int> allIndices, Color color)
        {
            System.Windows.Media.Media3D.MeshGeometry3D g3d = new System.Windows.Media.Media3D.MeshGeometry3D();
            for (int i = 0; i < allPoints.Count; i++)
            {
                g3d.Positions.Add(allPoints[i]);
            }
            for (int i = 0; i < allIndices.Count; i++)
            {
                g3d.TriangleIndices.Add(allIndices[i]);
            }
            DiffuseMaterial dm = new DiffuseMaterial(new SolidColorBrush(color));

            var ret = new System.Windows.Media.Media3D.GeometryModel3D(g3d, dm);
            return ret;
        }
        /// <summary>
        /// 用于不重复的顶点,自动按照顺序生成index
        /// </summary>
        /// <param name="allPoints"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static System.Windows.Media.Media3D.GeometryModel3D MakeTile(List<Point3D> allPoints, Color color)
        {
            var allIndices = new List<int>();
            for (int i = 0; i < allPoints.Count; i++)
            {
                allIndices.Add(i);
            }
            var g3d = new System.Windows.Media.Media3D.MeshGeometry3D();
            for (int i = 0; i < allPoints.Count; i++)
            {
                g3d.Positions.Add(allPoints[i]);
            }
            for (int i = 0; i < allIndices.Count; i++)
            {
                g3d.TriangleIndices.Add(allIndices[i]);
            }
            var dm = new System.Windows.Media.Media3D.DiffuseMaterial(new SolidColorBrush(color));

            var ret = new System.Windows.Media.Media3D.GeometryModel3D(g3d, dm);
            return ret;
        }
        //InteractiveVisual3D
        //public static InteractiveVisual3D MakeInteractiveTile(List<Point3D> allPoints, Color color, string caption)
        //{
        //    var allIndices = new List<int>();
        //    for (int i = 0; i < allPoints.Count; i++)
        //    {
        //        allIndices.Add(i);
        //    }
        //    System.Windows.Media.Media3D.MeshGeometry3D g3d = new System.Windows.Media.Media3D.MeshGeometry3D();
        //    for (int i = 0; i < allPoints.Count; i++)
        //    {
        //        g3d.Positions.Add(allPoints[i]);
        //    }
        //    for (int i = 0; i < allIndices.Count; i++)
        //    {
        //        g3d.TriangleIndices.Add(allIndices[i]);
        //    }
        //    DiffuseMaterial dm = new DiffuseMaterial(new SolidColorBrush(color));

        //    var ret = new InteractiveVisual3D();
        //    ret.Material = dm;
        //    ret.Geometry = g3d;
        //    ret.Visual = new System.Windows.Controls.TextBlock() { Text = caption };
        //    return ret;
        //}
        public static SharpDX.Vector3 P2V(Point3D p3)
        {
            return new SharpDX.Vector3((float)p3.X, (float)p3.Y, (float)p3.Z);
        }

        public static SharpDX.Vector3 U2V(DHVertex p3)
        {
            return new SharpDX.Vector3((float)p3.x, (float)p3.y, (float)p3.z);
        }
        public static MeshGeometryModel3D MakeMeshGeometryModel3D(List<Point3D> allPoints, HelixToolkit.Wpf.SharpDX.Material mat)
        {
            var allIndices = new List<int>();
            for (int i = 0; i < allPoints.Count; i++)
            {
                allIndices.Add(i);
            }
            var g3d = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();
            g3d.Positions = new HelixToolkit.Wpf.SharpDX.Core.Vector3Collection();
            g3d.TriangleIndices = new HelixToolkit.Wpf.SharpDX.Core.IntCollection();
            g3d.Normals = new HelixToolkit.Wpf.SharpDX.Core.Vector3Collection();
            g3d.Indices = new HelixToolkit.Wpf.SharpDX.Core.IntCollection();
            for (int i = 0; i < allPoints.Count; i++)
            {
                var p3 = allPoints[i];
                SharpDX.Vector3 v3 = P2V(p3);
                g3d.Positions.Add(v3);
                g3d.Normals.Add(new SharpDX.Vector3(0, 1, 0));
            }
            for (int i = 0; i < allIndices.Count; i++)
            {
                g3d.Indices.Add(allIndices[i]);
            }
            var ret = new MeshGeometryModel3D();
            ret.Material = mat;
            ret.Geometry = g3d;

            //ret. = new System.Windows.Controls.TextBlock() { Text = caption };

            return ret;
        }


        public static MeshGeometryModel3D MakeMeshGeometryModel3DUnity(DHTriangle triangle, HelixToolkit.Wpf.SharpDX.Material mat)
        {
            var allIndices = new List<int>(new int[] { 0, 1, 2 });

            var g3d = new HelixToolkit.Wpf.SharpDX.MeshGeometry3D();
            g3d.Positions = new HelixToolkit.Wpf.SharpDX.Core.Vector3Collection();
            g3d.TriangleIndices = new HelixToolkit.Wpf.SharpDX.Core.IntCollection();
            g3d.Normals = new HelixToolkit.Wpf.SharpDX.Core.Vector3Collection();
            g3d.Indices = new HelixToolkit.Wpf.SharpDX.Core.IntCollection();


            g3d.Positions.Add(U2V(triangle.vertices0));
            g3d.Positions.Add(U2V(triangle.vertices1));
            g3d.Positions.Add(U2V(triangle.vertices2));
            g3d.Normals.Add(new SharpDX.Vector3(0, 1, 0));
            g3d.Normals.Add(new SharpDX.Vector3(0, 1, 0));
            g3d.Normals.Add(new SharpDX.Vector3(0, 1, 0));

            for (int i = 0; i < allIndices.Count; i++)
            {
                g3d.Indices.Add(allIndices[i]);
            }
            var ret = new MeshGeometryModel3D();
            ret.Material = mat;
            ret.Geometry = g3d;

            //ret. = new System.Windows.Controls.TextBlock() { Text = caption };

            return ret;
        }

        private static void Ret_MouseDown3D(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine(e.Source.ToString());
        }
    }
}

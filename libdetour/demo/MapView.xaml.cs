using HelixToolkit.Wpf.SharpDX;
using LibDetour;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace demo
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        /// <summary>
        /// The ViewModel
        /// </summary>
        MainViewModel mViewModel=new MainViewModel();

        void SetupViewModel()
        {



            // Setup the Camera and raise the EventHandler once at the Start
            // X: 24.647, Y: 52.383, Z: 72.829, view direction X: -0.019, Y: -2.411, Z: -2.249
            mViewModel.modelView = viewport3D;
            mViewModel.Camera = viewport3D.Camera;
            mViewModel.Camera.Position = new Point3D(24.647, 52.383, 72.829);
            mViewModel.Camera.LookDirection = new Vector3D(-0.019, -2.411, -2.249);
            mViewModel.Grid = LineBuilderExt.GenerateGrid(Vector3.UnitY, -0, 50, -0, 50, 5, 5);
            mViewModel.Camera.Changed += Camera_Changed;
            Camera_Changed(mViewModel.Camera, new EventArgs());
        }
        public MapView()
        {
            InitializeComponent();
            SetupViewModel();

            this.DataContext = mViewModel;
            mViewModel.GroundGeometry = this.groundGroup.Children;
            mViewModel.PlayersGeometry = this.playersGroup.Children;
        }

        void Camera_Changed(object sender, EventArgs e)
        {
            var cam = (HelixToolkit.Wpf.SharpDX.PerspectiveCamera)sender;
            var pos = String.Format("X: {0:0.###}, ", cam.Position.X) +
                      String.Format("Y: {0:0.###}, ", cam.Position.Y) +
                      String.Format("Z: {0:0.###}", cam.Position.Z);
            var dir = String.Format("X: {0:0.###}, ", cam.LookDirection.X) +
                      String.Format("Y: {0:0.###}, ", cam.LookDirection.Y) +
                      String.Format("Z: {0:0.###}", cam.LookDirection.Z);
            // Set the Label Content
            statusLabel.Content = "View from " + pos + ", view direction " + dir;
            
        }


        MeshGeometryModel3D StartBall = null;
        MeshGeometryModel3D EndBall = null;
        LineGeometryModel3D WayPath = null;
        /// <summary>
        /// set start point or end point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewport3D_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition((IInputElement)sender);
            var p = viewport3D.FindNearestPoint(point);
            if (!p.HasValue) return;


            if (StartBall == null)
            {
                StartBall = CreateBall(PhongMaterials.Green);

            }
            if (EndBall == null)
            {
                EndBall = CreateBall(PhongMaterials.Red);

            }
            if (WayPath == null)
            {
                WayPath = new LineGeometryModel3D();
                WayPath.Color = SharpDX.Color.Blue;
                mViewModel.PlayersGeometry.Add(WayPath);
                WayPath.Attach(mViewModel.modelView.RenderHost);
            }
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                EndBall.Transform = new TranslateTransform3D(p.Value.X, p.Value.Y , p.Value.Z);

            }
            else
            {
                StartBall.Transform = new TranslateTransform3D(p.Value.X, p.Value.Y , p.Value.Z);
            }
            RenderPath();

        }

        private void RenderPath()
        {
            LineBuilder lb = new LineBuilder();
            var startv3 = StartBall.Transform.ToVector3();
            var endv3 = EndBall.Transform.ToVector3();

            var start = new DHVertex() { x = startv3.X, y = startv3.Y, z = startv3.Z };
            var end = new DHVertex() { x = endv3.X, y = endv3.Y, z = endv3.Z };
            DHPath path = new DHPath();
            if (dh.FindPath(true, start, end, path))
            {
                if (path.Length > 1)
                {
                    var vs = path.GetPathData();
                    for (int i = 0; i < path.Length - 1; i++)
                    {

                        Vector3 v = new Vector3((float)vs[i].x, (float)vs[i].y, (float)vs[i].z);
                        Vector3 nv = new Vector3((float)vs[i + 1].x, (float)vs[i + 1].y, (float)vs[i + 1].z);
                        lb.AddLine(new Vector3(v.X, v.Y + 1, v.Z), new Vector3(nv.X, nv.Y + 1, nv.Z));
                    }
                }
                WayPath.Geometry = lb.ToLineGeometry3D();

            }
            //playersGroup.Children.Clear();
            //playersGroup.Children.Add(WayPath);
        }

        private MeshGeometryModel3D CreateBall(HelixToolkit.Wpf.SharpDX.Material mat)
        {
            MeshGeometryModel3D mgm = new MeshGeometryModel3D();
            MeshBuilder mb = new MeshBuilder();
            //mb.AddSphere(MapReader.P2V(p), 2);
            mb.AddSphere(Vector3.Zero, 0.5);
            //mb.AddArrow(Vector3.Zero, new Vector3(0, 0, 2), 0.1);

            mgm.Geometry = mb.ToMeshGeometry3D();
            mgm.Material = mat;
            mViewModel.PlayersGeometry.Add(mgm);

            mgm.Attach(mViewModel.modelView.RenderHost);
            return mgm;
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            lineTriangulatedPolygon.Geometry = mViewModel.LineGeometry;

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            lineTriangulatedPolygon.Geometry = null;
        }

        Warpper dh = new Warpper();

        public void LoadMap(string bytesFileName)
        {
            bool succ = false;
            if (dh.MapLoaded)
            {
                dh.Unload();
            }
            succ = dh.Load(bytesFileName);
            if (succ)
            {
                DHTriangle[] triangles = dh.GetTriangles();
                DHVertex[] verts = dh.GetVertexes();
                var lb = new LineBuilder();
                groundGroup.Children.Clear();
                mViewModel.ModelTransform = new TranslateTransform3D();
                for (int i = 0; i < triangles.Length; i++)
                {
                    var node = triangles[i];

                    lb.AddLine(MapReader.U2V(node.vertices0), MapReader.U2V(node.vertices1));
                    lb.AddLine(MapReader.U2V(node.vertices1), MapReader.U2V(node.vertices2));
                    lb.AddLine(MapReader.U2V(node.vertices2), MapReader.U2V(node.vertices0));

                    MeshGeometryModel3D mgm = MapReader.MakeMeshGeometryModel3DUnity(node, mViewModel.Material);

                    groundGroup.Children.Add(mgm);
                    mgm.Attach(mViewModel.modelView.RenderHost);

                }
                mViewModel.LineGeometry = lb.ToLineGeometry3D();
                lineTriangulatedPolygon.Geometry = mViewModel.LineGeometry;

            }

            //icon = AddOrUpdatePlayerPosition("player", 0, 0, 0, 0.5f, 0, 0, 0);
            //icon.LastUpdateTime = DateTime.Now.AddSeconds(1);
            //Ray r = new Ray(mViewModel.Camera.Position,);



        }
    }
}

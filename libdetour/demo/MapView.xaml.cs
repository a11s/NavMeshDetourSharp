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
        MainViewModel mViewModel;
        public MapView()
        {
            InitializeComponent();

            mViewModel = new MainViewModel();
            this.DataContext = mViewModel;
            mViewModel.modelView = viewport3D;
            mViewModel.Camera = viewport3D.Camera;

            // Setup the Camera and raise the EventHandler once at the Start
            // X: 24.647, Y: 52.383, Z: 72.829, view direction X: -0.019, Y: -2.411, Z: -2.249
            mViewModel.Camera.Position = new Point3D(24.647, 52.383, 72.829);
            mViewModel.Camera.LookDirection = new Vector3D(-0.019, -2.411, -2.249);
            mViewModel.Grid = LineBuilderExt.GenerateGrid(Vector3.UnitY, -0, 50, -0, 50, 5, 5);
            mViewModel.Camera.Changed += Camera_Changed;
            Camera_Changed(mViewModel.Camera, new EventArgs());
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
        /// <summary>
        /// set start point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewport3D_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = viewport3D.FindNearestPoint(e.GetPosition((IInputElement)sender));
            if (!p.HasValue) return;
        }
        /// <summary>
        /// set end point
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewport3D_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

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

                //mViewModel.GroundGeometry.Clear();
                //groundGroup.Children.Clear();
                mViewModel.ModelTransform = new TranslateTransform3D();
                for (int i = 0; i < triangles.Length; i++)
                {
                    var node = triangles[i];

                    lb.AddLine(MapReader.U2V(node.vertices0), MapReader.U2V(node.vertices1));
                    lb.AddLine(MapReader.U2V(node.vertices1), MapReader.U2V(node.vertices2));
                    lb.AddLine(MapReader.U2V(node.vertices2), MapReader.U2V(node.vertices0));

                    MeshGeometryModel3D mgm = MapReader.MakeMeshGeometryModel3DUnity(node, mViewModel.Material);

                    //mgm.HitTest()
                    //mViewModel.GroundGeometry.Add(mgm);
                    //groundGroup.Children.Add(mgm);
                    mgm.Attach(mViewModel.modelView.RenderHost);

                }
                
                lineTriangulatedPolygon.Geometry = lb.ToLineGeometry3D();

            }

            //icon = AddOrUpdatePlayerPosition("player", 0, 0, 0, 0.5f, 0, 0, 0);
            //icon.LastUpdateTime = DateTime.Now.AddSeconds(1);
            //Ray r = new Ray(mViewModel.Camera.Position,);



        }
    }
}

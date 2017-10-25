using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;

namespace demo
{

    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    public class MainViewModel
    {
        public Viewport3DX modelView { get;  set; }
        public HelixToolkit.Wpf.SharpDX.Camera Camera { get;  set; }
        public LineGeometry3D Grid { get;  set; }

        public Color GridColor { get; set; }
        public TranslateTransform3D GridTransform { get; set; }
        public Element3DCollection GroundGeometry { get; set; }
        public Element3DCollection PlayersGeometry { get; set; }
        public LineGeometry3D LineGeometry { get; set; }
        public double LineThickness { get; set; }
        public double TriangulationThickness { get; set; }
        public bool ShowTriangleLines { get; set; }
        public int PointCount { get; set; }
        public Color4 AmbientLightColor { get; set; }
        public Color DirectionalLightColor { get; set; }
        public Vector3 DirectionalLightDirection { get; set; }
        public TranslateTransform3D ModelTransform { get; set; }
        public TranslateTransform3D ModelLineTransform { get; set; }
        public PhongMaterial Material { get; set; }
        public Color TriangulationColor { get; set; }

        public MainViewModel()
        {
            // Lines Setup
            this.LineThickness = 1d;
            this.TriangulationThickness = .5;
            this.ShowTriangleLines = true;

            // Count Setup
            this.PointCount = 1000;

            // Lighting Setup
            this.AmbientLightColor = new Color4(.1f, .1f, .1f, 1.0f);
            this.DirectionalLightColor = Color.White;
            this.DirectionalLightDirection = new Vector3(0, -1, 0);

            // Model Transformations
            this.ModelTransform = new TranslateTransform3D(0, 0, 0);
            this.ModelLineTransform = new TranslateTransform3D(0, 0.001, 0);

            // Model Materials and Colors
            this.Material = PhongMaterials.PolishedBronze;
            this.TriangulationColor = SharpDX.Color.Black;

            // Grid Setup
            this.Grid = LineBuilder.GenerateGrid(Vector3.UnitY, -5, 5, 0, 10);
            this.GridColor = SharpDX.Color.DarkGray;
            this.GridTransform = new TranslateTransform3D(0, -0.01, 0);


            this.GroundGeometry = new Element3DCollection();
            this.PlayersGeometry = new Element3DCollection();
        }
    }
}

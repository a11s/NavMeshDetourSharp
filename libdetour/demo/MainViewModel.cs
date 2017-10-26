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
    using Matrix = SharpDX.Matrix;
    using Media3D = System.Windows.Media.Media3D;
    using System.Windows.Media.Animation;
    public class MainViewModel
    {
        private bool isAnimated;

        public Viewport3DX modelView { get; set; }
        public HelixToolkit.Wpf.SharpDX.Camera Camera { get; set; }
        public PhongMaterial RedMaterial { get; private set; }
        public PhongMaterial GreenMaterial { get; private set; }
        public PhongMaterial BlueMaterial { get; private set; }
        public LineGeometry3D Grid { get; set; }

        public Color GridColor { get; set; }
        public TranslateTransform3D GridTransform { get; set; }
        //public Element3DCollection GroundGeometry { get; set; }
        //public Element3DCollection PlayersGeometry { get; set; }
        public ObservableElement3DCollection GroundGeometry { get; set; }
        public ObservableElement3DCollection PlayersGeometry { get; set; }

        public DefaultRenderTechniquesManager RenderTechniquesManager { get; private set; }
        public RenderTechnique RenderTechnique { get; private set; }
        public DefaultEffectsManager EffectsManager { get; private set; }
        public LineGeometry3D LineGeometry { get; set; }
        public double LineThickness { get; set; }
        public double TriangulationThickness { get; set; }
        public bool ShowTriangleLines { get; set; }
        public int PointCount { get; set; }
        public Color4 AmbientLightColor { get; set; }
        public Color DirectionalLightColor { get; set; }
        public Vector3 DirectionalLightDirection { get; set; }
        public object LightDirectionTransform { get; private set; }
        public Vector2 ShadowMapResolution { get; private set; }
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
            this.AmbientLightColor = new Color4(1f, 1f, 1f, 1.0f);
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


            //this.GroundGeometry = new Element3DCollection();
            //this.PlayersGeometry = new Element3DCollection();


            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            RenderTechnique = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);



            // setup lighting            

            this.LightDirectionTransform = CreateAnimatedTransform(-DirectionalLightDirection.ToVector3D(), new Vector3D(0, 1, -1), 24);
            this.ShadowMapResolution = new Vector2(2048, 2048);

            // camera setup
            this.Camera = new PerspectiveCamera { Position = (Point3D)(-DirectionalLightDirection.ToVector3D()), LookDirection = DirectionalLightDirection.ToVector3D(), UpDirection = new Vector3D(0, 1, 0) };

            // floor plane grid
            //Grid = LineBuilder.GenerateGrid();
            //GridColor = SharpDX.Color.Black;
            //GridTransform = new Media3D.TranslateTransform3D(-5, -1, -5);

            // scene model3d
            //var b1 = new MeshBuilder();
            //b1.AddSphere(new Vector3(0, 0, 0), 0.5);
            //b1.AddBox(new Vector3(0, 0, 0), 1, 0.25, 2, BoxFaces.All);
            //Model = b1.ToMeshGeometry3D();
            ////Instances = new[] { Matrix.Translation(0, 0, -1.5f), Matrix.Translation(0, 0, 1.5f) };

            //var b2 = new MeshBuilder();
            //b2.AddBox(new Vector3(0, 0, 0), 10, 0, 10, BoxFaces.PositiveY);
            //Plane = b2.ToMeshGeometry3D();
            //PlaneTransform = new Media3D.TranslateTransform3D(-0, -2, -0);
            //GrayMaterial = PhongMaterials.LightGray;
            ////GrayMaterial.TextureMap = new BitmapImage(new System.Uri(@"TextureCheckerboard2.jpg", System.UriKind.RelativeOrAbsolute)); 

            //// lines model3d            
            //Lines = LineBuilder.GenerateBoundingBox(Model);

            //// model trafos
            //Model1Transform = new Media3D.TranslateTransform3D(0, 0, 0);
            //Model2Transform = new Media3D.TranslateTransform3D(-2, 0, 0);
            //Model3Transform = new Media3D.TranslateTransform3D(+2, 0, 0);

            // model materials
            RedMaterial = PhongMaterials.Glass;
            GreenMaterial = PhongMaterials.Green;
            BlueMaterial = PhongMaterials.Blue;

        }
        private System.Windows.Media.Media3D.Transform3D CreateAnimatedTransform(Vector3D translate, Vector3D axis, double speed = 4)
        {
            var lightTrafo = new Media3D.Transform3DGroup();
            lightTrafo.Children.Add(new Media3D.TranslateTransform3D(translate));

            var rotateAnimation = new Rotation3DAnimation
            {
                RepeatBehavior = RepeatBehavior.Forever,
                //By = new Media3D.AxisAngleRotation3D(axis, 180),
                From = new Media3D.AxisAngleRotation3D(axis, 135),
                To = new Media3D.AxisAngleRotation3D(axis, 225),
                AutoReverse = true,
                Duration = TimeSpan.FromSeconds(speed / 4),
                //IsCumulative = true,                  
            };

            var rotateTransform = new Media3D.RotateTransform3D();
            rotateTransform.BeginAnimation(Media3D.RotateTransform3D.RotationProperty, rotateAnimation);
            lightTrafo.Children.Add(rotateTransform);
            return lightTrafo;
        }

        private void OnAnimatedChanged(bool value)
        {
            this.isAnimated = value;
            if (value)
            {
                this.LightDirectionTransform = CreateAnimatedTransform(-DirectionalLightDirection.ToVector3D(), new Vector3D(0, 1, -1), 24);
            }
            else
            {
                this.LightDirectionTransform = System.Windows.Media.Media3D.Transform3D.Identity;
            }
        }
    }
}

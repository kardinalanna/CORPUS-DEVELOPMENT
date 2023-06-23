// Copyright (C) ANSYS.  All Rights Reserved.
using System.Drawing;
using SpaceClaim.Api.V23.Modeler;
using SpaceClaim.Api.V23.Geometry;
using SpaceClaim.Api.V23.Extensibility;
using SpaceClaim.Api.V23;
using CreateBody.Properties;
using Ansys.Discovery.Api.V23.Physics.Materials;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Point = SpaceClaim.Api.V23.Geometry.Point;
using System.Diagnostics;
using Ansys.Discovery.Api.V23.Solution;
using Ansys.Discovery.Api.V23.Units;
using Ansys.Discovery.Api.V23.Physics.Conditions;
using Material = Ansys.Discovery.Api.V23.Physics.Materials.Material;
using SpaceClaim.Api.V23.Scripting.Extensions;
using SpaceClaim.Api.V23.Scripting.Selection;
using Body = SpaceClaim.Api.V23.Modeler.Body;
using Face = SpaceClaim.Api.V23.Modeler.Face;
using SpaceClaim.Api.V23.Scripting.Commands;
using Ansys.Discovery.Api.V23.Application;
using SpaceClaim.Collections;
using SpaceClaim.Api.V23.Scripting.Commands.CommandOptions;
using SpaceClaim.Api.V23.Unsupported.RuledCutting;


namespace CreateBody
{
    // Every tools and actions should be captured by commands. 
    public class CreateBlockCapsule : CommandCapsule
    {
        public List<double> mainBoxSize = new List<double>();
        public Vector mainBoxLocation = Vector.Create(0, 0, 0);
        public List<double> mainBoxBounds = new List<double>();
        public List<Sensor> sensorList = new List<Sensor>();
        public List<MountPoint> mountPoints = new List<MountPoint>();
        public List<DesignFace> suppurtFacesList = new List<DesignFace>();
        public List<Restrictor> restrictorsList = new List<Restrictor>();
        public List<Sensor> forceSensorsList = new List<Sensor>();
        public String material = "Plastic, PP (homopolymer, low flow)";
        public int number = 1;
        bool forceExist = false;

        public const string CommandName = "CreateBody.CreateBlock";

        public CreateBlockCapsule()
            : base(CommandName, Resources.ButtonText, Resources.CreateBlock, "Creation and starting simulation")
        {
        }

        protected override void OnUpdate(Command command)
        {
            Window window = Window.ActiveWindow;
            command.IsEnabled = window != null;
        }

        public void ParserMainBox(dynamic parametrs)
        {
            try
            {
                if (parametrs.mainBoxSize == null)
                    throw new Exception("MainBoxSize size not specified");
                foreach (double size in parametrs.mainBoxSize)
                    mainBoxSize.Add(CheckNotNegative(size));
                if (mainBoxSize.Count != 3)
                    throw new Exception("Invalid mainBoxSize. Specify 3 parameters for the MainBoxSize (length, width and height)");
                if (parametrs.mainBoxLocation != null)
                {
                    List<double> loc = new List<double>();
                    foreach (double l in parametrs.mainBoxLocation)
                        loc.Add(l);
                    if (loc.Count != 3)
                        throw new Exception("Invalid mainBoxLocation. Specify 3 parameters for the mainBoxLocation");
                    mainBoxLocation = Vector.Create(loc[0], loc[1], loc[2]);

                }
                double startX = Math.Round(mainBoxLocation.X + mainBoxSize[0] / 2, 3);
                double endX = Math.Round(mainBoxLocation.X - mainBoxSize[0] / 2, 3);
                double startY = Math.Round(mainBoxLocation.Y + mainBoxSize[1] / 2, 3);
                double endY = Math.Round(mainBoxLocation.Y - mainBoxSize[1] / 2, 3);
                double topZ = Math.Round(mainBoxLocation.Z + mainBoxSize[2] / 2, 3);
                double bottomZ = Math.Round(mainBoxLocation.Z - mainBoxSize[2] / 2, 3);

                mainBoxBounds = new List<double>()
                {
                startX,
                endX,
                startY,
                endY,
                topZ,
                bottomZ
                };
            }
            catch (Exception e)
            {
                Debug.Fail(e.Message);
            }
        }


        public double CheckNotNegative(double size)
        {
            if (size <= 0)
                throw new ArgumentException("Parameter " + size.ToString() + " must be non-negative and non-zero");
            return size;
        }


        public int CheckNotNegativeInt(int size)
        {
            if (size <= 0)
                throw new ArgumentException("Parameter " + size.ToString() + " must be non-negative and non-zero");
            return size;
        }

        public void ParserMountPoint(dynamic parametrs)
        {
            if (parametrs.mountPoints == null)
                throw new Exception("Parameters of mountPoint not specified");
            foreach (var point in parametrs.mountPoints)
            {
                List<double> mountPointLocation = new List<double>();
                if (point.location == null)
                    throw new Exception("Location for mountPoint not specified");
                if (point.radius == null)
                    throw new Exception("Radius for mountPoint not specified");
                if (point.length == null)
                    throw new Exception("Lenght for mountPoint not specified");
                foreach (double loc in point.location)
                    mountPointLocation.Add((double)loc);
                if (mountPointLocation.Count != 3)
                    throw new Exception("Invalid input location for a restrictor. Specify a location relative to the three coordinate axes, for example:\\\"location\\\": [ 0.1589, 0.018, 0.092 ]");
                var vector = Vector.Create(mountPointLocation[0], mountPointLocation[1], mountPointLocation[2]);
                double radius = (double)point.radius;
                double length = (double)point.length;
                MountPoint newPoint = new MountPoint(CheckNotNegative(radius), CheckNotNegative(length), vector);
                if (point.count != null)
                {
                    object[] countArray = point.count.ToObject<object[]>();
                    if (countArray.Length == 2)
                    {
                        if ((int)point.count[0] == 2)
                        {
                            newPoint.setCountOfPoint((int)point.count[0]);
                            newPoint.setPointSimetricAxis(CheckAxis((string)point.count[1]));
                        }
                        else
                            throw new Exception("The count of point can be 1, 2 (built symmetrically to the specified axis), 4 (built symmetrically to the x, y axes");
                    }
                    else if (countArray.Length == 1)
                    {
                        if ((int)point.count[0] == 1 || (int)point.count[0] == 4)
                            newPoint.setCountOfPoint((int)point.count[0]);
                        else
                            throw new Exception("The count of point can be 1, 2 (built symmetrically to the specified axis), 4 (built symmetrically to the x, y axes");
                    }
                    else
                        throw new Exception("The count of points is specified in the array. For 2 points must be specifed the axis of rotation (for example [2, \"x\"]), for 1 or 4 only count (for example [1] or [4])");
                }
                if (point.rotation != null)
                {
                    if (point.rotation.ToObject<object[]>().Length != 2)
                        throw new Exception("Incorrectly set rotation for points. Set angle and axis in the format \"rotation: [90, \"x\"]\"");
                    newPoint.setRotation((double)point.rotation[0], CheckAxis((string)point.rotation[1]));
                }
                mountPoints.Add(newPoint);
            }
        }


        public int CheckAxis(string axis)
        {
            switch (axis.ToLower())
            {
                case "x":
                    return 1;
                case "y":
                    return 2;
                case "z":
                    return 3;
                default:
                    throw new Exception("Incorrect axis of symmetry for holes. Valid are x, y, z, but specified " + axis);
            }
        }

        public void CheckPointInBox(MountPoint point)
        {
            List<double> dependsAxisSize;
            List<double> dependAxisLoc;
            double radius = point.getRadius();
            double length = point.getLength();
            foreach (Vector loc in point.getSimPointLocation())
            {
                dependAxisLoc = new List<double>() { loc[0], loc[1], loc[2] };
                if (point.getAxis() == 1)
                    dependsAxisSize = new List<double>() { radius, length / 2, radius };
                else if (point.getAxis() == 2)
                    dependsAxisSize = new List<double>() { length / 2, radius, radius, };
                else
                    dependsAxisSize = new List<double>() { radius, radius, length / 2 };

                if (Math.Round(dependAxisLoc[0] + dependsAxisSize[0], 3) > mainBoxBounds[0])
                    throw new Exception($"Point(location {loc}) is not in the body volume(axis x)");
                if (Math.Round(dependAxisLoc[0] - dependsAxisSize[0], 3) < mainBoxBounds[1])
                    throw new Exception($"Point(location {loc}) is not in the body volume(axis x)");
                if (Math.Round(dependAxisLoc[1] + dependsAxisSize[1], 3) > mainBoxBounds[2])
                    throw new Exception($"Point(location {loc}) is not in the body volume(axis y)");
                if (Math.Round(dependAxisLoc[1] - dependsAxisSize[1], 4) < mainBoxBounds[3])
                    throw new Exception($"Point(location {loc}) is not in the body volume(axis y)");
                if (Math.Round(dependAxisLoc[2] + dependsAxisSize[2], 3) > mainBoxBounds[4])
                    throw new Exception($"Point(location {loc}) is not in the body volume(axis z)");
                if (Math.Round(dependAxisLoc[2] - dependsAxisSize[2], 3) < (mainBoxBounds[5]))
                    throw new Exception($"Point(location  {loc} ) is not in the body volume(axis z)");
            }
        }

        public void ParserSensor(dynamic parametrs)
        {
 
            if (parametrs.sensors == null)
                throw new Exception("Sensors not specified");
            foreach (var i in parametrs.sensors)
            {
                string name = "sensor";
                if (i.name != null)
                    name = (string)i.name;
                if (i.shape == null)
                    throw new Exception("The shape (box/cylinder) not specified for the cylinder " + name);
                if (i.location == null)
                    throw new Exception("Location not specified for the cylinder " + name);
                if (i.size == null)
                    throw new Exception("Size not specified for the cylinder " + name);

                string shape = (string)i.shape;
                List<double> size = new List<double>();
                List<double> loc = new List<double>();
                if (shape == "box")
                {
                    foreach (double n in i.size)
                        size.Add(CheckNotNegative(n));
                    if (size.Count != 3)
                        throw new Exception("Invalid size for sensor " + name + " . For a box shape specified the length, width and height. For example \"size\": [ 0.0889, 0.025, 0.031 ]");
                }
                else if (shape == "cylinder")
                {
                    foreach (double n in i.size)
                        size.Add(CheckNotNegative(n));
                    if (size.Count != 2)
                        throw new Exception("Invalid size for sensor " + name + " . For a cylinder shape specified the radius and length. For example \"size\": [ 0.004, 0.0352 ]");
                }
                else
                    throw new Exception("Invalid shape for sensor " + name + " . Specify the box/cylinder shape");
                foreach (double l in i.location)
                    loc.Add((double)l);
                if (loc.Count != 3)
                    throw new Exception("Invalid location for the cylinder " + name + ". Specify a location relative to the three coordinate axes, for example:\\\"location\\\": [ 0.1589, 0.018, 0.092 ]");
                Vector locVector = Vector.Create(loc[0], loc[1], loc[2]);
                double load = 0;
                if (i.load != null)
                    load = CheckNotNegative((double)i.load);

                Sensor sensor = new Sensor(name, load, size, locVector, shape);
                if (i.rotation != null)
                    sensor.setRotation((double)i.rotation[0], CheckAxis((string)i.rotation[1]));

                CheckSensorInBox(sensor);
                sensorList.Add(sensor);
            }
        }


        public void CheckSensorInBox(Sensor sensor)
        {
            try
            {
                List<double> dependsAxisSize;
                List<double> dependAxisLoc;
                dependAxisLoc = new List<double>() { sensor.getLocation()[0], sensor.getLocation()[1], sensor.getLocation()[2] };
                if (sensor.getShape() == "cylinder")
                {
                    if (sensor.getAxis() == 1)
                        dependsAxisSize = new List<double>() { sensor.getSize()[0], sensor.getSize()[1] / 2, sensor.getSize()[0] };

                    else if (sensor.getAxis() == 2)
                        dependsAxisSize = new List<double>() { sensor.getSize()[1] / 2, sensor.getSize()[0], sensor.getSize()[0] };
                    else
                        dependsAxisSize = new List<double>() { sensor.getSize()[0], sensor.getSize()[0], sensor.getSize()[1] / 2 };
                }
                else
                {
                    if (sensor.getAxis() == 1)
                        dependsAxisSize = new List<double>() { sensor.getSize()[0] / 2, sensor.getSize()[2] / 2, sensor.getSize()[1] / 2 };
                    else if (sensor.getAxis() == 2)
                        dependsAxisSize = new List<double>() { sensor.getSize()[2] / 2, sensor.getSize()[1] / 2, sensor.getSize()[0] / 2 };
                    else if (sensor.getAxis() == 3)
                        dependsAxisSize = new List<double>() { sensor.getSize()[1] / 2, sensor.getSize()[0] / 2, sensor.getSize()[2] / 2 };
                    else
                        dependsAxisSize = new List<double>() { sensor.getSize()[0] / 2, sensor.getSize()[1] / 2, sensor.getSize()[2] / 2 };
                }

                if (Math.Round(dependAxisLoc[0] + dependsAxisSize[0], 3) > mainBoxBounds[0])
                    throw new Exception($"Sensor " + sensor.getName() + " is not in the body volume(axis x)");
                if (Math.Round(dependAxisLoc[0] - dependsAxisSize[0], 3) < mainBoxBounds[1])
                    throw new Exception("Sensor " + sensor.getName() + " is not in the body volume(axis x)");
                if (Math.Round(dependAxisLoc[1] + dependsAxisSize[1], 3) > mainBoxBounds[2])
                    throw new Exception("Sensor " + sensor.getName() + " is not in the body volume(axis y)");
                if (Math.Round(dependAxisLoc[1] - dependsAxisSize[1], 3) < mainBoxBounds[3])
                    throw new Exception("Sensor " + sensor.getName() + " is not in the body volume(axis y)");
                if (Math.Round(dependAxisLoc[2] + dependsAxisSize[2], 3) > mainBoxBounds[4])
                    throw new Exception("Sensor " + sensor.getName() + " is not in the body volume(axis z)");
                if (Math.Round(dependAxisLoc[2] - dependsAxisSize[2], 3) < (mainBoxBounds[5]))
                    throw new Exception("Sensor " + sensor.getName() + " is not in the body volume(axis z)");
            }
            catch (Exception e)
            {
                Debug.Fail(e.Message);
            }
        }

        public void ParserRestrictors(dynamic parametrs)
        {
            if (parametrs.restrictors == null)
                throw new Exception("Restrictor not specified");
            int number = 0;
            foreach (var r in parametrs.restrictors)
            {
                number++;
                if (r.location == null)
                    throw new Exception("Location not specified for the restrictors number " + number);
                if (r.size == null)
                    throw new Exception("Size not specified for the restrictors " + number);
                List<double> loc = new List<double>();
                List<double> size = new List<double>();
                foreach (double l in r.location)
                    loc.Add((double)l);
                if (loc.Count != 3)
                    throw new Exception("Invalid location for the restrictor number " + number + ". Specify a location relative to the three coordinate axes, for example:\\\"location\\\": [ 0.1589, 0.018, 0.092 ]");
                foreach (double s in r.size)
                    size.Add(CheckNotNegative((double)s));
                if (size.Count != 3)
                    throw new Exception("Invalid size for the restrictor number " + number + ". Specify the length, width and height.");
                Restrictor restrictor = new Restrictor(size, Vector.Create(loc[0], loc[1], loc[2]));
                if (r.rotation != null)
                    restrictor.setRotation((double)r.rotation[0], CheckAxis((string)r.rotation[1]));
                restrictorsList.Add(restrictor);
            }
        }

        public void ParsMaterials(dynamic parametrs)
        {
            if (parametrs.materials != null)
                material = (string)parametrs.materials;
            var s = $"{material}";
            if (parametrs.number != null)
                number = CheckNotNegativeInt((int)parametrs.number);
        }



        public void GetParametrs()
        {
            try
            {
                //var buffer = Resources.parametrs;
                //string path = Directory.GetCurrentDirectory() + file;
                string fileText = System.IO.File.ReadAllText("D:\\addin\\Sample_Addin_2\\CreateBody\\parametrs.json");

                //string fileText = System.Text.Encoding.Default.GetString(buffer);
                var parametrs = JsonConvert.DeserializeObject<dynamic>(fileText);
                ParserMainBox(parametrs);
                ParserMountPoint(parametrs);
                ParserSensor(parametrs);
                ParserRestrictors(parametrs);
                ParsMaterials(parametrs);
            }
            catch (Exception e)
            {
                Debug.Fail(e.Message);
                throw new ArgumentException(e.Message);
            }
        }


        protected override void OnExecute(Command command, ExecutionContext context, Rectangle buttonRect)
        {
            Window window = Window.ActiveWindow;
            window.InteractionMode = InteractionMode.Solid;
            if (window.Document.MainPart.GetBodies() != null || !window.Document.MainPart.IsEmpty)
            {
                window.Document.MainPart.ClearAllPartData();
                window.Document.MainPart.ClearAllPartData();
                foreach (Support sup in Support.GetAll())
                    Delete.Execute(Selection.Create(sup));
                foreach (Force force in Force.GetAll())
                    force.Delete();
                foreach (Support supportin in Support.GetAll())
                {
                    supportin.Delete();
                    Delete.Execute(Selection.Create(supportin));
                }
                mainBoxSize.Clear();
                sensorList.Clear();
                mountPoints.Clear();
                suppurtFacesList.Clear();
                forceSensorsList.Clear();
                mainBoxBounds.Clear();
                restrictorsList.Clear();
            }
            Trace.Listeners.Add(new DefaultTraceListener());
            GetParametrs();
            CreateSimulation();
            DesignBody desBodyMaster = CreateMainBox(window);
            IList<Face> basicFace6 = desBodyMaster.Shape.Faces.ToList();
            IList<DesignFace> basicDesFace6 = desBodyMaster.Faces.ToList();
            CreateMountPoint(desBodyMaster, basicFace6, mountPoints, suppurtFacesList);
            CreateRestrictors(window, desBodyMaster);
            //CreateSensors(window, desBodyMaster, basicFace6, basicDesFace6);
            //AssignPhysicalQuantities(desBodyMaster, window);
            //StartTologyOpt(window);
        }

        private DesignBody CreateMainBox(Window window)
        {
            Body body = Body.ExtrudeProfile(new RectangleProfile(Plane.PlaneXY, mainBoxSize[0], mainBoxSize[1]), mainBoxSize[2] / 2);
            DesignBody desBodyMaster = DesignBody.Create(window.Document.MainPart, "body", body);
            var extrudeFaceOptions = new ExtrudeFaceOptions();
            extrudeFaceOptions.ExtrudeType = ExtrudeType.ForceIndependent;
            DesignFace face = desBodyMaster.Faces.ToList()[0];
            Direction dirZ = face.Shape.GetGeometry<Plane>().Frame.DirZ;
            ExtrudeFaces.Execute(Selection.Create(face), dirZ, mainBoxSize[2] / 2, extrudeFaceOptions);
            Matrix matrix = Matrix.CreateTranslation(Vector.Create(mainBoxLocation[0], mainBoxLocation[1], mainBoxLocation[2]));
            desBodyMaster.Transform(matrix);
            return desBodyMaster;
        }

        public void SetForce(DesignFace face, double load)
        {
            VectorQuantityForce force = VectorQuantityForce.Create(0, 0, -load, ForceUnit.Newton);
            var sim = Simulation.GetCurrentSimulation();
            Force.GetAll();
            Force.Create(sim, face, force);
            forceExist = true;
        }

        public void CreateSimulation()
        {
            Simulation.GetCurrentSimulation().Label = "TopOpt";

        }
        protected void AssignPhysicalQuantities(DesignBody desBodyMaster, Window window)
        {
            try
            {
                var matAs = MaterialAssignment.GetAll().First();
                Material material = Material.GetLibraryMaterial(this.material);
                if (material == null)
                    throw new Exception(this.material + " is not in Discovery materials library. Select one:\n Plastic, ABS (high-impact) \n Plastic, PC(copolymer, heat resistant \n Plastic, PP (homopolymer, low flow)");
                matAs.Material = material;

                if (suppurtFacesList.Count > 0)
                {
                    foreach (DesignFace face in suppurtFacesList)
                        if (!face.IsDeleted)
                            Support.Create(Simulation.GetCurrentSimulation(), face, SupportType.Fixed);
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
            }
        }


        public void StartTologyOpt(Window window)
        {
            for (; number > 0; number--)
            {
                int goal;
                int persent;
                Random rand = new Random();
                if (forceExist)
                {
                    switch (number)
                    {
                        case 1:
                            goal = 2;
                            persent = 60;
                            break;
                        case 2:
                            goal = 1;
                            persent = 60;
                            break;
                        case 3:
                            goal = 0;
                            persent = 60;
                            break;
                        default:
                            goal = rand.Next(0, 4);
                            if (goal == 3) goal = 0;
                            persent = rand.Next(60, 70);
                            break;
                    }
                }
                else
                {
                    goal = 2;
                    persent = rand.Next(65, 75);
                    break;
                }

                Simulation simulation1 = Simulation.GetCurrentSimulation();
                simulation1.TopologyOptimization.Enable = true;
                OptimizationObjective optObj = (OptimizationObjective)goal;
                RatioQuantity ratioQuantity = RatioQuantity.Create(persent, RatioUnit.Percent);
                simulation1.TopologyOptimization.OptimizationObjective = optObj;
                simulation1.TopologyOptimization.PercentReduction = ratioQuantity;
                Solver.StartExplore();

                Stopwatch sw = Stopwatch.StartNew();
                while (Solver.GetProgressState() != ProgressState.Completed)
                {
                    if (sw.ElapsedMilliseconds > 900000)
                    {
                        sw.Stop();
                        break;
                    }
                }

                simulation1.TopologyOptimization.AddOptimizedBody();
                DesignMesh designMesh = window.Document.MainPart.Meshes.First();
                designMesh.SetColor(null, Color.White);
                simulation1.TopologyOptimization.Enable = false;
            }
        }


        protected void CreateMountPoint(DesignBody desBody, IList<Face> basicFace6, List<MountPoint> mountPoints, List<DesignFace> suppurtFacesList)
        {
            try
            {
                var extrudeFaceOptions = new ExtrudeFaceOptions();
                extrudeFaceOptions.ExtrudeType = ExtrudeType.ForceIndependent;
                foreach (MountPoint point in mountPoints)
                {

                    var locX = point.getLocation()[0];
                    var locY = point.getLocation()[1];
                    var locZ = point.getLocation()[2];
                    ICollection<DesignFace> collectionBefor = desBody.Faces;
                    if (point.getPointCount() == 4)
                    {
                        List<Vector> locationPoint = new List<Vector>
                    {
                        Vector.Create(locX, locY, locZ),
                        Vector.Create(-locX, locY, locZ),
                        Vector.Create(locX, -locY, locZ),
                        Vector.Create(-locX, -locY, locZ)
                    };
                        point.setSimPointLocation(locationPoint);
                        CheckPointInBox(point);
                        foreach (Vector loc in locationPoint)
                        {
                            Body cylinderExp = Body.ExtrudeProfile(new CircleProfile(Plane.PlaneXY, point.getRadius()), point.getLength() / 2);
                            DesignBody desSenBody = DesignBody.Create(desBody.Parent, "Cylinder", cylinderExp);
                            DesignFace face = desSenBody.Faces.ToList()[0];
                            Direction dirZ = face.Shape.GetGeometry<Plane>().Frame.DirZ;
                            ExtrudeFaces.Execute(Selection.Create(face), dirZ, point.getLength() / 2, extrudeFaceOptions);

                            if (point.getAngle() != 0)
                                Rotate(cylinderExp, point.getAngle(), point.getAxis());
                            cylinderExp.Transform(Matrix.CreateTranslation(loc));
                            desBody.Shape.Subtract(cylinderExp);
                        }
                        IEnumerable<DesignFace> newCol = desBody.Faces.Except(collectionBefor);
                        suppurtFacesList.AddRange(newCol);

                    }
                    else if (point.getPointCount() == 2)
                    {
                        Body cylinder1 = Body.ExtrudeProfile(new CircleProfile(Plane.PlaneXY, point.getRadius(), PointUV.Origin), point.getLength() / 2);
                        Body cylinder2 = Body.ExtrudeProfile(new CircleProfile(Plane.PlaneXY, point.getRadius(), PointUV.Origin), point.getLength() / 2);
                        DesignBody hole1 = DesignBody.Create(desBody.Parent, "Cylinder1", cylinder1);
                        DesignBody hole2 = DesignBody.Create(desBody.Parent, "Cylinder2", cylinder2);

                        DesignFace face1 = hole1.Faces.ToList()[0];
                        DesignFace face2 = hole2.Faces.ToList()[0];
                        Direction dirZ = face1.Shape.GetGeometry<Plane>().Frame.DirZ;
                        ExtrudeFaces.Execute(Selection.Create(face1), dirZ, point.getLength() / 2, extrudeFaceOptions);
                        ExtrudeFaces.Execute(Selection.Create(face2), dirZ, point.getLength() / 2, extrudeFaceOptions);

                        Vector vector1;
                        Vector vector2;

                        if (point.getAngle() != 0)
                        {
                            cylinder1 = Rotate(cylinder1, point.getAngle(), point.getAxis());
                            cylinder2 = Rotate(cylinder2, point.getAngle(), point.getAxis());
                        }
                        if (point.getPointSimetricAxis() == 1)
                        {
                            vector1 = Vector.Create(locX, locY, locZ);
                            vector2 = Vector.Create(locX, locY * -1, locZ);
                            cylinder1.Transform(Matrix.CreateTranslation(vector1));
                            cylinder2.Transform(Matrix.CreateTranslation(vector2));
                        }
                        else if (point.getPointSimetricAxis() == 2)
                        {
                            vector1 = Vector.Create(locX, locY, locZ);
                            vector2 = Vector.Create(locX * -1, locY, locZ);
                            cylinder1.Transform(Matrix.CreateTranslation(vector1));
                            cylinder2.Transform(Matrix.CreateTranslation(vector2));
                        }
                        else if (point.getPointSimetricAxis() == 3)
                        {
                            vector1 = Vector.Create(locX, locY, locZ);
                            vector2 = Vector.Create(locX, locY, locZ * -1);
                            cylinder1.Transform(Matrix.CreateTranslation(vector1));
                            cylinder2.Transform(Matrix.CreateTranslation(vector2));
                        }
                        point.addSimPointLocation(vector1);
                        point.addSimPointLocation(vector2);
                        CheckPointInBox(point);

                        collectionBefor = desBody.Faces;
                        desBody.Shape.Subtract(cylinder1);
                        desBody.Shape.Subtract(cylinder2);
                        IEnumerable<DesignFace> newCol = desBody.Faces.Except(collectionBefor);
                        suppurtFacesList.AddRange(newCol);
                    }
                    else
                    {
                        Vector loc = Vector.Create(point.getLocation()[0], point.getLocation()[1], point.getLocation()[2]);
                        point.addSimPointLocation(loc);
                        CheckPointInBox(point);

                        collectionBefor = desBody.Faces;
                        Body cylinder1 = Body.ExtrudeProfile(new CircleProfile(Plane.PlaneXY, point.getRadius(), PointUV.Origin), point.getLength() / 2);
                        DesignBody desSenBody = DesignBody.Create(desBody.Parent, "Cylinder3", cylinder1);
                        DesignFace face = desSenBody.Faces.ToList()[0];
                        Direction dirZ = face.Shape.GetGeometry<Plane>().Frame.DirZ;
                        ExtrudeFaces.Execute(Selection.Create(face), dirZ, point.getLength() / 2, extrudeFaceOptions);
                        if (point.getAngle() != 0)
                            cylinder1 = Rotate(cylinder1, point.getAngle(), point.getAxis());
                        cylinder1.Transform(Matrix.CreateTranslation(loc));

                        desBody.Shape.Subtract(cylinder1);
                        IEnumerable<DesignFace> newCol = desBody.Faces.Except(collectionBefor);
                        suppurtFacesList.AddRange(newCol);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Fail(e.Message);
            }
        }

        public void CreateSensors(Window window, DesignBody desBody, IList<Face> basicFace6, IList<DesignFace> basicDesFace6)
        {
            var extrudeFaceOptions = new ExtrudeFaceOptions();
            extrudeFaceOptions.ExtrudeType = ExtrudeType.ForceIndependent;
            Profile profile;
            double lenght;
            Vector locVector;
            List<double> sensSize;
            foreach (Sensor sensor in sensorList)
            {
                try
                {
                    locVector = sensor.getLocation();
                    sensSize = sensor.getSize();
                    double reqArea;
                    string shape = sensor.getShape();
                    if (shape == "box")
                    {
                        profile = new RectangleProfile(Plane.PlaneXY, sensSize[0], sensSize[1]);
                        lenght = sensSize[2] / 2;
                    }
                    else
                    {
                        profile = new CircleProfile(Plane.PlaneXY, sensSize[0]);
                        lenght = sensSize[1] / 2;
                    }
                    reqArea = profile.Area;
                    Body body = Body.ExtrudeProfile(profile, lenght);
                    DesignBody desBodySensor = DesignBody.Create(window.Document.MainPart, $"{sensor.getName()}", body);
                    DesignFace face = desBodySensor.Faces.ToList()[0];
                    Direction dirZ = face.Shape.GetGeometry<Plane>().Frame.DirZ;
                    ExtrudeFaces.Execute(Selection.Create(face), dirZ, lenght, extrudeFaceOptions);
                    if (sensor.getAngle() != 0)
                        body = Rotate(body, sensor.getAngle(), sensor.getAxis());
                    desBodySensor.Transform(Matrix.CreateTranslation(locVector));

                    if (sensor.getLoad() != 0)
                    {
                        ICollection<DesignFace> collectionBefor = desBody.Faces;
                        desBody.Shape.Subtract(body);
                        IEnumerable<DesignFace> newCol = desBody.Faces.Except(collectionBefor);
                        if (newCol.Count() != 0)
                        {
                            var sensArea = (shape == "box") ? sensSize[0] * sensSize[1] : Math.PI * Math.Pow(sensSize[0], 2);
                            SetForce(newCol.First(), sensor.getLoad());
                            forceExist = true;
                        }
                        else
                        {
                            ICollection<DesignFace> collectionBefor1 = desBody.Faces;
                            Direction dirSensor = -Plane.PlaneXY.Frame.DirZ;
                            double sensArea = 0;
                            sensArea = (shape == "box") ? sensSize[0] * sensSize[1] : Math.PI * Math.Pow(sensSize[0], 2);
                            DesignFace topBoxFace = null;
                            foreach (DesignFace desFace in desBody.Faces)
                            {
                                if ((desFace.Shape.GetNormal(Point.Origin) == -dirSensor) && desFace.Area > sensArea)
                                {
                                    topBoxFace = desFace;
                                    break;
                                }
                            }
                            if (topBoxFace == null)
                                continue;
                            Body planarBody;
                            Plane reqPlane = topBoxFace.Shape.GetGeometry<Plane>();
                            if (shape == "box")
                                planarBody = Body.CreatePlanarBody(reqPlane, (new RectangleProfile(reqPlane, sensSize[0], sensSize[1])).Boundary);
                            else
                                planarBody = Body.CreatePlanarBody(reqPlane, (new CircleProfile(reqPlane, sensSize[0])).Boundary);
                            DesignBody sen = DesignBody.Create(window.Document.MainPart, $"planarR", planarBody);
                            if (sensor.getAxis() == 3)
                            {
                                planarBody = Rotate(planarBody, sensor.getAngle(), sensor.getAxis());
                            }
                            CreateSensorTranslation(topBoxFace, sen, locVector);
                            Selection curvSelection = Selection.Create(sen);
                            ProjectToSolid.Execute(curvSelection, Selection.Empty(), Selection.Empty()).GetCreated<IDesignFace>();
                            Delete.Execute(curvSelection);
                            newCol = desBody.Faces.Except(collectionBefor1);
                            double maxReqArea = -1;
                            DesignFace biggerNewFace = newCol.Last();
                            foreach (DesignFace f in newCol)
                                if (f.Area > maxReqArea && f.Area <= reqArea)
                                {
                                    maxReqArea = f.Area;
                                    biggerNewFace = f;
                                }
                            SetForce(biggerNewFace, sensor.getLoad());
                            forceExist = true;
                        }
                    }
                    else
                    {
                        desBody.Shape.Subtract(body);
                    }
                }
                catch (Exception e)
                {
                    Debug.Fail(e.Message);
                }
            }
        }

        public void CreateSensorTranslation(DesignFace topBoxFace, DesignBody sen, Vector loc)
        {
            loc = Vector.Create(loc[0] - mainBoxLocation[0], loc[1] - mainBoxLocation[1], 0);
            sen.Transform(Matrix.CreateTranslation(loc));
        }


        public void CreateRestrictors(Window window, DesignBody desBody)
        {
            var extrudeFaceOptions = new ExtrudeFaceOptions();
            extrudeFaceOptions.ExtrudeType = ExtrudeType.ForceIndependent;
            foreach (Restrictor restrictor in restrictorsList)
            {
                try
                {
                    Profile profil = new RectangleProfile(Plane.PlaneXY, restrictor.getSize()[0], restrictor.getSize()[1]);
                    Body body = Body.ExtrudeProfile(profil, restrictor.getSize()[2] / 2);
                    DesignBody restDesignBody = DesignBody.Create(window.Document.MainPart, "restrictor", body);
                    DesignFace face = restDesignBody.Faces.ToList()[0];
                    Direction dirZ = face.Shape.GetGeometry<Plane>().Frame.DirZ;
                    ExtrudeFaces.Execute(Selection.Create(face), dirZ, restrictor.getSize()[2] / 2, extrudeFaceOptions);
                    if (restrictor.getAngle() != 0)
                    {
                        body = Rotate(body, restrictor.getAngle(), restrictor.getAxis());
                    }
                    body.Transform(Matrix.CreateTranslation(Vector.Create((double)restrictor.getLocation()[0], (double)restrictor.getLocation()[1], (double)restrictor.getLocation()[2])));
                    desBody.Shape.Subtract(body);
                }
                catch (Exception e)
                {
                    Debug.Fail(e.Message);
                    Notifications.Create(NotificationSeverity.Error, e.Message);
                }
            }

            double length = mainBoxSize[2];
            foreach (MountPoint point in mountPoints)
            {
                foreach (Vector locPoint in point.getSimPointLocation())
                {
                    var locationR = RestrictorLocation(point, locPoint, length, length);
                    double radius = point.getRadius() + point.getRadius() * 3.5;
                    var lenghtAbove = CheckRestrictorTouchPoint(locationR[0], length, radius, point.getAxis(), locPoint, point.getLength());
                    var lenghtUnder = CheckRestrictorTouchPoint(locationR[1], length, radius, point.getAxis(), locPoint, point.getLength());

                    if (lenghtAbove != length || lenghtUnder != length)
                    {
                        locationR = RestrictorLocation(point, locPoint, lenghtAbove, lenghtUnder);
                    }

                    MountPoint res = new MountPoint(radius, lenghtAbove, locationR[0]);
                    MountPoint res1 = new MountPoint(radius, lenghtUnder, locationR[1]);

                    if (point.getAxis() != 0)
                    {
                        res.setRotation(point.getAngle(), point.getAxis());
                        res1.setRotation(point.getAngle(), point.getAxis());
                    }
                    CreateCyliderRestrictor(res, desBody, extrudeFaceOptions);
                    CreateCyliderRestrictor(res1, desBody, extrudeFaceOptions);
                }
            }
        }

        public void CreateCyliderRestrictor(MountPoint cylRestrictor, DesignBody desBody, ExtrudeFaceOptions extrudeFaceOptions)
        {
            Body restrictorBody = Body.ExtrudeProfile(new CircleProfile(Plane.PlaneXY, cylRestrictor.getRadius(), PointUV.Origin), cylRestrictor.getLength() / 2);
            DesignBody desSenBody = DesignBody.Create(desBody.Parent, "restrictor", restrictorBody);
            DesignFace face = desSenBody.Faces.ToList()[0];
            Direction dirZ = face.Shape.GetGeometry<Plane>().Frame.DirZ;
            ExtrudeFaces.Execute(Selection.Create(face), dirZ, cylRestrictor.getLength() / 2, extrudeFaceOptions);
            if (cylRestrictor.getAngle() != 0)
                restrictorBody = Rotate(restrictorBody, cylRestrictor.getAngle(), cylRestrictor.getAxis());
            restrictorBody.Transform(Matrix.CreateTranslation(cylRestrictor.getLocation()));

            desBody.Shape.Subtract(restrictorBody);
        }

        public List<Vector> RestrictorLocation(MountPoint point, Vector locPoint, double lengthA, double lenghtU)
        {
            List<Vector> locationRes = new List<Vector>();
            if (point.getAxis() == 1)
            {
                locationRes.Add(Vector.Create(locPoint.X, (locPoint.Y + point.getLength() / 2 + lengthA / 2), locPoint.Z));
                locationRes.Add(Vector.Create(locPoint.X, (locPoint.Y - point.getLength() / 2 - lenghtU / 2), locPoint.Z));
            }
            else if (point.getAxis() == 2)
            {
                locationRes.Add(Vector.Create((locPoint.X + point.getLength() / 2 + lengthA / 2), locPoint.Y, locPoint.Z));
                locationRes.Add(Vector.Create((locPoint.X - point.getLength() / 2 - lenghtU / 2), locPoint.Y, locPoint.Z));
            }
            else
            {
                locationRes.Add(Vector.Create(locPoint.X, locPoint.Y, locPoint.Z + point.getLength() / 2 + lengthA / 2));
                locationRes.Add(Vector.Create(locPoint.X, locPoint.Y, (locPoint.Z - point.getLength() / 2 - lenghtU / 2)));
            }
            return locationRes;
        }

        public double CheckRestrictorTouchPoint(Vector locationR, double lengthR, double radiusR, int axis, Vector pointLoc, double pointLength)
        {
            bool diffX;
            bool diffY;
            bool diffZ;
            int localPointCentre;
            double X;
            double Y;
            double Z;
            double pX;
            double pY;
            double pZ;
            List<int> dependsPointAxis;

            double distance;
            double mainPointChentre;
            if (axis == 1)
            {
                X = locationR.X;
                Y = locationR.Z;
                Z = locationR.Y;
                mainPointChentre = pointLoc.Y;
                localPointCentre = 1;
                dependsPointAxis = new List<int> { 0, 2, 1 };

            }
            else if (axis == 2)
            {
                X = locationR.Z;
                Y = locationR.Y;
                Z = locationR.X;
                mainPointChentre = pointLoc.X;
                localPointCentre = 0;
                dependsPointAxis = new List<int> { 2, 1, 0 };
            }
            else
            {
                X = locationR.X;
                Y = locationR.Y;
                Z = locationR.Z;
                mainPointChentre = pointLoc.Z;
                localPointCentre = 2;
                dependsPointAxis = new List<int> { 0, 1, 2 };
            }

            List<double> difference = new List<double>();
            foreach (MountPoint point in mountPoints)
            {
                foreach (Vector locPoint in point.getSimPointLocation())
                {

                    var lengthP = point.getLength();
                    if (locPoint.Equals(pointLoc))
                        continue;
                    pX = locPoint[dependsPointAxis[0]];
                    pY = locPoint[dependsPointAxis[1]];
                    pZ = locPoint[dependsPointAxis[2]];
                    diffX = GetRestr(pX, X, lengthP, radiusR);
                    diffY = GetRestr(pY, Y, lengthP, radiusR);
                    diffZ = GetRestr(pZ, Z, lengthP, lengthR / 2);
                    if (diffX && diffY && diffZ)
                    {
                        distance = Math.Abs(mainPointChentre - locPoint[localPointCentre]) - pointLength - lengthP * 1.5;
                        if (distance < 0) distance = 0.1;
                        difference.Add(distance);
                    }
                }
            }
            if (difference.Count != 0)
                return difference.Min();
            return lengthR;
        }

        public bool GetRestr(double locP, double locR, double lengthP, double radiusR)
        {
            double diff;
            diff = Math.Abs(locP - locR) - lengthP / 2; 
            if (diff < radiusR)
            {
                return true;
            }
            return false;
        }

        protected Body Rotate(Body desBody, Double angle, int axic)
        {
            Point origin = Plane.PlaneXY.Frame.Origin;
            Direction dir;
            angle = angle * Math.PI / 180; 
            switch (axic)
            {
                case 1:
                    dir = Plane.PlaneXY.Frame.DirX;
                    break;
                case 2:
                    dir = Plane.PlaneXY.Frame.DirY;
                    break;
                case 3:
                    dir = Plane.PlaneXY.Frame.DirZ;
                    break;
            }
            desBody.Transform(Matrix.CreateRotation(Line.Create(origin, dir), angle));
            return desBody;
        }
    }
}


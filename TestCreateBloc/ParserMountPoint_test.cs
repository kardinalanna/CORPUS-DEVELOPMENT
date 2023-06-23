using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CreateBody;
using Newtonsoft.Json;
using SpaceClaim;
using SpaceClaim.Api.V23.Geometry;

namespace TestCreateBloc
{
    [TestClass]
    public class ParserMountPoint_test
    {
        HelpMethods helpMethods = new HelpMethods();
        CreateBlockCapsule createBlockCapsule = new CreateBlockCapsule();

        [DataTestMethod]
        [DataRow(0.0022, new double[] { -0.1675, 0.036, 0.002735 }, 0.0051)]
        [DataRow(0.02, new double[] { -10, -6, 1 }, 0.0031)]
        [DataRow(0.002, new double[] { -10, -6, 7 }, 0.051)]
        public void test_ParserMountPoint_Initialize(double Radius, double[] Location, double Length)
        {

            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 0);
            var mountPoint = new
            {
                mountPoints = new[] {
                    new {
                        radius = Radius,
                        location = Location,
                        length = Length,
                    }
                }
            };
            var data = JsonConvert.SerializeObject(mountPoint, Formatting.Indented);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserMountPoint(parametrs);
            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 1);
            foreach (MountPoint point in createBlockCapsule.mountPoints)
            {
                Assert.AreEqual(point.getRadius(), Radius);
                Assert.AreEqual(point.getLength(), Length);
                Assert.IsTrue(helpMethods.equalsVector(point.getLocation(), Vector.Create(Location[0], Location[1], Location[2])));
                Assert.AreEqual(point.getPointCount(), 1);
            }

        }


        [TestMethod]
        public void test_ParserMountPoint_mountPoint_not_specified()
        {
            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 0);
            var mountPoint = new
            {
            };
            var data = JsonConvert.SerializeObject(mountPoint, Formatting.Indented);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserMountPoint(parametrs);
            }
            catch (Exception ioex)
            {
                Assert.AreEqual(ioex.Message, "Parameters of mountPoint not specified");
                return;
            }
            Assert.Fail("Expected Exception was not thrown");
        }


        [TestMethod]
        public void test_ParserMountPoint_Location_dont_specified()
        {
            var mountPoint = new
            {
                mountPoints = new[] {
                    new {
                        radius = 0.003,
                        length = 0.03,
                    }
                }
            };
            var data = JsonConvert.SerializeObject(mountPoint);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserMountPoint(parametrs);
            }
            catch (Exception ioex)
            {
                Assert.IsTrue(ioex is Exception);
                return;
            }
            Assert.Fail("Expected was not thrown");
        }


        [DataTestMethod]
        [DataRow(0.0022, new double[] { -0.1675, 0.036 }, 0.0051)]
        [DataRow(0.02, new double[] { -10 }, 0.0031)]
        [DataRow(0.002, new double[] { }, 0.051)]
        [DataRow(0.2, new double[] { -10, -6, 7, 8, 0.9 }, 0.05)]
        public void test_ParserMountPoint_Location_is_not_3(double Radius, double[] Location, double Length)
        {
            var mountPoint = new
            {
                mountPoints = new[] {
                    new {
                        radius = Radius,
                        location = Location,
                        length = Length,
                    }
                }
            };
            var data = JsonConvert.SerializeObject(mountPoint);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserMountPoint(parametrs);
            }
            catch (Exception ioex)
            {
                Assert.AreEqual(ioex.Message, "Invalid input location for a restrictor. Specify a location relative to the three coordinate axes, for example:\\\"location\\\": [ 0.1589, 0.018, 0.092 ]");
                return;
            }
            Assert.Fail("Expected was not thrown");
        }

        [TestMethod]
        public void test_ParserMountPoint_Radius_dont_specified()
        {
            var mountPoint = new
            {
                mountPoints = new[] {
                    new {
                        location = new double[] { -10, -6, 7 },
                        length = 0.03,
                    }
                }
            };
            var data = JsonConvert.SerializeObject(mountPoint);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserMountPoint(parametrs);
            }
            catch (Exception ioex)
            {
                Assert.IsTrue(ioex is Exception);
                return;
            }
            Assert.Fail("Expected was not thrown");
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(4)]
        public void test_ParserMountPoint_setPointCount_count_1_4(int c)
        {
            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 0);
            var mountPoint = new
            {
                mountPoints = new[] {
                    new {
                        radius = 0.002,
                        location = new double[] { -10, -6, 7 },
                        length = 0.003,
                        count = new double[]{c}
                    }
                }
            };
            var data = JsonConvert.SerializeObject(mountPoint);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserMountPoint(parametrs);
            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 1);
            foreach (MountPoint point in createBlockCapsule.mountPoints)
            {
                Assert.AreEqual(point.getRadius(), 0.002);
                Assert.AreEqual(point.getLength(), 0.003);
                Assert.IsTrue(helpMethods.equalsVector(point.getLocation(), Vector.Create(-10, -6, 7)));
                Assert.AreEqual(point.getPointCount(), c);
            }
        }


        [DataTestMethod]
        [DataRow(2, "x", 1)]
        [DataRow(2, "y", 2)]
        [DataRow(2, "z", 3)]
        [DataRow(2, "X", 1)]
        [DataRow(2, "Y", 2)]
        [DataRow(2, "Z", 3)]
        public void test_ParserMountPoint_setPointCount_count_2(int c, string axis, int resAxis)
        {
            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 0);
            var mountPoint = new
            {
                mountPoints = new[] {
                    new {
                        radius = 0.002,
                        location = new double[] { -10, -6, 7 },
                        length = 0.003,
                        count = new object[]{c, axis}
                    }
                }
            };
            var data = JsonConvert.SerializeObject(mountPoint);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserMountPoint(parametrs);
            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 1);
            foreach (MountPoint point in createBlockCapsule.mountPoints)
            {
                Assert.AreEqual(point.getRadius(), 0.002);
                Assert.AreEqual(point.getLength(), 0.003);
                Assert.IsTrue(helpMethods.equalsVector(point.getLocation(), Vector.Create(-10, -6, 7)));
                Assert.AreEqual(point.getPointCount(), c);
                Assert.AreEqual(point.getPointSimetricAxis(), resAxis);
            }
        }

        [DataTestMethod]
        [DataRow(new object[] {4, 15, 33, "h"}, 1)]
        [DataRow(new object[] {2, "x", "s"}, 2)]
        [DataRow(new object[] {}, 3)]
        public void test_ParserMountPoint_setPointCount_CountLength_not_2_or_1(object[] countIn, int notUsed)
        {
            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 0);
            var mountPoint = new
            {
                mountPoints = new[] {
                    new {
                        radius = 0.002,
                        location = new double[] { -10, -6, 7 },
                        length = 0.003,
                        count = countIn
                    }
                }
            };
            var data = JsonConvert.SerializeObject(mountPoint);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserMountPoint(parametrs);
            }
            catch (Exception ioex)
            {
                Assert.AreEqual(ioex.Message, "The count of points is specified in the array. For 2 points must be specifed the axis of rotation (for example [2, \"x\"]), for 1 or 4 only count (for example [1] or [4])");
                return;
            }
            Assert.Fail("Expected was not thrown");
        }


        [DataTestMethod]
        [DataRow(new object[] { 3 }, 1)]
        [DataRow(new object[] { -4 }, 2)]
        [DataRow(new object[] { 0 }, 3)]
        [DataRow(new object[] { 1000 }, 4)]
        [DataRow(new object[] { 100, "x" }, 5)]
        [DataRow(new object[] { 3, "x" },6)]
        [DataRow(new object[] { 1, "y" },7)]
        [DataRow(new object[] { 4, "y" },8)]
        public void test_ParserMountPoint_setPointCount_count_not_2_4_1(object[] c, int dontUse)
        {
            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 0);
            var mountPoint = new
            {
                mountPoints = new[] {
                    new {
                        radius = 0.002,
                        location = new double[] { -10, -6, 7 },
                        length = 0.003,
                        count = c
                    }
                }
            };
            var data = JsonConvert.SerializeObject(mountPoint);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserMountPoint(parametrs);
            }
            catch (Exception ioex)
            {
                Assert.AreEqual(ioex.Message, "The count of point can be 1, 2 (built symmetrically to the specified axis), 4 (built symmetrically to the x, y axes");
                return;
            }
            Assert.Fail("Expected was not thrown");
        }


        [DataTestMethod]
        [DataRow(30.0, "x", 1)]
        [DataRow(-40.0, "y", 2)]
        [DataRow(90.0, "y", 2)]
        [DataRow(180.0, "z", 3)]
        [DataRow(150.0, "z", 3)]
        public void test_ParserMountPoint_setRotation(double angle, string axis, int resAxis)
        {
            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 0);
            object[] arr = new object[] { angle, axis };
            var mountPoint = new
            {
                mountPoints = new[] {
                    new {
                        radius = 0.002,
                        location = new double[] { -10, -6, 7 },
                        length = 0.003,
                        count =  new double[] { 1 },
                        rotation = arr
                    }
                }
            };
            var data = JsonConvert.SerializeObject(mountPoint);
            var parametrs = JsonConvert.DeserializeObject(data);

            createBlockCapsule.ParserMountPoint(parametrs);
            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 1);
            foreach (MountPoint point in createBlockCapsule.mountPoints)
            {
                Assert.AreEqual(point.getAngle(), angle);
                Assert.AreEqual(point.getAxis(), resAxis);
            }
        }


        [DataTestMethod]
        [DataRow(5, 10,new double[] {0,0,0}, 1)]
        [DataRow(1, 10, new double[] { 4, 4, 0 }, 2)]
        [DataRow(1, 10, new double[] { -4, -4, 0 }, 3)]
        [DataRow(2, 10, new double[] { 3, 0, 0 },4)]
        [DataRow(2, 10, new double[] { 0, 3, 0 }, 5)]
        [DataRow(5, 5, new double[] { 0, 0, 2.5 }, 6)]
        public void test_ParserMountPoint_CheckPointInBox(double radius, double length, double[] loc, int indetef)
        {
            var myData = new
            {
                mainBoxSize = new double[] { 10, 10, 10 },
                mainBoxLocation = new double[] { 0,0,0 },
            };
            var data = JsonConvert.SerializeObject(myData);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserMainBox(parametrs);

            Vector vector = Vector.Create(loc[0], loc[1], loc[2]);
            MountPoint point = new MountPoint(radius, length, vector);
            point.setSimPointLocation(new List<Vector>() { vector });
            createBlockCapsule.CheckPointInBox(point); 
        }

        [DataTestMethod]
        [DataRow(5, 10, new double[] { 0, 0, 0 }, 2, 90)]
        [DataRow(5, 10, new double[] { 0, 0, 0 }, 3, 90)]
        [DataRow(5, 10, new double[] { 0, 0, 0 }, 1, 90)]
        [DataRow(1, 10, new double[] { 4, 4, 0 }, 3, 90)]
        [DataRow(2, 2, new double[] { 3, 0, 0 }, 2, 90)]
        [DataRow(2, 2, new double[] { 0, 3, 0 }, 1, 90)]
        [DataRow(5, 5, new double[] { 0, 0, 2.5 }, 3, 90)]
        public void test_ParserMountPoint_CheckPointInBox_Rotation(double radius, double length, double[] loc, int axis, double angle)
        {
            var myData = new
            {
                mainBoxSize = new double[] { 10, 10, 10 },
                mainBoxLocation = new double[] { 0, 0, 0 },
            };
            var data = JsonConvert.SerializeObject(myData);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserMainBox(parametrs);

            Vector vector = Vector.Create(loc[0], loc[1], loc[2]);
            MountPoint point = new MountPoint(radius, length, vector);
            point.setSimPointLocation(new List<Vector>() { vector });
            point.setRotation(angle, axis);
            createBlockCapsule.CheckPointInBox(point);
        }

        [DataTestMethod]
        [DataRow(5, 10, new double[] { 0, 0,0.001 }, 0, 0, 1)]
        [DataRow(5, 10, new double[] { 0.001, 0, 0 }, 0, 0, 2)]
        [DataRow(5, 10, new double[] { 0, 0.001, 0 }, 0, 0, 3)]
        [DataRow(1, 10, new double[] { 4, 4, 0 }, 2, 90, 4)]
        [DataRow(1, 10, new double[] { 4, 4, 0 }, 1, 90, 5)]
        [DataRow(0.9, 5, new double[] { -4, -4, 0 }, 2, 90, 6)]
        [DataRow(2, 2, new double[] { 3.001, 0, 0 }, 1, 90, 7)]
        [DataRow(2, 3, new double[] { 0, 4, 0 }, 1, 90, 8)]
        [DataRow(5, 6, new double[] { 0, 1, 0 }, 2, 90, 9)]
        public void test_ParserMountPoint_CheckPointInBox_Exception(double radius, double length, double[] loc, int axis, double angle, int key)
        {
            var myData = new
            {
                mainBoxSize = new double[] { 10, 10, 10 },
                mainBoxLocation = new double[] { 0, 0, 0 },
            };
            var data = JsonConvert.SerializeObject(myData);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserMainBox(parametrs);

            Vector vector = Vector.Create(loc[0], loc[1], loc[2]);
            MountPoint point = new MountPoint(radius, length, vector);
            point.setSimPointLocation(new List<Vector>() { vector });
            point.setRotation(angle, axis);
            try
            {
                createBlockCapsule.CheckPointInBox(point);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains($"Point(location {vector}) is not in the body volume"));
                return;
            }
            Assert.Fail("Expected was not thrown");
        }

        [DataTestMethod]
        [DataRow( "radius", new object[] { -10, -6, 7 }, 0.44, new object[] { 1 }, new object[] { 0.90, "x" })]
        [DataRow( 0.003, new object[] { -10, "u", 7 }, 0.44, new object[] { 1 }, new object[] { 0.90, "x" })]
        [DataRow(0.04, new object[] { new double[] {2}, -6, 7 }, 0.44, new object[] { 1 }, new object[] { 0.90, "x" })]
        [DataRow(0.007, new object[] { -10, -6, 7 }, "jj", new object[] { 1 }, new object[] { 0.90, "x" })]
        [DataRow(0.1, new object[] { -10, -6, 7 }, 14, new object[] { "ll" }, new object[] { 0.90, "x" })]
        [DataRow(0.89, new object[] { -10, -6, 7 }, 14, new object[] { 4 }, new object[] { "dcc", "x" })]
        [DataRow(0.02, new object[] { -10, -6, 7 }, 14, new object[] { 2,  2 }, new object[] { 90, 7 })]
        [DataRow(0.045, new object[] { -10, -6, 7 }, 14, new object[] { -2}, new object[] { 90, 7 })]
        public void test_ParserMountPoint_ArgumentExeption(object R, object[] loc, object Length, object[] Count, object Rot)
        {
            Assert.AreEqual(createBlockCapsule.mountPoints.Count, 0);
            var mountPoint = new
            {
                mountPoints = new[] {
                    new {
                        radius = R,
                        location = loc,
                        length = Length,
                        count = Count,
                        rotation = Rot
                    }
                }
            };
            var data = JsonConvert.SerializeObject(mountPoint);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserMountPoint(parametrs);
            }
            catch (ArgumentException ioex)
            {
                Assert.IsTrue(ioex is ArgumentException);
                return;
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is Exception);
                return;
            }
            Assert.Fail("Expected was not thrown");
        }

    }
        
}

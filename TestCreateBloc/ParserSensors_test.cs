using CreateBody;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceClaim.Api.V23.Geometry;
using System.Xml.Linq;
using SpaceClaim;

namespace TestCreateBloc
{
    [TestClass]
    public class ParserSensors_test
    {
        HelpMethods helpMethods = new HelpMethods();
        CreateBlockCapsule createBlockCapsule = new CreateBlockCapsule();
        double[] Size = new double[] { 1, 1, 1 };
        double[] boxLocation = new double[] { 0, 0, 0 };

        public void start(double[] size, double[] loc)
        {
            var myData = new
            {
                mainBoxSize = size,
                mainBoxLocation = loc,
            };
            var data = JsonConvert.SerializeObject(myData);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserMainBox(parametrs);

        }

        [DataTestMethod]
        [DataRow("sens2", new double[] { 0.0375, 0.035, 0 }, new double[] { 0.0, 0, 0.089 }, "box")]
        [DataRow(233, new double[] { 0.0375, 0.035, 0.003 }, new double[] { 0.030, -0.06, 0.089 }, "box")]
        [DataRow("sens4", new double[] { 0.0375, 0, 0.45 }, new double[] { 0.0, 0, 0.08 }, "box")]
        [DataRow(0, new double[] { 0, 0.05, 0.0099 }, new double[] { -0.030, 0.06, 0.089 }, "box")]
        public void test_ParserSensors_Initialize_box(object Name, double[] Size, double[] Loc, string Shape)
        {
            start(this.Size, boxLocation);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        name = Name,
                        size = Size,
                        location = Loc,
                        shape = Shape
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserSensor(parametrs);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 1);
            foreach (Sensor s in createBlockCapsule.sensorList)
            {
                Assert.AreEqual(s.getName(), Name.ToString());
                Assert.IsTrue(helpMethods.equalsListDouble(s.getSize(), Size.ToList()));
                Assert.IsTrue(helpMethods.equalsVector(s.getLocation(), Vector.Create(Loc[0], Loc[1], Loc[2])));
                Assert.AreEqual(s.getShape(), Shape);
            }
        }

        [DataTestMethod]
        [DataRow("sens2", new double[] { 0.0375, 0 }, new double[] { 0.0, -0.003, 0.089 }, "cylinder")]
        [DataRow(2, new double[] { 0, 0.030 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        [DataRow("sens4", new double[] { 0.0375, 0.035 }, new double[] { 0.0, 0, 0.08 }, "cylinder")]
        [DataRow(3, new double[] { 0.03, 0.05 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        public void test_ParserSensors_Initialize_cylinder(object Name, double[] Size, double[] Loc, string Shape)
        {
            start(this.Size, boxLocation);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        name = Name,
                        size = Size,
                        location = Loc,
                        shape = Shape
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserSensor(parametrs);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 1);
            foreach (Sensor s in createBlockCapsule.sensorList)
            {
                Assert.AreEqual(s.getName(), Name.ToString());
                Assert.IsTrue(helpMethods.equalsListDouble(s.getSize(), Size.ToList()));
                Assert.IsTrue(helpMethods.equalsVector(s.getLocation(), Vector.Create(Loc[0], Loc[1], Loc[2])));
                Assert.AreEqual(s.getShape(), Shape);
            }
        }


        [DataTestMethod]
        [DataRow("sens2", new double[] { 0.0375, 0 }, new double[] { 0.0, -0.003, 0.089 }, "cylinder", 100)]
        [DataRow("sens3", new double[] { 0, 0.030 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder", 0)]
        [DataRow("sens4", new double[] { 0.0375, 0.035 }, new double[] { 0.0, 0, 0.08 }, "cylinder", 0.10)]
        [DataRow("sens5", new double[] { 0.03, 0.05 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder", 20)]
        [DataRow("sens4", new double[] { 0.0375, 0, 0.45 }, new double[] { 0.0, 0, 0.08 }, "box", 10)]
        [DataRow("sens5", new double[] { 0, 0.05, 0.0099 }, new double[] { -0.030, 0.06, 0.089 }, "box", 20)]
        [DataRow("sens5", new double[] { 0, 0.05, 0.0099 }, new double[] { -0.030, 0.06, 0.089 }, "box", 0.0040)]
        public void test_ParserSensors_SetLoad(string Name, double[] Size, double[] Loc, string Shape, double Load)
        {
            start(this.Size, boxLocation);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        name = Name,
                        size = Size,
                        location = Loc,
                        shape = Shape,
                        load = Load
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserSensor(parametrs);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 1);
            foreach (Sensor s in createBlockCapsule.sensorList)
            {
                Assert.AreEqual(s.getName(), Name);
                Assert.IsTrue(helpMethods.equalsListDouble(s.getSize(), Size.ToList()));
                Assert.IsTrue(helpMethods.equalsVector(s.getLocation(), Vector.Create(Loc[0], Loc[1], Loc[2])));
                Assert.AreEqual(s.getShape(), Shape);
                Assert.AreEqual(s.getLoad(), Load);
            }
        }

        [DataTestMethod]
        [DataRow("sens2", new double[] { 0.0375, 0 }, new double[] { 0.0, -0.003, 0.089 }, "cylinder", -100)]
        [DataRow("sens3", new double[] { 0, 0.030 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder", -0.15)]
        [DataRow("sens4", new double[] { 0.0375, 0.035 }, new double[] { 0.0, 0, 0.08 }, "cylinder", -0.10)]
        [DataRow("sens5", new double[] { 0.03, 0.05 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder", -20)]
        [DataRow("sens4", new double[] { 0.0375, 0, 0.45 }, new double[] { 0.0, 0, 0.08 }, "box", -10)]
        [DataRow("sens5", new double[] { 0, 0.05, 0.0099 }, new double[] { -0.030, 0.06, 0.089 }, "box", -20)]
        [DataRow("sens5", new double[] { 0, 0.05, 0.0099 }, new double[] { -0.030, 0.06, 0.089 }, "box", -0.0040)]
        public void test_ParserSensors_SetLoad_ExceptionLoad(string Name, double[] Size, double[] Loc, string Shape, double Load)
        {
            start(this.Size, boxLocation);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        name = Name,
                        size = Size,
                        location = Loc,
                        shape = Shape,
                        load = Load
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserSensor(parametrs);
            }
            catch (ArgumentException ioex)
            {
                Assert.AreEqual(ioex.Message, "Parameter " + Load + " be non-negative and non-zero");
                return;
            }
            Assert.Fail("Expected Exception was not thrown");
        }


        [DataTestMethod]
        [DataRow("sens2", new double[] { 0.0375, 0 }, new double[] { 0.0, -0.003, 0.089 }, "cylinder")]
        [DataRow("sens3", new double[] { 0, 0.030 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        [DataRow("sens4", new double[] { 0.0375, 0.035 }, new double[] { 0.0, 0, 0.08 }, "cylinder")]
        [DataRow("sens5", new double[] { 0.03, 0.05 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        [DataRow("sens4", new double[] { 0.0375, 0, 0.45 }, new double[] { 0.0, 0, 0.08 }, "box")]
        [DataRow("sens5", new double[] { 0, 0.05, 0.0099 }, new double[] { -0.030, 0.06, 0.089 }, "box")]
        [DataRow("sens5", new double[] { 0, 0.05, 0.0099 }, new double[] { -0.030, 0.06, 0.089 }, "box")]
        public void test_ParserSensors_WithoutName(string Name, double[] Size, double[] Loc, string Shape)
        {
            start(this.Size, boxLocation);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        size = Size,
                        location = Loc,
                        shape = Shape,
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserSensor(parametrs);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 1);
            foreach (Sensor s in createBlockCapsule.sensorList)
            {
                Assert.AreEqual(s.getName(), "sensor");
                Assert.IsTrue(helpMethods.equalsListDouble(s.getSize(), Size.ToList()));
                Assert.IsTrue(helpMethods.equalsVector(s.getLocation(), Vector.Create(Loc[0], Loc[1], Loc[2])));
                Assert.AreEqual(s.getShape(), Shape);
            }
        }


        [DataTestMethod]
        [DataRow(new double[] { 0.0375, 0 })]
        public void test_ParserSensors_NameFormatException(object Name)
        {
            start(this.Size, boxLocation);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        name = Name,
                        size =  new double[] { 0.0375, 0 },
                        location = new double[] { -0.030, 0.06, 0.089 },
                        shape = "cylinder",
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserSensor(parametrs);
            }
            catch (System.ArgumentException ioex)
            {
                Assert.IsTrue(ioex is ArgumentException);
                return;
            }
            catch (FormatException ex)
            {
                Assert.IsTrue(ex is FormatException);
                return;
            }
            Assert.Fail("Expected was not thrown");
        }


        [DataTestMethod]
        [DataRow("sens5", new double[] { 0.03, 0.05 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        [DataRow("sens4", new double[] { 0.0375, 0, 0.45 }, new double[] { 0.0, 0, 0.08 }, "box")]
        public void test_ParserSensors_ExceptionWitoutShape(string Name, double[] Size, double[] Loc, string Shape)
        {
            start(this.Size, boxLocation);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        name = Name,
                        size = Size,
                        location = Loc,
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserSensor(parametrs);
            }
            catch (Exception ioex)
            {
                Assert.AreEqual(ioex.Message, "The shape (box/cylinder) not specified for the cylinder " + Name);
                return;
            }
            Assert.Fail("Expected was not thrown");
        }


        [DataTestMethod]
        [DataRow("sens5", new double[] { 0.03, 0.05 }, new double[] { -0.030, 0.06, 0.089 }, "circle")]
        [DataRow("sens4", new double[] { 0.0375, 0, 0.45 }, new double[] { 0.0, 0, 0.08 }, "line")]
        [DataRow("sens1", new double[] { 0.0375, 0, 0.45 }, new double[] { 0.0, 0, 0.08 }, "")]
        [DataRow("sens1", new double[] { 0.0375, 0, 0.45 }, new double[] { 0.0, 0, 0.08 }, 9)]

        public void test_ParserSensors_ExceptionInvalidShape(string Name, double[] Size, double[] Loc, object Shape)
        {
            start(this.Size, boxLocation);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        name = Name,
                        size = Size,
                        location = Loc,
                        shape = Shape
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserSensor(parametrs);
            }
            catch (Exception ioex)
            {
                Assert.AreEqual(ioex.Message, "Invalid shape for sensor " + Name.ToString() + " . Specify the box/cylinder shape");
                return;
            }
        }


        [DataTestMethod]
        [DataRow("sens1", new double[] { 0.03, 0.05, 0.004 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        [DataRow("sens2", new double[] { 0.03 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        [DataRow("sens5", new double[] { }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        [DataRow("sens4", new double[] { 0.0375, 0 }, new double[] { 0.0, 0, 0.08 }, "box")]
        [DataRow("sens3", new double[] { }, new double[] { 0.0, 0, 0.08 }, "box")]
        [DataRow("sens9", new double[] { 0.0375, 0, 0.09, 00.8 }, new double[] { 0.0, 0, 0.08 }, "box")]

        public void test_ParserSensors_ExceptionInvalidSize(string Name, double[] Size, double[] Loc, string Shape)
        {
            start(this.Size, boxLocation);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        name = Name,
                        size = Size,
                        location = Loc,
                        shape = Shape
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserSensor(parametrs);
            }
            catch (Exception ioex)
            {
                if (Shape == "box")
                    Assert.AreEqual(ioex.Message, "Invalid size for sensor " + Name + " . For a box shape specified the length, width and height. For example \"size\": [ 0.0889, 0.025, 0.031 ]");
                else
                    Assert.AreEqual(ioex.Message, "Invalid size for sensor " + Name + " . For a cylinder shape specified the radius and length. For example \"size\": [ 0.004, 0.0352 ]");
                return;
            }
            Assert.Fail("Expected was not thrown");
        }


        [DataTestMethod]
        [DataRow("sens1", new object[] { 0.03, "lala" }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        [DataRow("sens2", new object[] { new double[] { }, 0.008 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        [DataRow("sens8", new object[] { "i", 0.778 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        [DataRow("dhg", new object[] { 0.0375, new double[] { }, 0.003 }, new double[] { 0.030, -0.06, 0.089 }, "box")]
        [DataRow("sens12", new object[] { 0, 0.778 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        [DataRow("sens4", new object[] { 12, 0 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        [DataRow("d", new object[] { 0.0375, 0, 0.003 }, new double[] { 0.030, -0.06, 0.089 }, "box")]
        [DataRow("sens5", new object[] { -12, 0.778 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder")]
        [DataRow("sen9", new object[] { 0.0375, 0.9, -0.003 }, new double[] { 0.030, -0.06, 0.089 }, "box")]
        [DataRow("sen10", new object[] { 0.0375, -1, 0.003 }, new double[] { 0.030, -0.06, 0.089 }, "box")]
        public void test_ParserSensors_ExceptionArgumentdSize(string Name, object[] Size, double[] Loc, string Shape)
        {
            start(this.Size, boxLocation);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        name = Name,
                        size = Size,
                        location = Loc,
                        shape = Shape
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserSensor(parametrs);
            }
            catch (ArgumentException ioex)
            {
                Assert.IsTrue(ioex is ArgumentException);
                return;
            }
            catch (FormatException ex)
            {
                Assert.IsTrue(ex is FormatException);
                return;
            }
            Assert.Fail("Expected was not thrown");
        }

        [DataTestMethod]
        [DataRow("sens2", new double[] { 0.0375, 0 }, new double[] { }, "cylinder")]
        [DataRow("sens3", new double[] { 0, 0.030 }, new double[] { -0.030, 0.0 }, "cylinder")]
        [DataRow("sens5", new double[] { 0, 0.05, 0.0099 }, new double[] { -0.030 }, "box")]
        [DataRow("sens5", new double[] { 0, 0.05, 0.0099 }, new double[] { -0.030, 0.06, 0.089, 0.99 }, "box")]
        public void test_ParserSensors_ExceptionInvalidLocation(string Name, double[] Size, double[] Loc, string Shape)
        {
            start(this.Size, boxLocation);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        name = Name,
                        size = Size,
                        location = Loc,
                        shape = Shape
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserSensor(parametrs);
            }
            catch (Exception ioex)
            {
                Assert.AreEqual(ioex.Message, "Invalid location for the cylinder " + Name + ". Specify a location relative to the three coordinate axes, for example:\\\"location\\\": [ 0.1589, 0.018, 0.092 ]");
                return;
            }
            Assert.Fail("Expected was not thrown");

        }


        [DataTestMethod]
        [DataRow("sens1", new double[] { 0.0375, 0.035 }, new object[] { "c", 0, 0.08 }, "cylinder", 1)]
        [DataRow("sens5", new double[] { 0.03, 0.05 }, new object[] { -0.030, new double[] { }, 0.089 }, "cylinder", 2)]
        [DataRow("sens8", new double[] { 0.0375, 0, 0.45 }, new object[] { 0.0, "lala", 0 }, "box", 3)]
        [DataRow("sens12", new double[] { 0, 0.05, 0.0099 }, new object[] { "i", 0.06, 0.089 }, "box", 4)]
        public void test_ParserSensors_ExceptionArgumentLocation(string Name, double[] Size, object[] Loc, string Shape, int not)
        {
            start(this.Size, boxLocation);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        name = Name,
                        size = Size,
                        location = Loc,
                        shape = Shape
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserSensor(parametrs);
            }
            catch (System.ArgumentException ioex)
            {
                Assert.IsTrue(ioex is ArgumentException);
                return;
            }
            catch (FormatException ex)
            {
                Assert.IsTrue(ex is FormatException);
                return;
            }
            Assert.Fail("Expected was not thrown");
        }



        [DataTestMethod]
        [DataRow("sens5", new double[] { 0.03, 0.05 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder", "x", 89, 1)]
        [DataRow("sens4", new double[] { 0.0375, 0, 0.45 }, new double[] { 0.0, 0, 0.08 }, "box", "y", -30, 2)]
        [DataRow("sens5", new double[] { 0, 0.05, 0.0099 }, new double[] { -0.030, 0.06, 0.089 }, "box", "z", 0, 3)]
        [DataRow("sens5", new double[] { 0, 0.05, 0.0099 }, new double[] { -0.030, 0.06, 0.089 }, "box", "y", 90, 2)]
        public void test_ParserSensors_Rotation(string Name, double[] Size, double[] Loc, string Shape, string Axis, double Angl, int ResAxis)
        {
            start(this.Size, boxLocation);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        name = Name,
                        size = Size,
                        location = Loc,
                        shape = Shape,
                        rotation = new object[] {Angl, Axis}
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserSensor(parametrs);
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 1);
            foreach (Sensor s in createBlockCapsule.sensorList)
            {
                Assert.AreEqual(s.getAxis(), ResAxis);
                Assert.AreEqual(s.getAngle(), Angl);
            }
        }


        [DataTestMethod]
        [DataRow("sens5", new double[] { 0.03, 0.05 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder", 10, 90, 1)]
        [DataRow("sens5", new double[] { 0.03, 0.05 }, new double[] { -0.030, 0.06, 0.089 }, "cylinder", "z", "lala", 3)]
        [DataRow("sens5", new double[] { 0, 0.05, 0.0099 }, new double[] { -0.030, 0.06, 0.089 }, "box", new double[] { }, 90, 3)]
        [DataRow("sens5", new double[] { 0, 0.05, 0.0099 }, new double[] { -0.030, 0.06, 0.089 }, "box", "x", new double[] { }, 2)]
        public void test_ParserSensors_ExceptionArgumentRotation(string Name, double[] Size, double[] Loc, string Shape, object Axis, object Angl, int ResAxis)
        {
            start(this.Size, boxLocation);
            var sensor = new
            {
                sensors = new[] {
                    new {
                        name = Name,
                        size = Size,
                        location = Loc,
                        shape = Shape,
                        rotation = new object[] {Angl, Axis}
                    }
                }
            };
            var data = JsonConvert.SerializeObject(sensor);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserSensor(parametrs);
            }
            catch (ArgumentException ioex)
            {
                Assert.IsTrue(ioex is ArgumentException);
                return;
            }
            catch (FormatException ex)
            {
                Assert.IsTrue(ex is FormatException);
                return;
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Incorrect axis of symmetry for holes. Valid are x, y, z, but specified " + Axis.ToString());
                return;
            }
            Assert.Fail("Expected was not thrown");
        }

        [DataTestMethod]
        [DataRow("sens_1", new double[] { 10, 10, 10 }, new double[] { 0, 0, 0 }, "box")]
        [DataRow("sens_1-1", new double[] { 3, 10, 7 }, new double[] { 0, 0, 0 }, "box")]
        [DataRow("sens_2", new double[] { 10, 2, 0.1 }, new double[] { 0, 0, 0 }, "box")]
        [DataRow("sens_3", new double[] { 0, 3, 10 }, new double[] { 0, 0, 0 }, "box")]
        [DataRow("sens_4", new double[] { 2.5, 10, 10 }, new double[] { -2.5, 0, 0 }, "box")]
        [DataRow("sens_4_1", new double[] { 2.5, 5, 10 }, new double[] { 2.5, 0, 0 }, "box")]
        [DataRow("sens_5", new double[] { 10, 2.5, 10 }, new double[] { 0, -2.5, 0 }, "box")]
        [DataRow("sens_5_1", new double[] { 10, 2.5, 10 }, new double[] { 0, 2.5, 0 }, "box")]
        [DataRow("sens_5", new double[] { 10, 10, 2.5 }, new double[] { 0, 0, -2.5 }, "box")]
        [DataRow("sens_5_1", new double[] { 10, 10, 2.5 }, new double[] { 0, 0, 2.5 }, "box")]
        public void test_ParserSensors_CheckSensorInBox_Box(string Name, double[] Size, double[] Loc, string Shape)
        {
            start(new double[] { 10, 10, 10 }, new double[] { 0, 0, 0 });
            Vector loc = Vector.Create(Loc[0], Loc[1], Loc[2]);
            Sensor sensor = new Sensor(Name, 0, Size.ToList(), loc, Shape);
            createBlockCapsule.CheckSensorInBox(sensor);
        }

        [DataTestMethod]
        [DataRow("sens_1", new double[] { 10, 10, 10 }, new double[] { 0, 0, 0 }, "box", 3, 90)]
        [DataRow("sens_1-1", new double[] { 3, 10, 3 }, new double[] { 0, 0, 0 }, "box", 1, 90)]
        [DataRow("sens_2", new double[] { 10, 2, 0.1 }, new double[] { 0, 0, 0 }, "box", 2, 90)]
        [DataRow("sens_3", new double[] { 0, 3, 10 }, new double[] { 0, 0, 0 }, "box", 1, 90)]
        [DataRow("sens_4", new double[] { 2.5, 10, 10 }, new double[] { -2.5, 0, 0 }, "box", 1, 90)]
        [DataRow("sens_4_1", new double[] { 2.5, 5, 10 }, new double[] { 2.5, 0, 0 }, "box", 3, 90)]
        [DataRow("sens_5_1", new double[] { 10, 2.5, 10 }, new double[] { 0, 2.5, 0 }, "box", 2, 90)]

        public void test_ParserSensors_CheckSensorInBox_Box_Rotation(string Name, double[] Size, double[] Loc, string Shape, int Axis, double Angle)
        {
            start(new double[] { 10, 10, 10 }, new double[] { 0, 0, 0 });
            Vector loc = Vector.Create(Loc[0], Loc[1], Loc[2]);
            Sensor sensor = new Sensor(Name, 0, Size.ToList(), loc, Shape);
            createBlockCapsule.CheckSensorInBox(sensor);
            sensor.setRotation(Angle, Axis);
            createBlockCapsule.CheckSensorInBox(sensor);
        }

        [DataTestMethod]
        [DataRow("sens_1", new double[] { 10, 2.5, 10 }, new double[] { 0, -2.5, 0 }, "box", 1, 90, "y")]
        [DataRow("sens_1_1", new double[] { 10, 2.5, 10 }, new double[] { 0, -2.5, 0 }, "box", 3, 90, "y")]
        [DataRow("sens_2", new double[] { 2.5, 10, 10 }, new double[] { 2.5, 0, 0 }, "box", 2, 90, "x")]
        [DataRow("sens_2_1", new double[] { 2.5, 10, 10 }, new double[] { 2.5, 0, 0 }, "box", 3, 90, "x")]
        [DataRow("sens_3", new double[] { 2.5, 10, 2.5 }, new double[] { 2.5, 0, 0 }, "box", 3, 90, "x")]
        [DataRow("sens_4", new double[] { 10, 2.5, 10 }, new double[] { 0, -2.5, 0 }, "box", 1, 90, "y")]
        public void test_ParserSensors_CheckSensorInBox_Box_Rotation_Exception(string Name, double[] Size, double[] Loc, string Shape, int Axis, double Angle, string ResA)
        {
            start(new double[] { 10, 10, 10 }, new double[] { 0, 0, 0 });
            Vector loc = Vector.Create(Loc[0], Loc[1], Loc[2]);
            Sensor sensor = new Sensor(Name, 0, Size.ToList(), loc, Shape);
            sensor.setRotation(Angle, Axis);
            try
            {
                createBlockCapsule.CheckSensorInBox(sensor);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Sensor " + sensor.getName() + " is not in the body volume(axis " +ResA+ ")");
                return;
            }
            Assert.Fail("Expected was not thrown");
        }

        [DataTestMethod]
        [DataRow("sens_1", new double[] { 5, 10}, new double[] { 0, 0, 0 }, "cylinder")]
        [DataRow("sens_1-1", new double[] { 5, 3}, new double[] { 0, 0, 0 }, "cylinder")]
        [DataRow("sens_2", new double[] { 5, 2}, new double[] { 0, 0, 0 }, "cylinder")]
        [DataRow("sens_3", new double[] { 5, 5}, new double[] { 0, 0, 0.25 }, "cylinder")]
        [DataRow("sens_4", new double[] { 2.5, 10}, new double[] { -2.5, 0, 0 }, "cylinder")]
        [DataRow("sens_4_1", new double[] { 2.5, 10}, new double[] { 2.5, 0, 0 }, "cylinder")]
        [DataRow("sens_5", new double[] { 2.5, 10}, new double[] { 0, -2.5, 0 }, "cylinder")]
        [DataRow("sens_5_1", new double[] { 2.5, 10}, new double[] { 0, 2.5, 0 }, "cylinder")]
        [DataRow("sens_6", new double[] { 5, 5}, new double[] { 0, 0, -2.5 }, "cylinder")]
        [DataRow("sens_6_1", new double[] { 5, 5}, new double[] { 0, 0, 2.5 }, "cylinder")]
        public void test_CheckSensorInBox_Cylinder(string Name, double[] Size, double[] Loc, string Shape)
        {
            start(new double[] { 10, 10, 10 }, new double[] { 0, 0, 0 });
            Vector loc = Vector.Create(Loc[0], Loc[1], Loc[2]);
            Sensor sensor = new Sensor(Name, 0, Size.ToList(), loc, Shape);
            createBlockCapsule.CheckSensorInBox(sensor);
        }

        [DataTestMethod]
        [DataRow("sens_1", new double[] { 5, 10 }, new double[] { 0, 0, 0 }, "cylinder", 2, 90, "y")]
        [DataRow("sens_1-1", new double[] { 5, 10 }, new double[] { 0, 0, 0 }, "cylinder", 1, 90, "y")]
        [DataRow("sens_2", new double[] { 5, 10 }, new double[] { 0, 0, 0 }, "cylinder", 3, 90, "y")]
        [DataRow("sens_3", new double[] { 4, 5 }, new double[] { 0, 0, 0.25 }, "cylinder", 1, 90, "x" )]
        [DataRow("sens_4", new double[] { 2.5, 10 }, new double[] { -2.5, 0, 0 }, "cylinder", 3, 90, "x")]
        [DataRow("sens_4_1", new double[] { 2.5,5 }, new double[] { 2.5, 0, 0 }, "cylinder", 2, 90, "x")]
        [DataRow("sens_5", new double[] { 2.5, 5}, new double[] { 0, -2.5, 0 }, "cylinder", 1, 90, "x")]
        [DataRow("sens_5_1", new double[] { 2.5, 10 }, new double[] { 0, 2.5, 0 }, "cylinder", 2, 90, "x")]
        [DataRow("sens_6", new double[] { 5, 5 }, new double[] { 0, 0, -2.5 }, "cylinder", 3, 90, "x")]
        [DataRow("sens_6_1", new double[] { 2.5, 5  }, new double[] { 0, 0, 2.5 }, "cylinder", 2, 90, "x")]
        public void test_CheckSensorInBox_Cylinder_rotation(string Name, double[] Size, double[] Loc, string Shape, int Axis, double Angl, string res)
        {
            start(new double[] { 10, 10, 10 }, new double[] { 0, 0, 0 });
            Vector loc = Vector.Create(Loc[0], Loc[1], Loc[2]);
            Sensor sensor = new Sensor(Name, 0, Size.ToList(), loc, Shape);
            sensor.setRotation(Angl, Axis);
            createBlockCapsule.CheckSensorInBox(sensor);
        }

        //[DataRow("sens_6_1", new double[] { 5, 5 }, new double[] { 0, 0, 2.5 }, "cylinder", 2, 90, "x")]
        //[DataRow("sens_4_1", new double[] { 2.5, 10 }, new double[] { 2.5, 0, 0 }, "cylinder", 2, 90, "x")]
        //[DataRow("sens_5", new double[] { 2.5, 10 }, new double[] { 0, -2.5, 0 }, "cylinder", 1, 90, "x")]


        [DataTestMethod]
        [DataRow("sens_6_1", new double[] { 5, 5 }, new double[] { 0, 0, 2.5 }, "cylinder", 2, 90, "z")]
        [DataRow("sens_4_1", new double[] { 2.5, 10 }, new double[] { 2.5, 0, 0 }, "cylinder", 2, 90, "x")]
        [DataRow("sens_5", new double[] { 2.5, 10 }, new double[] { 0, -2.5, 0 }, "cylinder", 1, 90, "y")]
        public void test_ParserSensors_CheckSensorInBox_Cylinder_Rotation_Exception(string Name, double[] Size, double[] Loc, string Shape, int Axis, double Angle, string ResA)
        {
            start(new double[] { 10, 10, 10 }, new double[] { 0, 0, 0 });
            Vector loc = Vector.Create(Loc[0], Loc[1], Loc[2]);
            Sensor sensor = new Sensor(Name, 0, Size.ToList(), loc, Shape);
            sensor.setRotation(Angle, Axis);
            try
            {
                createBlockCapsule.CheckSensorInBox(sensor);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Sensor " + sensor.getName() + " is not in the body volume(axis " + ResA + ")");
                return;
            }
            Assert.Fail("Expected was not thrown");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    public class ParserRestrictor_test
    {
        HelpMethods helpMethods = new HelpMethods();
        CreateBlockCapsule createBlockCapsule = new CreateBlockCapsule();


        [DataTestMethod]
        [DataRow(new double[] { 0.0375, 0.035, 0.9 }, new double[] { 0, 0, 0 }, 1)]
        [DataRow(new double[] { 0.0375, 0.035, 0.8 }, new double[] { -10, 0, 15 }, 2)]
        [DataRow(new double[] { 0.0375, 0.035, 0.7 }, new double[] { -0.03, -0.4, -0.089}, 3)]
        [DataRow(new double[] { 2, 3, 4 }, new double[] { 0.03, -0.4, 0.089 }, 4)]
        public void test_ParserRestrictor_InitializeWithoutRotation(double[] Size, double[] Loc, int key)
        {
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var restrictors = new
            {
                restrictors = new[] {
                    new {
                        size = Size,
                        location = Loc,
                    }
                }
            };
            var data = JsonConvert.SerializeObject(restrictors);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserRestrictors(parametrs);
            Assert.AreEqual(createBlockCapsule.restrictorsList.Count, 1);
            foreach (Restrictor s in createBlockCapsule.restrictorsList)
            {
                Assert.IsTrue(helpMethods.equalsListDouble(s.getSize(), Size.ToList()));
                Assert.IsTrue(helpMethods.equalsVector(s.getLocation(), Vector.Create(Loc[0], Loc[1], Loc[2])));
            }
        }


        [DataTestMethod]
        [DataRow(new double[] { 0.0375, 0.035, 0.9 }, new double[] { 0, 0, 0 }, 90, "z", 3)]
        [DataRow(new double[] { 0.0375, 0.035, 0.8 }, new double[] { -10, 0, 15 }, 90, "x", 1)]
        [DataRow(new double[] { 0.0375, 0.035, 0.7 }, new double[] { -0.03, -0.4, -0.089 }, 90, "y", 2)]
        [DataRow(new double[] { 2, 3, 4 }, new double[] { 0.03, -0.4, 0.089 }, 90, "y", 2)]
        public void test_ParserRestrictor_InitializeWithRotation(double[] Size, double[] Loc, double Angle, string Axis, int ResA)
        {
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var restrictors = new
            {
                restrictors = new[] {
                    new {
                        size = Size,
                        location = Loc,
                        rotation = new object[]{ Angle, Axis },
                    }
                }
            };
            var data = JsonConvert.SerializeObject(restrictors);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParserRestrictors(parametrs);
            Assert.AreEqual(createBlockCapsule.restrictorsList.Count, 1);
            foreach (Restrictor s in createBlockCapsule.restrictorsList)
            {
                Assert.AreEqual(s.getAngle(), Angle);
                Assert.AreEqual(s.getAxis(), ResA);
                Assert.IsTrue(helpMethods.equalsListDouble(s.getSize(), Size.ToList()));
                Assert.IsTrue(helpMethods.equalsVector(s.getLocation(), Vector.Create(Loc[0], Loc[1], Loc[2])));
            }
        }

        [DataTestMethod]
        [DataRow(new double[] { 0.09, 0.098 }, new double[] { 0, 0, 0 }, 1)]
        [DataRow(new double[] { }, new double[] { -0.03, -0.4, -0.089 }, 2)]
        [DataRow(new double[] { 0.0375 }, new double[] { -10, 0, 15 }, 3)]
        [DataRow(new double[] { 0.0375, 0.9, 0.7, 0.045 }, new double[] { -0.03, -0.4, -0.089 }, 4)]
        public void test_ParserRestrictor_ExceptionNotSpecified(double[] Size, double[] Loc, int key)
        {
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var restrictors = new
            {
               
            };
            var data = JsonConvert.SerializeObject(restrictors);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserRestrictors(parametrs);
            }
            catch (Exception ioex)
            {
                Assert.AreEqual(ioex.Message, "Restrictor not specified");
                return;
            }
            Assert.Fail("Expected Exception was not thrown");
        }

        [DataTestMethod]
        [DataRow(new double[] { 0.09, 0.098}, new double[] { 0, 0, 0 },  1)]
        [DataRow(new double[] {  }, new double[] { -0.03, -0.4, -0.089 },  2)]
        [DataRow(new double[] { 0.0375 }, new double[] { -10, 0, 15 }, 3)]
        [DataRow(new double[] { 0.0375, 0.9, 0.7, 0.045 }, new double[] { -0.03, -0.4, -0.089 },  4)]
        public void test_ParserRestrictor_ExceptionInvalidSize(double[] Size, double[] Loc, int key)
        {
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var restrictors = new
            {
                restrictors = new[] {
                    new {
                        size = Size,
                        location = Loc,
                    }
                }
            };
            var data = JsonConvert.SerializeObject(restrictors);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserRestrictors(parametrs);
            }
            catch (Exception ioex)
            {
                Assert.AreEqual(ioex.Message, "Invalid size for the restrictor number 1. Specify the length, width and height.");
                return;
            }
            Assert.Fail("Expected Exception was not thrown");
        }


        [DataTestMethod]
        [DataRow(new object[] { 0, 0, 0 }, new double[] { 0, 0, 0 }, 0, 1)]
        [DataRow(new object[] { 0.0375, 0, 0.7 }, new double[] { -0.03, -0.4, -0.089 }, 0, 2)]
        [DataRow(new object[] { 0.0375, 0.035, 0 }, new double[] { -10, 0, 15 }, 0, 3)]
        [DataRow(new object[] { -0.0375, 0, 0.7 }, new double[] { -0.03, -0.4, -0.089 }, -0.0375, 4)]
        [DataRow(new object[] { 2, -3, 4 }, new double[] { 0.03, -0.4, 0.089 }, -3, 5)]
        [DataRow(new object[] { 2, 3, -4 }, new double[] { 0.03, -0.4, 0.089 },-4, 6)]
        [DataRow(new object[] { 2, "lala", 4 }, new double[] { 0.03, -0.4, 0.089 }, -3, 7)]
        [DataRow(new object[] { 2, 3, new double[] { } }, new double[] { 0.03, -0.4, 0.089 }, -4, 8)]

        public void test_ParserRestrictor_ArgumentExceptionSize(object[] Size, double[] Loc, double res, int key)
        {
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var restrictors = new
            {
                restrictors = new[] {
                    new {
                        size = Size,
                        location = Loc,
                    }
                }
            };
            var data = JsonConvert.SerializeObject(restrictors);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserRestrictors(parametrs);
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

            Assert.Fail("Expected Exception was not thrown");
        }


        [DataTestMethod]
        [DataRow(new double[] { 0.09, 0.098, 0.09 }, new double[] { 0}, 1)]
        [DataRow(new double[] { 0.09, 0.098 , 0.09}, new double[] { -0.03, -0.4}, 2)]
        [DataRow(new double[] { 0.0375, 0.09, 0.09 }, new double[] { -10, 0, 15, 67 }, 3)]
        [DataRow(new double[] { 0.0375, 0.9, 0.7 }, new double[] {}, 4)]
        public void test_ParserRestrictor_ExceptionInvalidLocation(double[] Size, double[] Loc, int key)
        {
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var restrictors = new
            {
                restrictors = new[] {
                    new {
                        size = Size,
                        location = Loc,
                    }
                }
            };
            var data = JsonConvert.SerializeObject(restrictors);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserRestrictors(parametrs);
            }
            catch (Exception ioex)
            {
                Assert.AreEqual(ioex.Message, "Invalid location for the restrictor number " + 1 + ". Specify a location relative to the three coordinate axes, for example:\\\"location\\\": [ 0.1589, 0.018, 0.092 ]");
                return;
            }
            Assert.Fail("Expected Exception was not thrown");
        }


        [DataTestMethod]
        [DataRow(new double[] { 0.0375, 0.035, 0.9 }, new object[] { 0, 0, "lala" }, 1)]
        [DataRow(new double[] { 0.0375, 0.035, 0.8 }, new object[] { -10, "", 15 }, 2)]
        [DataRow(new double[] { 0.0375, 0.035, 0.7 }, new object[] { "k", -0.4, -0.089 }, 3)]
        [DataRow(new double[] { 2, 3, 4 }, new object[] { 0.03, -0.4, new double[] { } }, 4)]
        public void test_ParserRestrictor_ArgumentExceptionLocation(double[] Size, object[] Loc, int key)
        {
            Assert.AreEqual(createBlockCapsule.sensorList.Count, 0);
            var restrictors = new
            {
                restrictors = new[] {
                    new {
                        size = Size,
                        location = Loc,
                    }
                }
            };
            var data = JsonConvert.SerializeObject(restrictors);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParserRestrictors(parametrs);
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

            Assert.Fail("Expected Exception was not thrown");
        }

    }
}

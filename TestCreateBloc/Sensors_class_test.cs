using CreateBody;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceClaim.Api.V23.Geometry;
using System.Drawing;
using SpaceClaim.Api.V23;

namespace TestCreateBloc
{
    [TestClass]
    public class Sensors_class_test
    {
        HelpMethods helpMethods = new HelpMethods();
        CreateBlockCapsule createBlockCapsule = new CreateBlockCapsule();

        [TestMethod]
        public void test_Sensors_initialize()
        {
            Vector loc = Vector.Create(0, 0, 0);
            string name = "sens1";
            List<double> size = new List<double>();
            string shape = "box";
            double load = 0;

            Sensor sensor = new Sensor(name, load, size, loc, shape);
            Assert.IsNotNull(sensor);
            Assert.IsTrue(sensor.getName() is string);
            Assert.AreEqual(sensor.getName(), name);
            Assert.IsTrue(sensor.getLoad() is double);
            Assert.AreEqual(sensor.getLoad(), load);
            Assert.IsTrue(helpMethods.equalsVector(sensor.getLocation(), loc));
            Assert.IsTrue(sensor.getShape() is string);
            Assert.AreEqual(sensor.getShape(), shape);
            Assert.IsTrue(helpMethods.equalsListDouble(sensor.getSize(), size));
            Assert.IsTrue(sensor.getAngle() is double);
            Assert.AreEqual(sensor.getAngle(), 0);
            Assert.IsTrue(sensor.getAxis() is int);
            Assert.AreEqual(sensor.getAxis(), 0);
        }


        [DataTestMethod]
        [DataRow(1, 90)]
        [DataRow(2, 140)]
        [DataRow(0, 90.78)]
        [DataRow(3, 90.78)]
        public void test_Sensors_setRotation(int axis, double angle)
        {
            Vector loc = Vector.Create(0, 0, 0);
            string name = "sens1";
            List<double> size = new List<double>();
            string shape = "box";
            double load = 0;

            Sensor sensor = new Sensor(name, load, size, loc, shape);
            Assert.AreEqual(sensor.getName(), name);
            Assert.AreEqual(sensor.getLoad(), load);
            Assert.IsTrue(helpMethods.equalsVector(sensor.getLocation(), loc));
            Assert.AreEqual(sensor.getShape(), shape);
            Assert.IsTrue(helpMethods.equalsListDouble(sensor.getSize(), size));
            Assert.AreEqual(sensor.getAngle(), 0);
            Assert.AreEqual(sensor.getAxis(), 0);

            sensor.setRotation(angle, axis);
            Assert.AreEqual(sensor.getAngle(), angle);
            Assert.AreEqual(sensor.getAxis(), axis);

        }
    }
}


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CreateBody;
using SpaceClaim;
using SpaceClaim.Api.V23.Geometry;

namespace TestCreateBloc
{
    [TestClass]
    public class Restrictor_class_test
    {

        HelpMethods helpMethods = new HelpMethods();
        CreateBlockCapsule createBlockCapsule = new CreateBlockCapsule();

        [TestMethod]
        public void test_Restrictor_initialize()
        {
            Vector location = Vector.Create(0, 0, 0);
            List<double> size = new List<double>() {1, 1, 1 };
            Restrictor restrictor = new Restrictor(size, location);
            Assert.IsNotNull(restrictor);
            Assert.IsNotNull(restrictor.getSize());
            Assert.IsNotNull(restrictor.getLocation());
            Assert.IsNotNull(restrictor.getAngle());
            Assert.IsNotNull(restrictor.getAxis());

            Assert.IsTrue(helpMethods.equalsVector(restrictor.getLocation(), location));
            Assert.IsTrue(helpMethods.equalsListDouble(restrictor.getSize(), size));
            Assert.AreEqual(restrictor.getAxis(), 0);
            Assert.AreEqual(restrictor.getAngle(), 0);
        }

        [DataTestMethod]
        [DataRow(1, 90)]
        [DataRow(2, 140)]
        [DataRow(0, 90.78)]
        [DataRow(3, 90.78)]
        public void test_Restrictor_setRotation(int axis, double angle)
        {
            Vector location = Vector.Create(0, 0, 0);
            List<double> size = new List<double>() { 1, 1, 1 };
            Restrictor restrictor = new Restrictor(size, location);

            restrictor.setRotation(angle, axis);
            Assert.AreEqual(restrictor.getAngle(), angle);
            Assert.AreEqual(restrictor.getAxis(), axis);
        }
    }


}

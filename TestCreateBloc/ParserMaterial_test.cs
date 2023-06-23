using CreateBody;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceClaim.Api.V23.Geometry;
using Newtonsoft.Json;

namespace TestCreateBloc
{
    [TestClass]
    public class ParserMaterial_test
    {
        CreateBlockCapsule createBlockCapsule = new CreateBlockCapsule();

        [DataTestMethod]
        [DataRow("Plastic, ABC (homopolymer, low flow)")]
        [DataRow(1234)]
        public void test_ParserMaterial_Initialize(object material)
        {
            var myData = new
            {
                materials = material,

            };
            var data = JsonConvert.SerializeObject(myData);
            var parametrs = JsonConvert.DeserializeObject(data);
            Assert.AreEqual(createBlockCapsule.material, "Plastic, PP (homopolymer, low flow)");
            createBlockCapsule.ParsMaterials(parametrs);
            Assert.IsTrue(createBlockCapsule.material is string);
            Assert.AreEqual(createBlockCapsule.material, material.ToString());
        }

        [DataTestMethod]
        [DataRow(1, 1)]
        [DataRow(2, 2)]
        [DataRow(2.1, 2)]
        [DataRow(2.9, 3)]
        public void test_ParserMaterial_ParsNumber(object Number, int res)
        {
            var myData = new
            {
                number = Number,

            };
            var data = JsonConvert.SerializeObject(myData);
            var parametrs = JsonConvert.DeserializeObject(data);
            createBlockCapsule.ParsMaterials(parametrs);
            Assert.IsTrue(createBlockCapsule.number is int);
            Assert.AreEqual(createBlockCapsule.number,res);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(-1.9)]
        [DataRow(0)]
        [DataRow("lsls")]
        [DataRow(new double[] { 1 })]
        public void test_ParserMaterial_ParsNumberExeption(object Number)
        {
            var myData = new
            {
                number = Number,

            };
            var data = JsonConvert.SerializeObject(myData);
            var parametrs = JsonConvert.DeserializeObject(data);
            try
            {
                createBlockCapsule.ParsMaterials(parametrs);
            }
            catch (ArgumentException ioex)
            {
                Assert.IsTrue(ioex is ArgumentException);
                return;
            }
            catch (FormatException ioex)
            {
                Assert.IsTrue(ioex is FormatException);
                return;
            }
            Assert.Fail("Expected Exception was not thrown");
        }

    }
}

using SpaceClaim.Api.V23.Scripting.Commands.CommandOptions;
using SpaceClaim.Api.V23;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceClaim.Api.V23.Geometry;

namespace CreateBody
{
    public class Restrictor
    {
        List<double> size;
        Vector location;
        double angle = 0;
        int axis = 0; //x = 1, y = 2, z = 3

        public Restrictor(List<double> size, Vector location)
        {
            this.size = size;
            this.location = location;
        }
    public Vector getLocation() { return this.location; }

    public List<double> getSize() { return this.size; }
        public void setRotation(double angle, int axis)
        {
            this.axis = axis;
            this.angle = angle;
        }
        public int getAxis()
        {
            return axis;
        }

        public double getAngle()
        {
            return angle;
        }
    }
}

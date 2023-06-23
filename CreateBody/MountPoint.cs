using System.Collections.Generic;
using SpaceClaim.Api.V23.Geometry;

namespace CreateBody
{
    public class MountPoint
    {
        double radius;
        double length;
        Vector location;
        double angle = 0;
        int axis = 0; //x = 1, y = 2, z = 3
        int count = 1;
        int pointSimetricAxis = 0;
        List<Vector> simPointLocation = new List<Vector>();

        public MountPoint(double radius, double extrude, Vector location)
        {
            this.radius = radius;
            this.length = extrude;
            this.location = location;

        }

        public void addSimPointLocation(Vector location)
        {
            simPointLocation.Add(location);
        }

        public List<Vector> getSimPointLocation()
        {
            return simPointLocation;
        }

        public void setCountOfPoint(int count)
        {
            this.count = count;
        }

        public void setPointSimetricAxis(int pointSimetricAxis)
        {
            this.pointSimetricAxis= pointSimetricAxis;
        }

        public int getPointSimetricAxis()
        {
            return pointSimetricAxis;
        }


        public void setRotation(double angle, int axis) {
            this.axis = axis;
            this.angle = angle;
        }

        public int getPointCount()
        {
            return count;
        }

        public double getRadius()
        {
            return radius;
        }

        public Vector getLocation()
        {
            return location;
        }
        public int getAxis() 
        { 
        return axis;
        }

        public double getAngle()
        {
            return angle;
        }

        public double getLength()
        {
            return length;
        }

        public void setSimPointLocation(List<Vector> simPointLocation)
        {
            this.simPointLocation = simPointLocation;
        }
    }
}

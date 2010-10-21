using System;
using System.Configuration;
using System.Web;

namespace gMapMaker
{
    public class LatLongPair
    {
        protected double topLatField;
        protected double leftLongField;
        protected double bottomLatField;
        protected double rightLongField;

        public LatLongPair(double lat1, double lng1, double lat2, double lng2)
        {
            topLatField = lat1;
            leftLongField = lng1;
            bottomLatField = lat2;
            rightLongField = lng2;
        }
        
        public double TopLat
        {
            get
            {
                return topLatField;
            }
        }
        public double LeftLong
        {
            get
            {
                return leftLongField;
            }
        }
        public double BottomLat
        {
            get
            {
                return bottomLatField;
            }
        }
        public double RightLong
        {
            get
            {
                return rightLongField;
            }
        }
        public double LongWidth
        {
            get
            {
                return rightLongField - leftLongField;
            }
        }
        public double LatHeight
        {
            get
            {
                return topLatField - bottomLatField;
            }
        }
    }
}

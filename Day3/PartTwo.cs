using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Day3.partTwo
{
    class PuzzleTwo
    {


        private string _fileData;
        private List<List<string>> _PathDataList;

        public PuzzleTwo LoadData(string fileLocation)
        {

            if (File.Exists(fileLocation))
            {
                this._fileData = File.ReadAllText(fileLocation);
                return this;
            }
            else
                return null;

        }

        public PuzzleTwo ParseData()
        {
            string[] DataSplitBylines;
            this._PathDataList = new List<List<string>>();


            DataSplitBylines = this._fileData.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            foreach (string aLine in DataSplitBylines)
            {
                List<string> pathData = new List<string>();
                string[] eachPathArray = aLine.Split(",", StringSplitOptions.RemoveEmptyEntries);
                pathData.AddRange(eachPathArray);

                this._PathDataList.Add(pathData);

            }

            return this;
        }

        public int FindIntersections()
        {
            List<List<Point>> PathDataListAsPoints = this.ConvertAllDataToPoints();
            // list of points that where different wires overlap
            List<Point> intersectionPoints = new List<Point>();
            List<IntersectionsPointData> intersectionData = new List<IntersectionsPointData>();

            // find out which lines intersect
            for (int eachLineData = 0; eachLineData < PathDataListAsPoints.Count; eachLineData++)
            {
                List<Point> CurrentLineData = PathDataListAsPoints[eachLineData];

                for (int lineStartIndex = 0; lineStartIndex < CurrentLineData.Count - 1; lineStartIndex++)
                {
                    Point CurrentLineStartPoint = CurrentLineData[lineStartIndex];
                    Point CurrentLineEndPoint = CurrentLineData[lineStartIndex + 1];

                    for (int lineDataToCheckIndex = eachLineData + 1; lineDataToCheckIndex < PathDataListAsPoints.Count; lineDataToCheckIndex++)
                    {
                        List<Point> LineDataToCheckAgainst = PathDataListAsPoints[lineDataToCheckIndex];

                        for (int eachPointPostion = 0; eachPointPostion < (LineDataToCheckAgainst.Count - 1); eachPointPostion++)
                        {
                            Point ComparingLineStartPoint = LineDataToCheckAgainst[eachPointPostion];
                            Point ComparingLineEndPoint = LineDataToCheckAgainst[eachPointPostion + 1];

                            int interesctionAnswer;
                            float x, y;

                            if (lineStartIndex == 0 || eachPointPostion == 0)
                                continue;

                            interesctionAnswer = this.get_line_intersection(CurrentLineStartPoint.x, CurrentLineStartPoint.y, CurrentLineEndPoint.x, CurrentLineEndPoint.y,
                                                       ComparingLineStartPoint.x, ComparingLineStartPoint.y, ComparingLineEndPoint.x, ComparingLineEndPoint.y,
                                                       out x, out y);

                            if (interesctionAnswer == 1)
                            {
                                intersectionPoints.Add(new Point((int)x, (int)y));
                                IntersectionsPointData intersectData = new IntersectionsPointData((int)x, (int)y,
                                    new Line(CurrentLineStartPoint, CurrentLineEndPoint),
                                    new Line(ComparingLineStartPoint, ComparingLineEndPoint));

                                intersectionData.Add(intersectData);
                            }
                        }
                    }
                }



            }

            Point ClosestIntersectionPoint = this.FindClosesIntersectionPoint(intersectionPoints);
            this.FindLowestIntersectionBetweenSumOfBothWires(intersectionData);

            return this.FindLowestIntersectionBetweenSumOfBothWires(intersectionData);
        }

        private Point FindClosesIntersectionPoint(List<Point> intersectionPoints)
        {
            int Distance = 0;
            Point ClosestPoint = intersectionPoints[0];

            Distance = intersectionPoints[0].x + intersectionPoints[0].y;
            for (int i = 1; i < intersectionPoints.Count; i++)
            {
                Point aPoint = intersectionPoints[i];
                int temp = aPoint.x + aPoint.y;
                if ((aPoint.x + aPoint.y) < Distance)
                {
                    Distance = aPoint.x + aPoint.y;
                    ClosestPoint = aPoint;
                }
            }

            return ClosestPoint;
        }

        private int FindLowestIntersectionBetweenSumOfBothWires(List<IntersectionsPointData> intersectionData)
        {
            int Distance = intersectionData[0].DistanceToLineOneIntersection + intersectionData[0].DistanceToLineTwoIntersection;
            IntersectionsPointData closetIntersection = intersectionData[0];

            for(int i = 1; i < intersectionData.Count; i++)
            {
                IntersectionsPointData anIntersectData = intersectionData[i];

                if( (anIntersectData.DistanceToLineOneIntersection + anIntersectData.DistanceToLineTwoIntersection) < Distance)
                {
                    closetIntersection = intersectionData[i];
                    Distance = anIntersectData.DistanceToLineOneIntersection + anIntersectData.DistanceToLineTwoIntersection;
                }
            }

            return Distance;
        }

        private List<List<Point>> ConvertAllDataToPoints()
        {
            List<List<Point>> PathDataListAsPoints = new List<List<Point>>();

            foreach (List<string> lineData in this._PathDataList)
            {
                List<Point> LineDataAsPoints = new List<Point>();
                Point currentPoint = new Point(0, 0);
                // used for caculating distance to last point
                Point previousePoint = currentPoint;

                LineDataAsPoints.Add(currentPoint);
                foreach (string PathDataAsString in lineData)
                {
                    currentPoint = this.ConvertToPoint(PathDataAsString, currentPoint);
                    currentPoint.CaculateDistance(previousePoint);
                    LineDataAsPoints.Add(currentPoint);

                    previousePoint = currentPoint;
                }

                PathDataListAsPoints.Add(LineDataAsPoints);
            }

            return PathDataListAsPoints;
        }


        private Point ConvertToPoint(string inputData, Point PreviousePointLocation)
        {
            string direction = inputData.Substring(0, 1);
            int moveAmmount = int.Parse(inputData.Substring(1));

            Point newPointLocation = new Point();

            switch (direction.ToUpper())
            {
                case "L":

                    newPointLocation.x = PreviousePointLocation.x - moveAmmount;
                    newPointLocation.y = PreviousePointLocation.y;
                    break;

                case "R":

                    newPointLocation.x = PreviousePointLocation.x + moveAmmount;
                    newPointLocation.y = PreviousePointLocation.y;
                    break;

                case "U":

                    newPointLocation.x = PreviousePointLocation.x;
                    newPointLocation.y = PreviousePointLocation.y + moveAmmount;
                    break;

                case "D":

                    newPointLocation.x = PreviousePointLocation.x;
                    newPointLocation.y = PreviousePointLocation.y - moveAmmount;
                    break;

            }

            return newPointLocation;

        }

        // got this off stack overflow. credit goes to them.
        // https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
        // Returns 1 if the lines intersect, otherwise 0. In addition, if the lines 
        // intersect the intersection point may be stored in the floats i_x and i_y.
        private int get_line_intersection(float p0_x, float p0_y, float p1_x, float p1_y,
            float p2_x, float p2_y, float p3_x, float p3_y, out float i_x, out float i_y)
        {
            float s1_x, s1_y, s2_x, s2_y;
            s1_x = p1_x - p0_x; s1_y = p1_y - p0_y;
            s2_x = p3_x - p2_x; s2_y = p3_y - p2_y;

            float s, t;
            s = (-s1_y * (p0_x - p2_x) + s1_x * (p0_y - p2_y)) / (-s2_x * s1_y + s1_x * s2_y);
            t = (s2_x * (p0_y - p2_y) - s2_y * (p0_x - p2_x)) / (-s2_x * s1_y + s1_x * s2_y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                i_x = p0_x + (t * s1_x);
                i_y = p0_y + (t * s1_y);
                return 1;
            }

            i_x = -1;
            i_y = -1;
            return 0; // No collision
        }
    }



    public class Point
    {
        public int x = 0, y = 0;

        public int DistanceFromLastPoint = 0;
        public int DistanceFromStart = 0;

        public Point()
        {
            
        }
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void CaculateDistance(Point PointToCompare)
        {
            //this.DistanceFromLastPoint = ((PointToCompare.x - this.x) * (PointToCompare.x - this.x) + (PointToCompare.y - this.y) * (PointToCompare.y - this.y));
            
            this.DistanceFromLastPoint = Math.Abs((PointToCompare.x - this.x) + (PointToCompare.y - this.y));
            this.DistanceFromStart = PointToCompare.DistanceFromStart + this.DistanceFromLastPoint;
        }
    }
    public class Line
    {
        public Point Start, End;
        public int Distance;

        public int DistanceFromStart;

        public Line(Point start, Point end)
        {
            this.Start = start;
            this.End = end;

            this.CaculateDistanceBetweenPoints();
        }

        public Line(int x1, int y1, int x2, int y2)
        {
            this.Start = new Point(x1, y1);
            this.End = new Point(x2, y2);

            this.CaculateDistanceBetweenPoints();
        }

        private void CaculateDistanceBetweenPoints()
        {
            //this.Distance = ((Start.x - End.x) * (Start.x - End.x) + (Start.y - End.y) * (Start.y - End.y));
            this.Distance = Math.Abs((Start.x - End.x) + (Start.y - End.y));
        }


    }

    public class IntersectionsPointData
    {
        public int PointOfIntersectionX;
        public int PointOfIntersectionY;

        Line LineOne;
        Line LineTwo;

        public int DistanceToLineOneIntersection;
        public int DistanceToLineTwoIntersection;

        public IntersectionsPointData(int PointOfIntersectionX, int PointOfIntersectionY, Line LineOne, Line LineTwo)
        {
            this.PointOfIntersectionX = PointOfIntersectionX;
            this.PointOfIntersectionY = PointOfIntersectionY;

            this.LineOne = LineOne;
            this.LineTwo = LineTwo;

            this.CaculateDistances();
        }

        public void CaculateDistances()
        {
            //int distanceOne = ((LineOne.Start.x - PointOfIntersectionX) * (LineOne.Start.x - PointOfIntersectionX) + (LineOne.Start.y - PointOfIntersectionY) * (LineOne.Start.y - PointOfIntersectionY));
            //int distanceTwo = ((LineTwo.Start.x - PointOfIntersectionX) * (LineTwo.Start.x - PointOfIntersectionX) + (LineTwo.Start.y - PointOfIntersectionY) * (LineTwo.Start.y - PointOfIntersectionY));

            int distanceOne = Math.Abs((LineOne.Start.x - PointOfIntersectionX) + (LineOne.Start.y - PointOfIntersectionY));
            int distanceTwo = Math.Abs((LineTwo.Start.x - PointOfIntersectionX) + (LineTwo.Start.y - PointOfIntersectionY));

            this.DistanceToLineOneIntersection = this.LineOne.Start.DistanceFromStart + distanceOne;
            this.DistanceToLineTwoIntersection = this.LineTwo.Start.DistanceFromStart + distanceTwo;
        }
    }

}

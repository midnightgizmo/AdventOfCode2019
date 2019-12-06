using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Day3
{
    public class PuzzleOne
    {

        public PuzzleOne()
        {
            Line lineOne = new Line(0, 0, 4, 5);
            //Line lineTwo = new Line(0, 10, 10, 0);
            Line lineTwo = new Line(0, 10, 4, 6);
            float x, y;
            int answer = this.get_line_intersection(lineOne.Start.x, lineOne.Start.y, lineOne.End.x, lineOne.End.y,
                                                    lineTwo.Start.x, lineTwo.Start.y, lineTwo.End.x, lineTwo.End.y,
                                                    out x, out y);

            int i = 0;

        }


        private string _fileData;
        private List<List<string>> _PathDataList;

        public PuzzleOne LoadData(string fileLocation)
        {

            if (File.Exists(fileLocation))
            {
                this._fileData = File.ReadAllText(fileLocation);
                return this;
            }
            else
                return null;

        }

        public PuzzleOne ParseData()
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

            // find out which lines intersect
            for(int eachLineData = 0; eachLineData < PathDataListAsPoints.Count; eachLineData++)
            {
                List<Point> CurrentLineData = PathDataListAsPoints[eachLineData];

                for (int lineStartIndex = 0; lineStartIndex < CurrentLineData.Count - 1; lineStartIndex++)
                {
                    Point CurrentLineStartPoint = CurrentLineData[lineStartIndex];
                    Point CurrentLineEndPoint = CurrentLineData[lineStartIndex + 1];

                    for (int lineDataToCheckIndex = eachLineData + 1; lineDataToCheckIndex < PathDataListAsPoints.Count; lineDataToCheckIndex++)
                    {
                        List<Point> LineDataToCheckAgainst = PathDataListAsPoints[lineDataToCheckIndex];

                        for(int eachPointPostion = 0; eachPointPostion < (LineDataToCheckAgainst.Count - 1); eachPointPostion++)
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

                            if(interesctionAnswer == 1)
                            {
                                intersectionPoints.Add(new Point((int)x, (int)y));
                            }
                        }
                    }
                }
      

                
            }

            Point ClosestIntersectionPoint = FindClosesIntersectionPoint(intersectionPoints);

            return ClosestIntersectionPoint.x + ClosestIntersectionPoint.y;
        }

        private Point FindClosesIntersectionPoint(List<Point> intersectionPoints)
        {
            int Distance = 0;
            Point ClosestPoint = intersectionPoints[0];

            Distance = intersectionPoints[0].x + intersectionPoints[0].y;
            for(int i = 1; i < intersectionPoints.Count;i++)
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

        private List<List<Point>> ConvertAllDataToPoints()
        {
            List<List<Point>> PathDataListAsPoints = new List<List<Point>>();

            foreach (List<string> lineData in this._PathDataList)
            {
                List<Point> LineDataAsPoints = new List<Point>();
                Point currentPoint = new Point(0, 0);

                LineDataAsPoints.Add(currentPoint);
                foreach (string PathDataAsString in lineData)
                {
                    currentPoint = this.ConvertToPoint(PathDataAsString, currentPoint);
                    LineDataAsPoints.Add(currentPoint);
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

            switch(direction.ToUpper())
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


    struct Point
    {
        public int x, y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    struct Line
    {
        public Point Start, End;

        public Line(Point start, Point end)
        {
            this.Start = start;
            this.End = end;
        }

        public Line(int x1, int y1, int x2, int y2)
        {
            this.Start = new Point(x1, y1);
            this.End = new Point(x2, y2);
        }


    }

}

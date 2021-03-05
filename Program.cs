using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace HashCode2021TrafficSignalling
{
    class Program
    {
        static string path = @"InputFiles\";
        static string fileName = "a.txt";
        static void Main(string[] args)
        {
            try
            {
                TrafficLights TrafficLights = ReadInputFile(path + fileName);
                OutPutFile outPutFile = new OutPutFile();
                outPutFile = ScheduleIntersection(TrafficLights);
                WriteOutputFile(outPutFile);
            }
            catch (Exception exc) { Console.WriteLine(exc.Message); }
        }

        private static OutPutFile ScheduleIntersection(TrafficLights trafficLights)
        {
            OutPutFile outPutFile = new OutPutFile();
            List<IntersectionSchedule> intersectionScheduleList = new List<IntersectionSchedule>();
            int intersections = trafficLights.NumOfIntersection;
            int streets = trafficLights.NumOfStreets;
            int cars = trafficLights.NumOfCars;
            var UniqueIntersections = trafficLights.streets.Select(p => p.IntersectionEndStreet).Distinct().ToList();

            for (int i = 0; i < UniqueIntersections.Count; i++)
            {
                int currentIntersection = UniqueIntersections[i];
                var allcarPaths = trafficLights.carPaths;

                var allStreets = trafficLights.streets.Where(p => p.IntersectionEndStreet == currentIntersection).ToList();

                IntersectionSchedule intersectionSchedule = new IntersectionSchedule();

                intersectionSchedule.NumberncomingOfStreets = 0;
                intersectionSchedule.NumberncomingOfStreets = allStreets.Count;
                intersectionSchedule.IntersectionID = currentIntersection;
                foreach (var street in allStreets)
                {
                    var currentStreet = street;

                    OrderNDurationGreenLight orderNDurationGreenLight = new OrderNDurationGreenLight();
                    orderNDurationGreenLight.StreetName = currentStreet.StreetName;
                    //second + 1 for car passing
                    int isStartofPath = 0;
                    foreach (var carPath in allcarPaths)
                    {
                        if (carPath.StreetName[0] == currentStreet.StreetName)
                            isStartofPath = 1;
                    }
                    orderNDurationGreenLight.GreenSeconds = 1 + isStartofPath;
                    intersectionSchedule.orderNDurationGreenLights.Add(orderNDurationGreenLight);

                }
                if (intersectionSchedule != null)
                {
                    intersectionSchedule.orderNDurationGreenLights = intersectionSchedule.orderNDurationGreenLights.OrderByDescending(p => p.GreenSeconds).ToList();
                    intersectionScheduleList.Add(intersectionSchedule);
                    intersectionScheduleList = intersectionScheduleList.OrderByDescending(p => p.NumberncomingOfStreets).ToList();

                    decimal IntersectionPercentage = (decimal)intersectionScheduleList.Count / (decimal)UniqueIntersections.Count;
                    Console.WriteLine(intersectionScheduleList.Count + " / " + UniqueIntersections.Count +
                    " = " + IntersectionPercentage);
                }
            }
            outPutFile.intersectionSchedules = intersectionScheduleList;
            outPutFile.NumIntersectionHasSchedule = intersectionScheduleList.Count;
            return outPutFile;
        }

        private static TrafficLights ReadInputFile(string fileName)
        {
            var lines = File.ReadAllLines(fileName);

            var line = lines[0];
            var fline = line.Split(' ');
            TrafficLights trafficLight = new TrafficLights();
            trafficLight.durationinSec = int.Parse(fline[0]);
            trafficLight.NumOfIntersection = int.Parse(fline[1]);
            trafficLight.NumOfStreets = int.Parse(fline[2]);
            trafficLight.NumOfCars = int.Parse(fline[3]);
            trafficLight.BonusPointsBeforeTime = int.Parse(fline[4]);

            for (var i = 1; i < trafficLight.NumOfStreets + 1; i++)
            {
                var otherLines = lines[i].Split(' ');
                Street street = new Street();
                street.IntersectionStartStreet = int.Parse(otherLines[0]);
                street.IntersectionEndStreet = int.Parse(otherLines[1]);
                street.StreetName = otherLines[2];
                street.TimeStartEndStreet = int.Parse(otherLines[3]);
                trafficLight.streets.Add(street);
            }

            for (var i = trafficLight.streets.Count + 1; i < trafficLight.streets.Count + trafficLight.NumOfCars + 1; i++)
            {
                var otherLines = lines[i].Split(' ');
                CarPath carPath = new CarPath();
                carPath.NumOfStreet = int.Parse(otherLines[0]);
                carPath.StreetName = otherLines.Skip(1).ToList();
                trafficLight.carPaths.Add(carPath);
            }
            trafficLight.carPaths = trafficLight.carPaths.OrderBy(p => p.NumOfStreet).ToList();
            return trafficLight;
        }

        private static void WriteOutputFile(OutPutFile outPut)
        {
            string createText = outPut.NumIntersectionHasSchedule + Environment.NewLine;

            foreach (var op in outPut.intersectionSchedules)
            {
                createText += op.IntersectionID + Environment.NewLine + op.NumberncomingOfStreets + Environment.NewLine;
                foreach (var abc in op.orderNDurationGreenLights)
                {
                    createText += abc.StreetName + " " + abc.GreenSeconds + Environment.NewLine;
                }
            }
            File.WriteAllText(path + fileName + "_Output.txt", createText);

        }
        class TrafficLights
        {
            public int durationinSec { get; set; }
            public int NumOfIntersection { get; set; }
            public int NumOfStreets { get; set; }
            public int NumOfCars { get; set; }
            public int BonusPointsBeforeTime { get; set; }


            public List<Street> streets = new List<Street>();
            public List<CarPath> carPaths = new List<CarPath>();

            public TrafficLights() { }
        }
        class Street
        {
            public int IntersectionStartStreet { get; set; }
            public int IntersectionEndStreet { get; set; }
            public string StreetName { get; set; }
            public int TimeStartEndStreet { get; set; }

        }
        class CarPath
        {
            public int NumOfStreet { get; set; }
            public List<string> StreetName { get; set; }
        }


        class OutPutFile
        {
            public int NumIntersectionHasSchedule { get; set; }

            public List<IntersectionSchedule> intersectionSchedules = new List<IntersectionSchedule>();

        }
        class IntersectionSchedule
        {
            public int IntersectionID { get; set; }
            public int NumberncomingOfStreets { get; set; }
            public List<OrderNDurationGreenLight> orderNDurationGreenLights = new List<OrderNDurationGreenLight>();
        }
        class OrderNDurationGreenLight
        {
            public string StreetName { get; set; }
            public int GreenSeconds { get; set; }
        }
    }
}

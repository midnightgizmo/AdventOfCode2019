using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Day6
{
    public class PuzzleOne
    {
        private string _fileData;
        private List<string[]> _parsedFileData;

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
            string[] DataSplitByLines;
            this._parsedFileData = new List<string[]>();


            DataSplitByLines = this._fileData.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            foreach (string aLine in DataSplitByLines)
            {
                string[] splitData = aLine.Split(new char[] { ')' });
                this._parsedFileData.Add(new string[] { splitData[0], splitData[1] });
            }

            return this;
        }

        public int ComputeTotalDirectAndIndirectOrbits()
        {
            Planet UniversalCenterOfMass = new Planet();
            List<Planet> ListOfAllplanets = new List<Planet>();

            UniversalCenterOfMass.PlanetName = "COM";

            foreach (string[] orbitData in this._parsedFileData)
            {
                string planetName = orbitData[1];
                string planetNameItOrbits = orbitData[0];

                Planet PlanetThatOrbits = ListOfAllplanets.Find((s) => s.PlanetName == planetName);
                if(PlanetThatOrbits == null)
                {
                    PlanetThatOrbits = new Planet();
                    PlanetThatOrbits.PlanetName = planetName;

                    ListOfAllplanets.Add(PlanetThatOrbits);
                }

                Planet PlanetBeingOrbited;

                if (orbitData[0] == "COM")
                    PlanetBeingOrbited = UniversalCenterOfMass;
                else
                    PlanetBeingOrbited = ListOfAllplanets.Find((s) => s.PlanetName == planetNameItOrbits);


                if (PlanetBeingOrbited == null)
                {
                    PlanetBeingOrbited = new Planet();
                    PlanetBeingOrbited.PlanetName = planetNameItOrbits;
                    ListOfAllplanets.Add(PlanetBeingOrbited);
                }
                PlanetBeingOrbited.PlanetsThatOrbitThisPlanet.Add(PlanetThatOrbits);
                PlanetThatOrbits.OrbitOf = PlanetBeingOrbited;

            }

            int Count = 0;
            foreach(Planet aPlanet in ListOfAllplanets)
            {
                Count += this.CaculateOrbitsBacktoCOM(aPlanet);
            }
            return Count;
        }

        private int CaculateOrbitsBacktoCOM(Planet planet)
        {
            int count = 0;
            Planet currentPlanet = planet;
            while(currentPlanet.OrbitOf != null)
            {
                count++;
                currentPlanet = currentPlanet.OrbitOf;
            }

            return count;
        }
    }

    public class Planet
    {
        public string PlanetName { get; set; }

        private Planet _OrbitOf;
        /// <summary>
        /// The planet this planet orbits (null if does not orbit one, 
        /// which would be the universal Center of Mass (COM))
        /// </summary>
        public Planet OrbitOf
        {
            get { return _OrbitOf; }
            set { _OrbitOf = value; }
        }

        private List<Planet> _ListOfPlanetsThatOrbitThisPlanet = new List<Planet>();
        public List<Planet> PlanetsThatOrbitThisPlanet
        {
            get
            {
                return _ListOfPlanetsThatOrbitThisPlanet;
            }
        }



    }
}

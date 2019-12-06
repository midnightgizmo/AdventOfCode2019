using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Day6.PartTwo
{
    public class PuzzleTwo
    {
        private string _fileData;
        private List<string[]> _parsedFileData;

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
            Planet YOU = null, SAN = null;
            List<Planet> ListOfAllplanets = new List<Planet>();

            UniversalCenterOfMass.PlanetName = "COM";

            foreach (string[] orbitData in this._parsedFileData)
            {
                string planetName = orbitData[1];
                string planetNameItOrbits = orbitData[0];

                Planet PlanetThatOrbits = ListOfAllplanets.Find((s) => s.PlanetName == planetName);
                if (PlanetThatOrbits == null)
                {
                    PlanetThatOrbits = new Planet();
                    PlanetThatOrbits.PlanetName = planetName;

                    ListOfAllplanets.Add(PlanetThatOrbits);
                }
                if (PlanetThatOrbits.PlanetName == "YOU")
                    YOU = PlanetThatOrbits;
                if (PlanetThatOrbits.PlanetName == "SAN")
                    SAN = PlanetThatOrbits;

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

            if (YOU != null && SAN != null)
                return CaculateHopsBetweenPlanets(YOU, SAN);
            else
                return -1;
        }

        private int CaculateOrbitsBacktoCOM(Planet planet)
        {
            int count = 0;
            Planet currentPlanet = planet;
            while (currentPlanet.OrbitOf != null)
            {
                count++;
                currentPlanet = currentPlanet.OrbitOf;
            }

            return count;
        }

        private int CaculateHopsBetweenPlanets(Planet p1, Planet p2)
        {
            bool HaveFoundMatchingOrbit = false;

            Planet p1ParentOrbits = p1.OrbitOf;
            int hopsCount = 0;
            while(HaveFoundMatchingOrbit == false && p1ParentOrbits != null)
            {
                int numberOfSteps;
                
                if (this.IsPartOfPlanetsOrbit(p1ParentOrbits, p2, out numberOfSteps) == true)
                    return hopsCount + numberOfSteps;

                hopsCount++;
                p1ParentOrbits = p1ParentOrbits.OrbitOf;
            }
            return 0;
        }

        private bool IsPartOfPlanetsOrbit(Planet PossibleParentPlanet, Planet possibleChildPlanet, out int numberStepsFromParentToChildPlanet)
        {
            int hops = 0;
            numberStepsFromParentToChildPlanet = -1;
            
            Planet currentPlanet = possibleChildPlanet.OrbitOf;
            while (currentPlanet != null)
            {
                hops++;
                if (currentPlanet.PlanetName == PossibleParentPlanet.PlanetName)
                {
                    numberStepsFromParentToChildPlanet = hops - 1;
                    return true;
                }

                currentPlanet = currentPlanet.OrbitOf;


            }

            return false;
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

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Day8
{
    public class PuzzleOne
    {
        private string _fileData;
        private List<Layer> LayersList;

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
            char[] PixelData; 
            //List<char[]> layers = new List<char[]>();
            LayersList = new List<Layer>();

            int LayerWidth = 25;
            int LayerHeight = 6;
            int NumberOfPixelsInALayer = LayerWidth * LayerHeight;



            PixelData = this._fileData.ToCharArray();

            int startIndex = 0;
            while( (startIndex + NumberOfPixelsInALayer) <= PixelData.Length)
            {
                char[] aLayer= new char[NumberOfPixelsInALayer];
                Array.Copy(PixelData, startIndex, aLayer, 0, NumberOfPixelsInALayer);

                //layers.Add(aLayer);
                LayersList.Add(Layer.ConvertToLayer(aLayer,LayerWidth));

                startIndex += NumberOfPixelsInALayer;
            }
            



            return this;
        }

        public int CheckImage()
        {
            var orderdByFewestZeros = this.LayersList.OrderBy((s) => s.TotalZeroDigits);

            Layer layerWithFewestZeros =  orderdByFewestZeros.First();

            return layerWithFewestZeros.TotalOneDigits * layerWithFewestZeros.TotalTwoDigits;
        }
    }


    public class Layer
    {
        public List<LayerRow> LayRows = new List<LayerRow>();

        public int TotalZeroDigits;
        public int TotalOneDigits;
        public int TotalTwoDigits;

        public static Layer ConvertToLayer(char[] layerData, int rowLength)
        {
            Layer aLayer = new Layer();

            int startIndex = 0;
            while( (startIndex + rowLength) <= layerData.Length )
            {
                char[] aRow = new char[rowLength];
                Array.Copy(layerData, startIndex, aRow, 0, rowLength);
                LayerRow aConvertedRow = LayerRow.ConvertToRow(aRow);
                aLayer.LayRows.Add(aConvertedRow);

                aLayer.TotalZeroDigits += aConvertedRow.TotalZeroDigits;
                aLayer.TotalOneDigits += aConvertedRow.TotalOneDigits;
                aLayer.TotalTwoDigits += aConvertedRow.TotalTwoDigits;

                startIndex += rowLength;
            }

            return aLayer;
        }
    }

    public class LayerRow
    {
        public int[] Row;

        public int TotalZeroDigits;
        public int TotalOneDigits;
        public int TotalTwoDigits;
        public static LayerRow ConvertToRow(char[] layerRowData)
        {
            LayerRow aRow = new LayerRow();

            aRow.Row = new int[layerRowData.Length];
           
            for(int pixelIndex = 0; pixelIndex < layerRowData.Length; pixelIndex++)
            {
                char aPixel = layerRowData[pixelIndex];
                aRow.Row[pixelIndex] = int.Parse(aPixel.ToString());
                // keep a count of the number of 0, 1, 2 digits on this row.
                switch(aRow.Row[pixelIndex])
                {
                    case 0:
                        aRow.TotalZeroDigits++;
                        break;

                    case 1:
                        aRow.TotalOneDigits++;

                        break;
                    case 2:
                        aRow.TotalTwoDigits++;
                        break;

                }
            }

            return aRow;
        }
    }
}

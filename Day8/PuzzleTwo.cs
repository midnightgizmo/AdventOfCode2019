using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Day8.PartTwo
{
    public class PuzzleTwo
    {
        private string _fileData;
        private List<Layer> LayersList;

        int LayerWidth = 25;
        int LayerHeight = 6;

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
            char[] PixelData;
            //List<char[]> layers = new List<char[]>();
            LayersList = new List<Layer>();

            
            int NumberOfPixelsInALayer = LayerWidth * LayerHeight;



            PixelData = this._fileData.ToCharArray();

            int startIndex = 0;
            while ((startIndex + NumberOfPixelsInALayer) <= PixelData.Length)
            {
                char[] aLayer = new char[NumberOfPixelsInALayer];
                Array.Copy(PixelData, startIndex, aLayer, 0, NumberOfPixelsInALayer);

                //layers.Add(aLayer);
                LayersList.Add(Layer.ConvertToLayer(aLayer, LayerWidth));

                startIndex += NumberOfPixelsInALayer;
            }




            return this;
        }

        public void CreateImage()
        {
            // create the canvas
            Bitmap messageImage = new Bitmap(this.LayerWidth,this.LayerHeight);
            // create the thing that draws on the canvas
            Graphics graphics = Graphics.FromImage(messageImage);
            // create the colors.
            Brush zeroBrush = Brushes.Black;
            Brush oneBrush = Brushes.White;
            Brush TwoBrush = Brushes.Transparent;
            //Color zeroBrush = Color.Pink;
            //Color oneBrush = Color.White;
            //Color TwoBrush = Color.Transparent;

            // go through each layer, from back to front
            for (int eachLayerIndex = (LayersList.Count-1); eachLayerIndex >= 0; eachLayerIndex--)
            {
                // get the current layer
                Layer eachLayer = LayersList[eachLayerIndex];

                // go through each row for this layer
                for(int eachRowIndex = 0; eachRowIndex < eachLayer.LayRows.Count; eachRowIndex++)
                {
                    // grab the row
                    LayerRow aRow = eachLayer.LayRows[eachRowIndex];

                    // go through each pixel for this row
                    for (int eachPixelIndex = 0; eachPixelIndex < aRow.Row.Count(); eachPixelIndex++ )
                    {
                        int pixel = aRow.Row[eachPixelIndex];
                        // should be eaither 0,1 or 2
                        switch(pixel)
                        {
                            case 0:
                                graphics.FillRectangle(zeroBrush, eachPixelIndex, eachRowIndex, 1, 1);
                                //messageImage.SetPixel(eachPixelIndex, eachRowIndex, zeroBrush);
                                break;

                            case 1:
                                graphics.FillRectangle(oneBrush, eachPixelIndex, eachRowIndex, 1, 1);
                                //messageImage.SetPixel(eachPixelIndex, eachRowIndex, oneBrush);
                                break;

                            case 2:
                                graphics.FillRectangle(TwoBrush, eachPixelIndex, eachRowIndex, 1, 1);
                                //messageImage.SetPixel(eachPixelIndex, eachRowIndex, TwoBrush);
                                break;
                        }
                    }
                }
            }

            messageImage.Save(System.IO.Directory.GetCurrentDirectory() + "\\image.bmp");
            messageImage.Dispose();


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
            while ((startIndex + rowLength) <= layerData.Length)
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

            for (int pixelIndex = 0; pixelIndex < layerRowData.Length; pixelIndex++)
            {
                char aPixel = layerRowData[pixelIndex];
                aRow.Row[pixelIndex] = int.Parse(aPixel.ToString());
                // keep a count of the number of 0, 1, 2 digits on this row.
                switch (aRow.Row[pixelIndex])
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

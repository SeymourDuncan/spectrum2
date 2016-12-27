using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMLoader.Model
{
    public class FileParser: IDisposable
    {
        public FileParser()
        {
            _invertedDecimalSeparator = DecimalSeparator == "." ? "," : ".";
        }

        public List<double[]> RowDoubleValues { get; set; } = new List<double[]>();

        static readonly string CsvExtension = ".csv";
        static readonly string XlsxExtension = ".xlsx";
        static readonly string DecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        readonly string _invertedDecimalSeparator;

        string _fileName;
        public bool ParseFile(string fileName)
        {
            RowDoubleValues.Clear();
            _fileName = fileName;
            if (!File.Exists(fileName))
                return false;

            var extens = Path.GetExtension(fileName);
            if (extens.Equals(CsvExtension, StringComparison.CurrentCultureIgnoreCase))
            {
                return ParseCsv();
            }
            else if (extens.Equals(XlsxExtension, StringComparison.CurrentCultureIgnoreCase))
            {
                return ParseXlsx();
            }

            return false;
        }

        private bool ParseCsv()
        {
            using (var reader = new StreamReader(File.OpenRead(_fileName)))
            {                               
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line?.Split(';');
                    if (values?.Length < 2)
                        continue;

                    double[] doubles;
                    bool valid = TryConvertToDoubleArr(values, out doubles);
                    if (valid)
                        RowDoubleValues.Add(doubles);
                }
            }            
            return true;
        }

        bool TryConvertToDoubleArr(string[] arr, out double[] doubles)
        {
            // конвертим. Нас интересует чтобы 1-ые 2 значения были дабл, на остальное пока пох.
            doubles = new double[2];
            try
            {
                double dval;
                ConvertToDouble(arr[0], out dval);                                    
                doubles[0] = dval;
                ConvertToDouble(arr[1], out dval);
                doubles[1] = dval;
            }
            catch
            {
                return false;
            }
            return true;
        }


        void ConvertToDouble(string strval, out double dval)
        {            
            bool res = double.TryParse(strval, out dval);
            if (!res)
            {
                // пробуем поменять
                strval = strval.Replace(_invertedDecimalSeparator, DecimalSeparator);
                dval = double.Parse(strval);
            }           
        }

        private bool ParseXlsx()
        {
            return true;
        }

        public void Dispose()
        {
            RowDoubleValues.Clear();
        }
    }

}

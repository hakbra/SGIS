using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SGIS
{

    class DBFReader
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct DBFHeader
        {
            public byte version;
            public byte updateYear;
            public byte updateMonth;
            public byte updateDay;
            public Int32 numRecords;
            public Int16 headerLen;
            public Int16 recordLen;
            public Int16 reserved1;
            public byte incompleteTrans;
            public byte encryptionFlag;
            public Int32 reserved2;
            public Int64 reserved3;
            public byte MDX;
            public byte language;
            public Int16 reserved4;
        }

        // This is the field descriptor structure. 
        // There will be one of these for each column in the table.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct FieldDescriptor
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
            public string fieldName;
            public char fieldType;
            public Int32 address;
            public byte fieldLen;
            public byte count;
            public Int16 reserved1;
            public byte workArea;
            public Int16 reserved2;
            public byte flag;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] reserved3;
            public byte indexFlag;
        }
        public static DataTable read(string filename)
        {
            DataTable dt = new DataTable();

            // Read the header into a buffer
            BinaryReader br = new BinaryReader(File.OpenRead(filename));
            byte[] buffer = br.ReadBytes(Marshal.SizeOf(typeof(DBFHeader)));

            // Marshall the header into a DBFHeader structure
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            DBFHeader header = (DBFHeader)Marshal.PtrToStructure(
                                handle.AddrOfPinnedObject(), typeof(DBFHeader));
            handle.Free();


            // Read in all the field descriptors. 
            // Per the spec, 13 (0D) marks the end of the field descriptors
            ArrayList fields = new ArrayList();
            while ((13 != br.PeekChar()))
            {
                buffer = br.ReadBytes(Marshal.SizeOf(typeof(FieldDescriptor)));
                handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                fields.Add((FieldDescriptor)Marshal.PtrToStructure(
                            handle.AddrOfPinnedObject(), typeof(FieldDescriptor)));
                handle.Free();
            }

            char endchar = br.ReadChar();

            dt.Columns.Add(new DataColumn("sgis_id", typeof(Int32)));
            DataColumn col = null;
            foreach (FieldDescriptor field in fields)
            {
                switch (field.fieldType)
                {
                    case 'N':
                        if (field.count == 0)
                            col = new DataColumn(field.fieldName, typeof(Int32));
                        else
                            col = new DataColumn(field.fieldName, typeof(double));
                        break;
                    case 'C':
                        col = new DataColumn(field.fieldName, typeof(string));
                        break;
                    case 'D':
                        col = new DataColumn(field.fieldName, typeof(DateTime));
                        break;
                    case 'L':
                        col = new DataColumn(field.fieldName, typeof(bool));
                        break;
                    case 'F':
                        col = new DataColumn(field.fieldName, typeof(float));
                        break;
                    default:
                        throw new Exception("Unknown fieldType");
                }
                dt.Columns.Add(col);
            }

            // Read in all the records
            for (int counter = 0; counter <= header.numRecords - 1; counter++)
            {
                // First we'll read the entire record into a buffer and then read each 
                // field from the buffer. This helps account for any extra space at the 
                // end of each record and probably performs better.
                buffer = br.ReadBytes(header.recordLen);
                BinaryReader recReader = new BinaryReader(new MemoryStream(buffer));


                char delchar = recReader.ReadChar();
                if (delchar != ' ') // if '*' then record is deleted
                    continue;

                // Loop through each field in a record
                DataRow row = dt.NewRow();
                row["sgis_id"] = counter+1;
                foreach (FieldDescriptor field in fields)
                {
                    int fieldLen = field.fieldLen;
                    switch (field.fieldType)
                    {
                        case 'D': // Date (YYYYMMDD)
                            string year = Encoding.ASCII.GetString(recReader.ReadBytes(4));
                            string month = Encoding.ASCII.GetString(recReader.ReadBytes(2));
                            string day = Encoding.ASCII.GetString(recReader.ReadBytes(2));
                            row[field.fieldName] = System.DBNull.Value;
                            try
                            {
                                if ((Int32.Parse(year) > 1900))
                                {
                                    row[field.fieldName] = new DateTime(Int32.Parse(year), 
                                                               Int32.Parse(month), Int32.Parse(day));
                                }
                            }
                            catch {}
                            break;
                        case 'N':
                            string strnum = Encoding.ASCII.GetString(recReader.ReadBytes(fieldLen));
                            try
                            {
                                if (field.count == 0)
                                {
                                    int num = Int32.Parse(strnum);
                                    row[field.fieldName] = num;
                                }
                                else
                                {
                                    double num = double.Parse(strnum, CultureInfo.InvariantCulture);
                                    row[field.fieldName] = num;
                                }
                            }
                            catch (Exception e){
                                string error = e.Message;
                                Console.WriteLine(error);
                            }
                            break;
                        case 'F':
                            string strfloat = Encoding.ASCII.GetString(recReader.ReadBytes(fieldLen));
                            try
                            {
                                float num = float.Parse(strfloat);
                                row[field.fieldName] = num;
                            }
                            catch (Exception e)
                            {
                                string error = e.Message;
                                Console.WriteLine(error);
                            }
                            break;
                        case 'C':
                            string data = Encoding.ASCII.GetString(recReader.ReadBytes(fieldLen));
                            row[field.fieldName] = data;
                            break;
                        case 'L':
                            char boolchar = recReader.ReadChar();
                            row[field.fieldName] = ("1tTyY".IndexOf(boolchar) > -1) ? true : false;
                            break;
                        default:
                            throw new Exception("Unknown fieldType");
                    }
                }

                recReader.Close();
                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}

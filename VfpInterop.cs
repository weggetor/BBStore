using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Bitboxx.DNNModules.BBStore
{
    public static class VfpInterop
    {
        public static Hashtable Xml2Obj(string tcXml)
        {
            int lnCnt = 1;
            Hashtable loReturn = new Hashtable();
            for (int lnLoopColl = 1; lnLoopColl < lnCnt+1; lnLoopColl++)
            {
                // Wir brauchen alles zwischen öffnendem und dazugehörigem schließenden Paramobject
                int lnAtPosStart = At("<paramobject>", tcXml, lnLoopColl);
                int lnAtPosEnd = At("</paramobject>", tcXml, lnLoopColl);
                string lcXml = StrExtract(tcXml, "<paramobject>", "</paramobject>", lnLoopColl, 1);
                
                // Haben wir noch öffnende '<paramobject>' im Text ?
                int lnAnzSubObj = Occurs("<paramobject>",lcXml);
                if (lnAnzSubObj > 0)
                    lnAtPosEnd = At("</paramobject>", tcXml, lnLoopColl + lnAnzSubObj);

                lcXml = tcXml.Substring(lnAtPosStart + 12, lnAtPosEnd - lnAtPosStart - 13);
                while (lcXml != String.Empty)
                {
                    string lcTerm = StrExtract(lcXml, "<", ">",1,0);
                    string lcProperty = GetWordNumb(lcTerm, 1," ");
                    string lcType = StrExtract(GetWordNumb(lcTerm, 2," "), "type=\"", "\"",1,1);
                    string lcValue = StrExtract(lcXml, "<" + lcProperty + " type=\"" + lcType + "\">", "</" + lcProperty + ">", 1, 1);
                    string lcDimen = "";
                    if (lcType.StartsWith("Array"))
                    {
                        lcDimen = lcType.Substring(6);
                        lcType = "Array";
                    }

                    switch (lcType)
                    {
                        case "String":
                            loReturn.Add(lcProperty, lcValue);
                            break;
                        case "Integer":
                            loReturn.Add(lcProperty, Int32.Parse(lcValue));
                            break;
                        case "Datetime":
                        case "Date":
                            loReturn.Add(lcProperty,DateTime.Parse(lcValue,System.Globalization.CultureInfo.CurrentUICulture));
                            break;
                        case "Numeric":
                            loReturn.Add(lcProperty,Decimal.Parse(lcValue,System.Globalization.CultureInfo.CurrentUICulture));
                            break;
                        case "Boolean":
                            loReturn.Add(lcProperty,(lcValue == ".T."? true: false));
                            break;
                        case "Array":
                            int lnDim1 = Convert.ToInt32(GetWordNumb(lcDimen,1,"_"));
                            int lnDim2 = Convert.ToInt32(GetWordNumb(lcDimen,2,"_"));
                            string lcParamObject = "<paramobject>" + lcValue + "</paramobject>";
                            Hashtable loObj = Xml2Obj(lcParamObject);
                            if (lnDim2 == 0)
                            {
                                Object[] myArr = new Object[lnDim1];
                                for (int lnLoop2 = 1; lnLoop2 <= lnDim1; lnLoop2++)
			                    {
                                    myArr[lnLoop2-1] = loObj["element_" + lnLoop2.ToString() + "_0"];
			                    }
                                loReturn.Add(lcProperty, myArr);
                            }
                            else
                            {
                                // Zweidimensionales Array
                                Object[,] myArr = new Object[lnDim1,lnDim2];
                                for (int lnLoop2 = 1; lnLoop2 <= lnDim1; lnLoop2++)
			                    {
                                    for (int lnLoop3 = 1; lnLoop3 <= lnDim2 ; lnLoop3++)
			                        {
                                        myArr[lnLoop2-1, lnLoop3-1] = loObj["element_" + lnLoop2.ToString() + "_" + lnLoop3.ToString()];
                                    }
			                    }
                                loReturn.Add(lcProperty, myArr);
                            }
                            break;
                        default:
                            loReturn.Add(lcProperty, null);
                            break;
                    }
                    string tag = "</"+lcProperty + ">";
                    int lnAtPos = At(tag, lcXml,1) + tag.Length -1;
                    lcXml = lcXml.Substring(lnAtPos);
                }
            }

            return loReturn;
        }
        public static string Obj2Xml(Hashtable toObj)
        {
            int lnCnt = 1;
            StringBuilder sb = new StringBuilder();
            for (int lnLoopColl = 1; lnLoopColl < lnCnt + 1; lnLoopColl++)
            {
                Hashtable loObj = toObj;
                sb.Append("<paramobject>");
                foreach (DictionaryEntry item in loObj)
                {
                    string lcProperty = (string)item.Key;
                    object luValue = item.Value;
                    string lcType = luValue.GetType().Name;
                    string lcValue = luValue.ToString();
                    string lcFullType = "";
                    switch (lcType)
                    {
                        case "String":
                            lcFullType = "String";
                            break;
                        case "Int32":
                            lcFullType = "Integer";
                            break;
                        case "DateTime":
                            DateTime tuValue = (DateTime) luValue;
                            if (tuValue == tuValue.Date)
                            {
                                lcFullType = "Date";
                                lcValue = lcValue.Substring(0, 10);
                            }
                            else
                                lcFullType = "Datetime";
                            break;
                        case "Boolean":
                            lcFullType = "Boolean";
                            lcValue = ((Boolean)luValue ? ".T.":".F.");
                            break;
                        case "Decimal":
                        case "Double":
                            lcFullType = "Numeric";
                            break;
                        case "Object[,]":
                            Object[,] myArr2 = (Object[,])luValue;
                            int lnDim1 = myArr2.GetLength(0);
                            int lnDim2 = myArr2.GetLength(1);
                            lcFullType = "Array_" + lnDim1.ToString() + "_" + lnDim2.ToString();
                            Hashtable temp2 = new Hashtable();
                            for (int i = 0; i < lnDim1; i++)
                            {
                                for (int j = 0; j < lnDim2; j++)
                                {
                                    temp2.Add("element_" + (i + 1).ToString() + "_" + (j + 1).ToString(), myArr2[i, j]);
                                }
                            }
                            lcValue = Obj2Xml(temp2);
                            lcValue = StrExtract(lcValue, "<paramobject>", "</paramobject>", 1, 1);
                            break;
                        case "Object[]":
                            Object[] myArr1 = (Object[])luValue;
                            int lnDim = myArr1.GetLength(0);

                            lcFullType = "Array_" + lnDim.ToString() + "_0";
                            Hashtable temp1 = new Hashtable();
                            for (int i = 0; i < lnDim; i++)
                            {
                                temp1.Add("element_" + (i + 1).ToString() + "_0", myArr1[i]);
                            }
                            lcValue = Obj2Xml(temp1);
                            lcValue = StrExtract(lcValue, "<paramobject>", "</paramobject>", 1, 1);
                            break;
                        default:
                            break;
                    }
                    sb.Append("<" + lcProperty + " type=\"" + lcFullType + "\">");
                    sb.Append(lcValue);
                    sb.Append("</" + lcProperty + ">");
                    
                }
                sb.Append("</paramobject>");
            }
            return sb.ToString();
        }

        #region Global Methods
        public static string SerializeToBase64String(object obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, obj);
            long length = memoryStream.Length;
            byte[] bytes = memoryStream.GetBuffer();

            string infoData = Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None);

            string encodedData = infoData;
            return encodedData;
        }

        public static object DeserializeFromBase64String(string content)
        {
            byte[] memoryData = Convert.FromBase64String(content);
            int length = memoryData.Length;

            MemoryStream memoryStream = new MemoryStream(memoryData, 0, length);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            object obj = binaryFormatter.Deserialize(memoryStream);

            return obj;
        }
        #endregion

        #region VFP-Helper-Klassen
        /// <summary>
        /// Receives a string along with starting and ending delimiters and returns the 
        /// part of the string between the delimiters. Receives a beginning occurence
        /// to begin the extraction from and also receives a flag (0/1) where 1 indicates
        /// that the search should be case insensitive.
        /// <pre>
        /// Example:
        /// string cExpression = "JoeDoeJoeDoe";
        /// VFPToolkit.strings.StrExtract(cExpression, "o", "eJ", 1, 0);		//returns "eDo"
        /// </pre>
        /// </summary>
        public static string StrExtract(string cSearchExpression, string cBeginDelim, string cEndDelim, int nBeginOccurence, int nFlags)
        {
            string cstring = cSearchExpression;
            string cb = cBeginDelim;
            string ce = cEndDelim;
            string lcRetVal = "";

            //Check for case-sensitive or insensitive search
            if (nFlags == 1)
            {
                cstring = cstring.ToLower();
                cb = cb.ToLower();
                ce = ce.ToLower();
            }

            //Lookup the position in the string
            int nbpos = At(cb, cstring, nBeginOccurence) + cb.Length - 1;
            int nepos = cstring.IndexOf(ce, nbpos + 1);

            //Extract the part of the strign if we get it right
            if (nepos > nbpos)
            {
                lcRetVal = cSearchExpression.Substring(nbpos, nepos - nbpos);
            }

            return lcRetVal;
        }
        public static int At(string cSearchFor, string cSearchIn, int nOccurence)
        {
            return _at(cSearchFor, cSearchIn, nOccurence, 1);
        }

        /// Private Implementation: This is the actual implementation of the At() and RAt() functions. 
        /// Receives two strings, the expression in which search is performed and the expression to search for. 
        /// Also receives an occurence position and the mode (1 or 0) that specifies whether it is a search
        /// from Left to Right (for At() function)  or from Right to Left (for RAt() function)
        private static int _at(string cSearchFor, string cSearchIn, int nOccurence, int nMode)
        {
            //In this case we actually have to locate the occurence
            int i = 0;
            int nOccured = 0;
            int nPos = 0;
            if (nMode == 1) { nPos = 0; }
            else { nPos = cSearchIn.Length; }

            //Loop through the string and get the position of the requiref occurence
            for (i = 1; i <= nOccurence; i++)
            {
                if (nMode == 1) { nPos = cSearchIn.IndexOf(cSearchFor, nPos); }
                else { nPos = cSearchIn.LastIndexOf(cSearchFor, nPos); }

                if (nPos < 0)
                {
                    //This means that we did not find the item
                    break;
                }
                else
                {
                    //Increment the occured counter based on the current mode we are in
                    nOccured++;

                    //Check if this is the occurence we are looking for
                    if (nOccured == nOccurence)
                    {
                        return nPos + 1;
                    }
                    else
                    {
                        if (nMode == 1) { nPos++; }
                        else { nPos--; }

                    }
                }
            }
            //We never found our guy if we reached here
            return 0;
        }


        /// <summary>
        /// Receives two strings as parameters and searches for one string within another. 
        /// This function ignores the case and if found, returns the beginning numeric position 
        /// otherwise returns 0
        /// <pre>
        /// Example:
        /// VFPToolkit.strings.AtC("d", "Joe Doe");	//returns 5
        /// </pre>
        /// </summary>
        /// <param name="cSearchFor"> </param>
        /// <param name="cSearchIn"> </param>
        public static int AtC(string cSearchFor, string cSearchIn)
        {
            return cSearchIn.ToLower().IndexOf(cSearchFor.ToLower()) + 1;
        }

        /// <summary>
        /// Returns the number of occurences of a character within a string
        /// <pre>
        /// Example:
        /// VFPToolkit.strings.Occurs('o', "Joe Doe");		//returns 2
        /// 
        /// Tip: If we have a string say lcString, then lcString[3] gives us the 3rd character in the string
        /// </pre>
        /// </summary>
        /// <param name="tcChar"> </param>
        /// <param name="cExpression"> </param>
        public static int Occurs(char tcChar, string cExpression)
        {
            int i, nOccured = 0;

            //Loop through the string
            for (i = 0; i < cExpression.Length; i++)
            {
                //Check if each expression is equal to the one we want to check against
                if (cExpression[i] == tcChar)
                {
                    //if  so increment the counter
                    nOccured++;
                }
            }
            return nOccured;
        }
        /// <summary>
        /// Returns the number of occurences of one string within another string
        /// <pre>
        /// Example:
        /// VFPToolkit.strings.Occurs("oe", "Joe Doe");		//returns 2
        /// VFPToolkit.strings.Occurs("Joe", "Joe Doe");	//returns 1
        /// 
        /// Tip: String.IndexOf() searches the string (starting from left) for another character or string expression
        /// </pre>
        /// </summary>
        /// <param name="cString"> </param>
        /// <param name="cExpression"> </param>
        public static int Occurs(string cString, string cExpression)
        {
            int nPos = 0;
            int nOccured = 0;
            do
            {
                //Look for the search string in the expression
                nPos = cExpression.IndexOf(cString, nPos);

                if (nPos < 0)
                {
                    //This means that we did not find the item
                    break;
                }
                else
                {
                    //Increment the occured counter based on the current mode we are in
                    nOccured++;
                    nPos++;
                }
            } while (true);

            //Return the number of occurences
            return nOccured;
        }
        /// <summary>
        /// Receives a string as a parameter and counts the number of words in that string
        /// <pre>
        /// Example:
        /// string lcString = "Joe Doe is a good man";
        /// VFPToolkit.strings.GetWordCount(lcString);		//returns 6
        /// </pre>
        /// </summary>
        /// <param name="cString"> </param>
        public static long GetWordCount(string cString)
        {
            int i = 0;
            long nLength = cString.Length;
            long nWordCount = 0;

            //Begin by checking for the first word
            if (!Char.IsWhiteSpace(cString[0]))
            {
                nWordCount++;
            }

            //Now look for white spaces and count each word
            for (i = 0; i < nLength; i++)
            {
                //Check for a space to begin counting a word
                if (Char.IsWhiteSpace(cString[i]))
                {
                    //We think we encountered a word
                    //Remove any following white spaces if any after this word
                    do
                    {
                        //Check if we have reached the limit and if so then exit the loop
                        i++;
                        if (i >= nLength) { break; }
                        if (!Char.IsWhiteSpace(cString[i]))
                        {
                            nWordCount++;
                            break;
                        }
                    } while (true);

                }

            }
            return nWordCount;
        }

        /// <summary>
        /// Based on the position specified, returns a word from a string 
        /// Receives a string as a parameter and counts the number of words in that string
        /// <pre>
        /// Example:
        /// string lcString = "Joe Doe is a good man";
        /// VFPToolkit.strings.GetWordNumber(lcString, 5);		//returns "good"
        /// </pre>
        /// </summary>
        /// <param name="cString"> </param>
        /// <param name="nWordPosition"> </param>
        public static string GetWordNumb(string cString, int nWordPosition, string cWhitespaceChar)
        {
            int nBeginPos = At(cWhitespaceChar, cString, nWordPosition - 1);
            int nEndPos = At(cWhitespaceChar, cString, nWordPosition);
            // first word ?
            if (nBeginPos == 0 && nEndPos > 0)
                return cString.Substring(0, nEndPos - 1);
            // last word ?
            else if (nEndPos == 0 && nBeginPos > 0)
                return cString.Substring(nBeginPos);
            else
                return cString.Substring(nBeginPos + 1, nEndPos - 1 - nBeginPos);
        }
        #endregion
    }
}

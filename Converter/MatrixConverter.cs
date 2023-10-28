using System;
using System.Collections.Generic;

using SMath.Manager;

namespace MaximaPlugin.Converter
{
   /// <summary>
   /// Store the information of the input element
   /// </summary>
    public class ElementStore
    {
        public int rows = 0;
        public int cols = 0;
        public int items = 0;
        public int refPointer = 0;
        public int bracketOpen = 0;
        public int bracketClose = 0;
        public int squareBracketOpen = 0;
        public int squareBracketClose = 0;
        public int charCounter = 0;
        public List<ElementStore> refs = new List<ElementStore>();
        //first list : List entry for any item; second list: entry for data in item e.g.(a,f(layerMsg),b) 
        public List<List<string>> itemData = new List<List<string>>();
    }

    /// <summary>
    /// Manage the storing of the information of the input element. 
    /// Used in multiple other classes to record and convert the input to SMath and Maxima syntax
    /// </summary>
    public class ElementStoreManager
    {
        public string layerMsg = "!NEW!+!ELEMENT!";
        ElementStore nextStore;
        public ElementStore currentStore;
        public bool arrayIn = false;

		/// <summary>
        /// Determine cols and rows
        /// </summary>
        public void terminate ()
		{
			if (currentStore.rows == 0 && currentStore.cols > 0 && currentStore.items > 0)
				currentStore.rows = currentStore.items / currentStore.cols;
			else if (currentStore.cols == 0 && currentStore.rows > 0 && currentStore.items > 0)
				currentStore.cols = currentStore.items / currentStore.rows;
		}

        /// <summary>
        /// Creates new entry
        /// </summary>
        public void newElementStore()
        {
            if (currentStore == null)
            {
                currentStore = new ElementStore();
                currentStore.refs.Add(null);
                arrayIn = true;
            }
            else
            {
                nextStore = new ElementStore();
                nextStore.refs.Add(currentStore);
                currentStore.refs.Add(nextStore);
                currentStore.itemData[currentStore.itemData.Count - 1].Add(layerMsg);
                currentStore = nextStore;
            }
            addNewItemToCurrent();
        }

        /// <summary>
        /// Go back to previous entry
        /// </summary>
        /// <returns></returns>
        public bool prevElementStore()
        {
            if (currentStore.refs[0] != null)
            {
                currentStore.refPointer = 0;
                currentStore = currentStore.refs[0];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Go to the next entry
        /// </summary>
        public void nextElementStore()
        {
            currentStore.refPointer++;
            if (currentStore.refs.Count > currentStore.refPointer)
                currentStore = currentStore.refs[currentStore.refPointer];
        }

        /// <summary>
        /// Go to the first entry
        /// </summary>
        public void gotoFirstElementStore()
        {
            if (arrayIn)
            {
                while (currentStore.refs[0] != null)
                {
                    currentStore = currentStore.refs[0];
                }

            }
        }

        /// <summary>
        /// Add new item to the current entry
        /// </summary>
        public void addNewItemToCurrent()
        {
            currentStore.itemData.Add(new List<string>());
            currentStore.items++;
        }

        /// <summary>
        /// Add data to last element of itemData list
        /// </summary>
        /// <param name="data"></param>
        public void addItemDataToCurrent(string data)
        {
            currentStore.itemData[currentStore.itemData.Count - 1].Add(data);
        }

        /// <summary>
        /// Increase the counters by the given increments
        /// </summary>
        /// <param name="deltaRows"></param>
        /// <param name="deltaCols"></param>
        /// <param name="deltaItems"></param>
        public void increaseCounter(int deltaRows, int deltaCols, int deltaItems)
        {
            currentStore.rows = currentStore.rows + deltaRows;
            currentStore.cols = currentStore.cols + deltaCols;
            currentStore.items = currentStore.items + deltaItems;
        }

        /// <summary>
        /// Flip the row and column
        /// </summary>
        public void flipRowsCols()
        {
            int tmp = currentStore.rows;
            currentStore.rows = currentStore.cols;
            currentStore.cols = tmp;
        }
    }

    /// <summary>
    /// Handles Matrix and List conversion from SMath to Maxima
    /// </summary>
    public static class MatrixAndListFromSMathToMaxima
    {
		public static int arrayStart = 0, arrayEnd = 0;

        /// <summary>
        /// Replaces the part arrayStart...arrayEnd in input by newArrayString 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="newArrayString"></param>
        /// <param name="arrayStart"></param>
        /// <param name="arrayEnd"></param>
        /// <returns></returns>
        public static string MakeString(string input, string newArrayString, int arrayStart, int arrayEnd)
        {
            string tmp1 = input.Substring(0, arrayStart);
            string tmp2 = input.Substring(arrayEnd, input.Length - arrayEnd);
            return tmp1 + newArrayString + tmp2;
        }

        /// <summary>
        /// Writes the contents of store to a string between systemStart and systemEnd.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="systemStart"></param>
        /// <param name="systemEnd"></param>
        /// <returns></returns>
        public static string MakeTermString(ElementStoreManager store, string systemStart, string systemEnd)
        {
            string rowString = "[";
            string itemsString = "";

            for (int i = 0; i < store.currentStore.items; i++)
            {
                for (int j = 0; j < store.currentStore.itemData[i].Count; j++)
                {
                    if (store.currentStore.itemData[i][j] != store.layerMsg) 
                    {
                        itemsString = itemsString + store.currentStore.itemData[i][j]; 
                    }
                    else
                    {
                        store.nextElementStore();
                        itemsString = itemsString + MakeTermString(store, systemStart, systemEnd);
                        store.prevElementStore();
                    }
                }
                if ((i + 1) % store.currentStore.cols != 0) /// || (i == 0 && store.currentStore.items!=1)) 
                {
                    itemsString = itemsString + GlobalProfile.ArgumentsSeparatorStandard;
                }
                else // if ((i % store.currentStore.cols == 0 && i !=0 ) || store.currentStore.items==1 ) 
                {
                    rowString = rowString + itemsString + "]";
                    itemsString = "";

                    if (i < store.currentStore.items - 1) rowString = rowString + GlobalProfile.ArgumentsSeparatorStandard + "[";
                }
            }
            return systemStart + rowString + systemEnd;
        }

        /// <summary>
        /// Translates an SMath matrix given as string to Maxima
        /// Called by Converter.PrepareTermsForMaxima
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MatrixConvert (string input)
		{
			bool doAgain;
			do{
				doAgain=false;
				arrayStart = 0;
				arrayEnd = 0;
				ElementStoreManager store= new ElementStoreManager();
				store = SMathMatrixDataCollection (store, input);
				if(store.arrayIn)
				{
					input = MakeString(input, MakeTermString(store, "matrix(", ")"), arrayStart, arrayEnd);
					doAgain=true;
				}
			}while(doAgain);
            return input;      
		}

		/// <summary>
        /// Translates an SMath list given as string to Maxima
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ListConvert(string input)
        {
			bool doAgain;
			do{
				doAgain=false;
				arrayStart = 0;
				arrayEnd = 0;
				ElementStoreManager store= new ElementStoreManager();
				store = SMathListDataCollection (store, input);
				if(store.arrayIn)
				{
					input = MakeString(input, MakeTermString(store, "", ""), arrayStart, arrayEnd);
					doAgain=true;
				}
			}while(doAgain);
            return input;      
		}

        /// <summary>
        /// Converts an SMath matrix given as string to the internal storage structure
        /// </summary>
        /// <param name="store"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ElementStoreManager SMathMatrixDataCollection(ElementStoreManager store, string input)
        {
            bool inArray = true, foundArray = false;
            int i = 0;
            //Make structure
            while (i < input.Length && inArray)
            {
                if (i < (input.Length - 4) && input[i] == 'm' && input[i + 1] == 'a' && input[i + 2] == 't' && input[i + 3] == '(')
                { // new matrix start is found
                    if (foundArray && store.currentStore.charCounter > 0)
                    { // something left to store
                        store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                        store.currentStore.charCounter = 0;
                    }
                    if (!foundArray)
                    { // first occurance
                        foundArray = true;
                        arrayStart = i;
                    }
                    store.newElementStore(); 
                    store.currentStore.bracketOpen++; 
                    i += 3;
                }
                else if (input[i] == GlobalProfile.ArgumentsSeparatorStandard && foundArray)
                { // entry separator is found
                    if (store.currentStore.bracketOpen - store.currentStore.bracketClose == 1)
                    { // if current structure is open
                        if (store.currentStore.charCounter > 0)
                        { // something to add
                            store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                            store.currentStore.charCounter = 0;
                        }
                        store.addNewItemToCurrent(); // open new entry
                    }
                    else
                    {
                        store.currentStore.charCounter++;
                    }
                }
                else if (input[i] != GlobalProfile.ArgumentsSeparatorStandard && foundArray)
                { // no separator and we are in a matrix
                    if (input[i] == '(')
                    { // count opening brackets
                        store.currentStore.bracketOpen++;
                    }
                    if (input[i] == ')')
                    { // count closing brackets
                        store.currentStore.bracketClose++;
                    }
                    //LayerDown
                    if (store.currentStore.bracketOpen - store.currentStore.bracketClose == 0)
                    { // the current structure is closed

                        // GetData
                        {
                            store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                            store.currentStore.charCounter = 0;
                        }
                        // copy rows and cols from itemdata to integers
                        store.increaseCounter(Convert.ToInt32(store.currentStore.itemData[store.currentStore.itemData.Count - 2][0]), Convert.ToInt32(store.currentStore.itemData[store.currentStore.itemData.Count - 1][0]), -2);
                        // delete rows and cols from itemdata
                        store.currentStore.itemData.RemoveRange(store.currentStore.itemData.Count - 2, 2);
						store.terminate();
                        //Break if arrayend is reached
                        if (!store.prevElementStore())
                        {
                            inArray = false;
                            arrayEnd = i + 1;
                        }
                    }
                    else
                    {
                        store.currentStore.charCounter++;
                    }
                }
                i++;
            }
            store.gotoFirstElementStore();
            return store;
        }

        /// <summary>
        /// Converts an SMath list given as string to the internal storage structure
        /// </summary>
        /// <param name="store"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ElementStoreManager SMathListDataCollection(ElementStoreManager store, string input)
        {
            bool inArray = true, foundArray = false;
            int i = 0;
            //Make structure
            while (i < input.Length && inArray)
            {
                if (i < (input.Length - 3) && input[i] == 's' && input[i + 1] == 'y' && input[i + 2] == 's' && input[i + 3] == '(' && (i == 0 || i > 0 && !Char.IsLetter(input[i - 1])))
                {
                    if (foundArray && store.currentStore.charCounter > 0)
                    {
                        store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                        store.currentStore.charCounter = 0;
                    }
                    if (!foundArray)
                    {
                        foundArray = true;
                        arrayStart = i;
                    }
                    store.newElementStore();
                    store.currentStore.bracketOpen++;
                    i += 3;
                }
                else if (input[i] == GlobalProfile.ArgumentsSeparatorStandard && foundArray)
                {
                    if (store.currentStore.bracketOpen - store.currentStore.bracketClose == 1)
                    {
                        if (store.currentStore.charCounter > 0)
                        {
                            store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                            store.currentStore.charCounter = 0;
                        }
                        store.addNewItemToCurrent();
                    }
                    else
                    {
                        store.currentStore.charCounter++;
                    }
                }
                else if (input[i] != GlobalProfile.ArgumentsSeparatorStandard && foundArray)
                {
                    if (input[i] == '(')
                    {
                        store.currentStore.bracketOpen++;
                    }
                    if (input[i] == ')')
                    {
                        store.currentStore.bracketClose++;
                    }
                    //LayerDown
                    if (store.currentStore.bracketOpen - store.currentStore.bracketClose == 0)
                    {

                        // GetData
                        {
                            store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                            store.currentStore.charCounter = 0;
                        }
                        // copy rows and cols from itemdata to integers note: rows in Smath are cols in maxima
                        store.increaseCounter(Convert.ToInt32(store.currentStore.itemData[store.currentStore.itemData.Count - 1][0]), Convert.ToInt32(store.currentStore.itemData[store.currentStore.itemData.Count - 2][0]), -2);
                        // delte rows and cols from itemdata
                        store.currentStore.itemData.RemoveRange(store.currentStore.itemData.Count - 2, 2);
						store.terminate();

                        //Break if arrayend is reached
                        if (!store.prevElementStore())
                        {
                            inArray = false;
                            arrayEnd = i + 1;
                        }
                    }
                    else
                    {
                        store.currentStore.charCounter++;
                    }
                }
                i++;
            }
            store.gotoFirstElementStore();
            return store;
        }

    }

    /// <summary>
    /// Handles Matrix and List conversion from Maxima to SMath
    /// </summary>
    public static class MatrixAndListFromMaximaToSMath
	{
        public static bool solveFunction = false, firstCall=true;
		public static int arrayStart = 0, arrayEnd = 0;


        /// <summary>
        /// Replaces the part arrayStart...arrayEnd in input by newArrayString 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="newArrayString"></param>
        /// <param name="arrayStart"></param>
        /// <param name="arrayEnd"></param>
        /// <returns></returns>
        public static string MakeString(string input, string newArrayString, int arrayStart, int arrayEnd)
        {
            string tmp1 = input.Substring(0, arrayStart);
            string tmp2 = input.Substring(arrayEnd, input.Length - arrayEnd);
            return tmp1 + newArrayString + tmp2;
        }

        /// <summary>
        /// Writes the contents of store to a string between systemStart and systemEnd.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="systemStart"></param>
        /// <param name="systemEnd"></param>
        /// <returns></returns>
        public static string MakeTermString(ElementStoreManager store, string systemStart, string systemEnd)
        {
            string itemsString = "";
            string newSystemStart = "";

            //skip sys() with only one element
            if (solveFunction && systemStart == "sys(" && store.currentStore.items == 1 && store.currentStore.itemData.Count == 1 && store.currentStore.itemData[0][0] != store.layerMsg) return store.currentStore.itemData[0][0];
            if (solveFunction && systemStart == "sys(" && store.currentStore.items == 1 && store.currentStore.itemData.Count == 1 && store.currentStore.itemData[0][0] == store.layerMsg)
            {
                store.nextElementStore();
                return MakeTermString(store, systemStart, systemEnd);
            }



            if (solveFunction && firstCall && systemStart == "sys(" && store.currentStore.refs.Count > 1)
            {
                store.flipRowsCols();
                newSystemStart = "mat(";
                firstCall = false;
            }
            else
            {
                newSystemStart = systemStart;
            }




            for (int i = 0; i < store.currentStore.items; i++)
            {
                for (int j = 0; j < store.currentStore.itemData[i].Count; j++)
                {
                    if (store.currentStore.itemData[i][j] != store.layerMsg)
                    {
						itemsString = itemsString + store.currentStore.itemData[i][j];
                    }
                    else
                    {
                        store.nextElementStore();
                        itemsString = itemsString + MakeTermString(store, systemStart, systemEnd);
                        store.prevElementStore();
                    }
					if(j == store.currentStore.itemData[i].Count-1)
						itemsString = itemsString + GlobalProfile.ArgumentsSeparatorStandard;

                }
                if (i==store.currentStore.items-1) /// || (i == 0 && store.currentStore.items!=1)) 
                {
                    itemsString = itemsString + Convert.ToString(store.currentStore.rows) + GlobalProfile.ArgumentsSeparatorStandard + Convert.ToString(store.currentStore.cols);
                }
            }
            return newSystemStart + itemsString + systemEnd;
        }

        /// <summary>
        /// Translates an SMath matrix given as string to Maxima
        /// Called by Converter.PrepareTermsForMaxima
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MatrixConvert (string input)
		{
			bool doAgain;
			do{
				doAgain=false;
				arrayStart = 0;
				arrayEnd = 0;
				ElementStoreManager store= new ElementStoreManager();
				store = MaximaMatrixDataCollection (store, input);
				if(store.arrayIn)
				{
					input = MakeString(input, MakeTermString(store, "mat(", ")"), arrayStart, arrayEnd);
					doAgain=true;
				}
			}while(doAgain);
            return input;      
		}

        /// <summary>
        /// Translates a nested SMath list given as string to Maxima
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MultiListConvert(string input)
        {
            bool doAgain;
            do
            {
                doAgain = false;
                arrayStart = 0;
                arrayEnd = 0;
                ElementStoreManager store = new ElementStoreManager();
                store = MaximaMultiListDataCollection(store, input);
                if (store.arrayIn)
                {
                    store.flipRowsCols();
                    input = MakeString(input, MakeTermString(store, "mat(", ")"), arrayStart, arrayEnd);
                    doAgain = true;
                }
            } while (doAgain);
            return input;
        }

        /// <summary>
        /// Translates an SMath list given as string to Maxima
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ListConvert(string input)
        {
			bool doAgain;
			do{
				doAgain=false;
				arrayStart = 0;
				arrayEnd = 0;
				ElementStoreManager store= new ElementStoreManager();
				store = MaximaListDataCollection (store, input);
				if(store.arrayIn)
				{
					input = MakeString(input, MakeTermString(store, "sys(", ")"), arrayStart, arrayEnd);
                    firstCall = true;
					doAgain=true;
				}
			}while(doAgain);
			return input;        
		}

        /// <summary>
        /// Converts an SMath matrix given as string to the internal storage structure
        /// </summary>
        /// <param name="store"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ElementStoreManager MaximaMatrixDataCollection(ElementStoreManager store, string input)
        {
            bool inArray = true, foundArray = false;
            int i = 0;
            //Make structure
            while (i < input.Length && inArray)
            {
                if (i < (input.Length - 6) &&
                    input[i + 0] == 'm' &&
                    input[i + 1] == 'a' &&
                    input[i + 2] == 't' &&
                    input[i + 3] == 'r' &&
                    input[i + 4] == 'i' &&
                    input[i + 5] == 'x' &&
                    input[i + 6] == '(')
                {
                    if (foundArray && store.currentStore.charCounter > 0)
                    {
                        store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                        store.currentStore.charCounter = 0;
                    }
                    if (!foundArray)
                    {
                        foundArray = true;
                        arrayStart = i;
                    }
                    store.newElementStore();
                    store.currentStore.bracketOpen++;
                    i += 6;
                }
                else if (foundArray && input[i] == '[' && (store.currentStore.bracketOpen - store.currentStore.bracketClose == 1) && (store.currentStore.squareBracketOpen - store.currentStore.squareBracketClose == 0))
                {
					//store.addNewItemToCurrent();
					store.increaseCounter(1,0,0);
					store.currentStore.squareBracketOpen++;
                }
                else if (foundArray && input[i] == GlobalProfile.ArgumentsSeparatorStandard && (store.currentStore.bracketOpen - store.currentStore.bracketClose == 1) && (store.currentStore.squareBracketOpen - store.currentStore.squareBracketClose == 1))
                {
                    if (store.currentStore.charCounter > 0)
                    {
                        store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                        store.currentStore.charCounter = 0;
                    }
					store.addNewItemToCurrent();
					//cahrcounter?
                }
                else if (foundArray && input[i] == ']' && (store.currentStore.bracketOpen - store.currentStore.bracketClose == 1) && (store.currentStore.squareBracketOpen - store.currentStore.squareBracketClose == 1))
                {
                    if (store.currentStore.charCounter > 0)
                    {
                        store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                        store.currentStore.charCounter = 0;
                    }
					store.currentStore.squareBracketClose++;
                }
                else if (foundArray && input[i] == ')' && (store.currentStore.bracketOpen - store.currentStore.bracketClose == 1) && (store.currentStore.squareBracketOpen - store.currentStore.squareBracketClose == 0))
                {
                    store.currentStore.bracketClose++;
					store.terminate();
                    if (!store.prevElementStore())
                    {
                        inArray = false;
                        arrayEnd = i + 1;
                    }
                }
                else if (foundArray)
                {
                    if (input[i] == '(') { store.currentStore.bracketOpen++; }
                    else if (input[i] == ')') { store.currentStore.bracketClose++; }
                    else if (input[i] == '[') { store.currentStore.squareBracketOpen++; }
                    else if (input[i] == ']') { store.currentStore.squareBracketClose++; }

                    if (input[i] == GlobalProfile.ArgumentsSeparatorStandard && (store.currentStore.bracketOpen - store.currentStore.bracketClose == 1) && (store.currentStore.squareBracketOpen - store.currentStore.squareBracketClose == 0))
						store.addNewItemToCurrent();
					else
                        store.currentStore.charCounter++;
 
                }
                i++;
            }
            store.gotoFirstElementStore();
            return store;
        }

        /// <summary>
        /// Converts an SMath nested lists given as string to the internal storage structure
        /// </summary>
        /// <param name="store"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ElementStoreManager MaximaMultiListDataCollection(ElementStoreManager store, string input)
        {
            bool inArray = true, foundArray = false;
            int i = 0;
            //Make structure
            while (i < input.Length && inArray)
            {
                if (i < (input.Length - 1) &&
                    input[i + 0] == '[' &&
                    input[i + 1] == '[')
                {
                    if (foundArray && store.currentStore.charCounter > 0)
                    {
                        store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                        store.currentStore.charCounter = 0;
                    }
                    else if (foundArray)
                    {
                        store.increaseCounter(1, 0, 0);
                    }

                    if (!foundArray)
                    {
                        foundArray = true;
                        arrayStart = i;
                    }
                    store.newElementStore();
                    store.currentStore.squareBracketOpen++;
                }
                else if (foundArray && i < (input.Length - 1) && input[i] == '[' && input[i + 1] != '[' && (store.currentStore.bracketOpen - store.currentStore.bracketClose == 0))// && (store.currentStore.squareBracketOpen - store.currentStore.squareBracketClose > 0))
                {
                    store.increaseCounter(1, 0, 0);
                    store.currentStore.squareBracketOpen++;
                }
                else if (foundArray && input[i] == GlobalProfile.ArgumentsSeparatorStandard && (store.currentStore.bracketOpen - store.currentStore.bracketClose == 0))// && (store.currentStore.squareBracketOpen - store.currentStore.squareBracketClose >0))
                {
                    if (store.currentStore.charCounter > 0)
                    {
                        store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                        store.currentStore.charCounter = 0;
                    }

                    store.addNewItemToCurrent();
                }

                else if (foundArray && i < (input.Length - 1) && input[i] == ']' && input[i + 1] == ']' && (store.currentStore.bracketOpen - store.currentStore.bracketClose == 0)) //&& (store.currentStore.squareBracketOpen - store.currentStore.squareBracketClose > 0))
                {
                    if (store.currentStore.charCounter > 0)
                    {
                        store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                        store.currentStore.charCounter = 0;
                    }
                    store.currentStore.squareBracketClose++;
                    store.terminate();
                    if (!store.prevElementStore())
                    {
                        inArray = false;
                        arrayEnd = i + 2;
                    }
                }
                else if (foundArray && input[i] == ']' && (store.currentStore.bracketOpen - store.currentStore.bracketClose == 0))// && (store.currentStore.squareBracketOpen - store.currentStore.squareBracketClose >1))
                {
                    if (store.currentStore.charCounter > 0)
                    {
                        store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                        store.currentStore.charCounter = 0;
                    }
                    store.currentStore.squareBracketClose++;
                }
                else if (foundArray)
                {
                    if (input[i] == '(') { store.currentStore.bracketOpen++; }
                    else if (input[i] == ')') { store.currentStore.bracketClose++; }
                    else if (input[i] == '[') { store.currentStore.squareBracketOpen++; }
                    else if (input[i] == ']') { store.currentStore.squareBracketClose++; }

                    if (input[i] == GlobalProfile.ArgumentsSeparatorStandard && (store.currentStore.bracketOpen - store.currentStore.bracketClose == 0) && (store.currentStore.squareBracketOpen - store.currentStore.squareBracketClose > 1))
                        store.addNewItemToCurrent();
                    else
                        store.currentStore.charCounter++;

                }
                i++;
            }
            store.gotoFirstElementStore();
            return store;
        }

        /// <summary>
        /// Converts an SMath list given as string to the internal storage structure
        /// </summary>
        /// <param name="store"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ElementStoreManager MaximaListDataCollection(ElementStoreManager store, string input)
        {
            bool inArray = true, foundArray = false;
            int i = 0;
            //Make structure
            while (i < input.Length && inArray)
            {
                if (input[i] == '[' && (i == 0 || (i > 0 && !Char.IsLetterOrDigit(input[i - 1]))))
                {
                    //Save data to current item
                    if (foundArray && store.currentStore.charCounter > 0)
                    {
                        store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                        store.currentStore.charCounter = 0;
                    }
					if (!foundArray)
                    {
                        foundArray = true;
                        arrayStart = i;
                    }
                    //Create new container
                    store.newElementStore();
               //   store.currentStore.bracketOpen++;
                }
                else if (foundArray && input[i] == GlobalProfile.ArgumentsSeparatorStandard)
                {
	                if (store.currentStore.charCounter > 0 && store.currentStore.bracketOpen - store.currentStore.bracketClose == 0)
                    {
	                    store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                        store.currentStore.charCounter = 0;
						store.addNewItemToCurrent();
                    }
                    else if (store.currentStore.bracketOpen - store.currentStore.bracketClose == 0)
                    {
                        store.addNewItemToCurrent();
                    }
					else
                    {
                        store.currentStore.charCounter++;
                    }
                }
				else if (foundArray && input[i] == ']')
				{
					if (store.currentStore.charCounter > 0)
                    {
                            store.addItemDataToCurrent(input.Substring(i - store.currentStore.charCounter, store.currentStore.charCounter));
                            store.currentStore.charCounter = 0;
                    }
                    //LayerDown
					store.currentStore.rows=store.currentStore.items;
					store.terminate();
                   	if (!store.prevElementStore())
                   	{
                   	    inArray = false;
                   	    arrayEnd = i + 1;
                   	}
				}
                else if(foundArray)
                {
					if 		(input[i] == '(') { store.currentStore.bracketOpen++; }
                    else if (input[i] == ')') { store.currentStore.bracketClose++; }
		            store.currentStore.charCounter++;
                }
                i++;
            }
            store.gotoFirstElementStore();
            return store;
        }
	}
}